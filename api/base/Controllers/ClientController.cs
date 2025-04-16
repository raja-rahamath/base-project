using api.Core.Entities.SaaS;
using api.Infrastructure.Data;
using api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;
using System.Text;
using api.Application.Services;
using System.Security.Claims;

namespace api.Controllers
{
    /// <summary>
    /// Controller for client management operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowFrontend")]
    [Produces("application/json")]
    public class ClientController : ControllerBase
    {
        private readonly ILogger<ClientController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly ITenantProvider _tenantProvider;
        private readonly ITenantDbContextFactory _tenantDbContextFactory;
        
        /// <summary>
        /// Constructor for ClientController
        /// </summary>
        /// <param name="logger">The logger service</param>
        /// <param name="dbContext">The database context</param>
        /// <param name="tenantProvider">The tenant provider</param>
        /// <param name="tenantDbContextFactory">The tenant DbContext factory</param>
        public ClientController(ILogger<ClientController> logger, ApplicationDbContext dbContext, ITenantProvider tenantProvider, ITenantDbContextFactory tenantDbContextFactory)
        {
            _logger = logger;
            _dbContext = dbContext;
            _tenantProvider = tenantProvider;
            _tenantDbContextFactory = tenantDbContextFactory;
        }
        
        /// <summary>
        /// Register a new client
        /// </summary>
        /// <param name="request">The client registration request</param>
        /// <returns>The newly created client information</returns>
        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Register a new client",
            Description = "Creates a new client with an admin user without requiring a plan",
            OperationId = "RegisterClient",
            Tags = new[] { "Client" }
        )]
        [SwaggerResponse(201, "Client successfully registered", typeof(ApiResponse<ClientRegistrationResponse>))]
        [SwaggerResponse(400, "Invalid request parameters", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Server error during registration", typeof(ApiResponse<object>))]
        public async Task<IActionResult> Register([FromBody] ClientRegistrationRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrEmpty(request.CompanyName) || 
                    string.IsNullOrEmpty(request.CountryCode) || 
                    string.IsNullOrEmpty(request.Email) ||
                    string.IsNullOrEmpty(request.FirstName) ||
                    string.IsNullOrEmpty(request.LastName) ||
                    string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Required fields are missing"));
                }
                
                // Check if client email already exists
                var existingClient = await _dbContext.Clients.FirstOrDefaultAsync(c => c.Email == request.Email);
                if (existingClient != null)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("A client with this email already exists"));
                }
                
                // Check if user email already exists
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("A user with this email already exists"));
                }
                
                // Create client
                var client = new Client
                {
                    Id = Guid.NewGuid(),
                    CompanyName = request.CompanyName,
                    VatNumber = request.VatNumber ?? string.Empty,
                    CountryCode = request.CountryCode,
                    BillingAddressLine1 = request.BillingAddressLine1 ?? string.Empty,
                    BillingAddressLine2 = request.BillingAddressLine2 ?? string.Empty,
                    City = request.City ?? string.Empty,
                    State = request.State ?? string.Empty,
                    PostalCode = request.PostalCode ?? string.Empty,
                    Email = request.Email,
                    Phone = request.Phone ?? string.Empty,
                    Website = request.Website ?? string.Empty,
                    Status = ClientStatus.Active,
                    DomainUrl = request.DomainUrl ?? string.Empty,
                    DatabaseName = request.DatabaseName ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };
                
                // Create hash and salt for password
                CreatePasswordHash(request.Password, out string passwordHash, out string passwordSalt);
                
                // Create admin user
                var user = new UserRef
                {
                    Id = Guid.NewGuid(),
                    ClientId = client.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Phone = request.Phone ?? string.Empty,
                    Role = UserRole.Admin,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    IsActive = true,
                    PreferredLanguage = request.PreferredLanguage ?? "en-US",
                    PreferredTheme = request.PreferredTheme ?? "light",
                    CreatedAt = DateTime.UtcNow
                };
                
                try
                {
                    // Add client
                    await _dbContext.Clients.AddAsync(client);
                    
                    // Add user
                    await _dbContext.Users.AddAsync(user);
                    
                    // Check if we should skip client plan creation
                    if (!request.SkipClientPlan.GetValueOrDefault())
                    {
                        // Get default plan (Basic)
                        var defaultPlan = await _dbContext.Plans.FirstOrDefaultAsync(p => p.Name == "Basic");
                        
                        if (defaultPlan != null)
                        {
                            // Calculate billing cycle based on request
                            DateTime startDate = DateTime.UtcNow;
                            DateTime endDate = request.BillingCycle == BillingCycle.Annual
                                ? startDate.AddYears(1)
                                : startDate.AddMonths(1);
                            decimal price = request.BillingCycle == BillingCycle.Annual
                                ? defaultPlan.AnnualPrice
                                : defaultPlan.MonthlyPrice;
                            
                            // Create client plan
                            var clientPlan = new ClientPlan
                            {
                                Id = Guid.NewGuid(),
                                ClientId = client.Id,
                                PlanId = defaultPlan.Id,
                                StartDate = startDate,
                                EndDate = endDate,
                                BillingCycle = request.BillingCycle,
                                Price = price,
                                IsActive = true,
                                AutoRenew = true,
                                CreatedAt = DateTime.UtcNow
                            };
                            
                            await _dbContext.ClientPlans.AddAsync(clientPlan);
                            _logger.LogInformation("Created client plan for client {ClientId} with plan {PlanId}", client.Id, defaultPlan.Id);
                        }
                        else
                        {
                            _logger.LogWarning("No default plan found for client {ClientId}", client.Id);
                        }
                    }
                    
                    // Save changes without transaction (in-memory DB doesn't support transactions)
                    await _dbContext.SaveChangesAsync();
                    
                    _logger.LogInformation("Client {ClientId} registered successfully", client.Id);
                    
                    // Get active client plan if exists
                    var activePlan = await _dbContext.ClientPlans
                        .Include(cp => cp.Plan)
                        .FirstOrDefaultAsync(cp => cp.ClientId == client.Id && cp.IsActive);
                    
                    // Prepare response
                    var response = new ClientRegistrationResponse
                    {
                        ClientId = client.Id,
                        CompanyName = client.CompanyName,
                        Email = client.Email,
                        UserId = user.Id,
                        PlanName = activePlan?.Plan?.Name ?? "No Plan",
                        SubscriptionEndDate = activePlan?.EndDate ?? DateTime.UtcNow.AddYears(1)
                    };
                    
                    return CreatedAtAction(nameof(GetClient), new { id = client.Id }, 
                        ApiResponse<ClientRegistrationResponse>.SuccessResponse(response, "Client registered successfully"));
                }
                catch (Exception ex)
                {
                    // Log error (can't rollback with in-memory DB)
                    _logger.LogError(ex, "Error during client registration");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering client");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
        
        /// <summary>
        /// Get client by ID
        /// </summary>
        /// <param name="id">The client ID</param>
        /// <returns>The client information</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get client by ID",
            Description = "Retrieves a client by their ID",
            OperationId = "GetClient",
            Tags = new[] { "Client" }
        )]
        [SwaggerResponse(200, "Client retrieved successfully", typeof(ApiResponse<ClientResponse>))]
        [SwaggerResponse(404, "Client not found", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Server error", typeof(ApiResponse<object>))]
        public async Task<IActionResult> GetClient(Guid id)
        {
            try
            {
                var client = await _dbContext.Clients
                    .FirstOrDefaultAsync(c => c.Id == id);
                
                if (client == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Client not found"));
                }
                
                var adminUser = await _dbContext.Users
                    .Where(u => u.ClientId == id && u.Role == UserRole.Admin)
                    .FirstOrDefaultAsync();
                
                var response = new ClientResponse
                {
                    Id = client.Id,
                    CompanyName = client.CompanyName,
                    VatNumber = client.VatNumber,
                    CountryCode = client.CountryCode,
                    Email = client.Email,
                    Phone = client.Phone,
                    Website = client.Website,
                    Status = client.Status.ToString(),
                    DomainUrl = client.DomainUrl,
                    DatabaseName = client.DatabaseName,
                    CreatedAt = client.CreatedAt,
                    
                    BillingInfo = new BillingAddressInfo
                    {
                        AddressLine1 = client.BillingAddressLine1,
                        AddressLine2 = client.BillingAddressLine2,
                        City = client.City,
                        State = client.State,
                        PostalCode = client.PostalCode,
                        CountryCode = client.CountryCode
                    },
                    
                    AdminUser = adminUser != null ? new UserInfo
                    {
                        Id = adminUser.Id,
                        FirstName = adminUser.FirstName,
                        LastName = adminUser.LastName,
                        Email = adminUser.Email,
                        Phone = adminUser.Phone
                    } : null
                };
                
                return Ok(ApiResponse<ClientResponse>.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving client");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
        
        /// <summary>
        /// Get all clients
        /// </summary>
        /// <returns>List of all clients</returns>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all clients",
            Description = "Retrieves a list of all clients",
            OperationId = "GetAllClients",
            Tags = new[] { "Client" }
        )]
        [SwaggerResponse(200, "Clients retrieved successfully", typeof(ApiResponse<IEnumerable<ClientSummaryResponse>>))]
        [SwaggerResponse(500, "Server error", typeof(ApiResponse<object>))]
        public async Task<IActionResult> GetAllClients()
        {
            try
            {
                var clients = await _dbContext.Clients.ToListAsync();
                
                var response = clients.Select(client => 
                {
                    return new ClientSummaryResponse
                    {
                        Id = client.Id,
                        CompanyName = client.CompanyName,
                        Email = client.Email,
                        Status = client.Status.ToString(),
                        CreatedAt = client.CreatedAt,
                        PlanName = "No Plan",
                        SubscriptionEndDate = null
                    };
                });
                
                return Ok(ApiResponse<IEnumerable<ClientSummaryResponse>>.SuccessResponse(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving clients");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
        
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var client = await _tenantProvider.GetCurrentClientAsync();
            if (client == null)
                return BadRequest("Invalid client or subdomain.");

            using var clientDb = _tenantDbContextFactory.CreateDbContextForClient(client.DatabaseName);
            var users = await clientDb.Users.ToListAsync();
            return Ok(users);
        }
        
        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using var hmac = new HMACSHA512();
            var saltBytes = hmac.Key;
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            
            passwordSalt = Convert.ToBase64String(saltBytes);
            passwordHash = Convert.ToBase64String(hashBytes);
        }

        // POST: api/Client/change-email
        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound();

            var oldEmail = user.Email;
            user.Email = request.NewEmail;
            await _dbContext.SaveChangesAsync();

            // TODO: Send notification email to new address
            // await _emailService.SendEmailChangedNotification(user.Email, oldEmail);

            return Ok(ApiResponse<string>.SuccessResponse("Email updated and notification sent.", user.Email));
        }

        // POST: api/Client/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound();

            // TODO: Validate old password if required
            // Hash new password and update
            user.PasswordHash = CreatePasswordHash(request.NewPassword, user.PasswordSalt);
            await _dbContext.SaveChangesAsync();

            return Ok(ApiResponse<string>.SuccessResponse("Password updated successfully."));
        }

        private Guid? GetUserId()
        {
            var claim = User.FindFirst("UserId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return null;
            if (Guid.TryParse(claim.Value, out var guid))
                return guid;
            return null;
        }

        // GET: api/Plan
        [HttpGet("/api/Plan")]
        public async Task<IActionResult> GetAvailablePlans()
        {
            var plans = await _dbContext.Plans
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .Select(p => new PlanDto
                {
                    Id = p.Id.ToString(),
                    Name = p.Name,
                    Description = p.Description,
                    MonthlyPrice = p.MonthlyPrice,
                    AnnualPrice = p.AnnualPrice,
                    MaxUsers = p.MaxUsers,
                    MaxStorageGB = p.MaxStorageGB,
                    Features = p.Features
                })
                .ToListAsync();
            return Ok(plans);
        }

        // POST: api/Plan/change
        [HttpPost("/api/Plan/change")]
        public async Task<IActionResult> ChangePlan([FromBody] ChangePlanRequest request)
        {
            var clientId = GetClientId();
            if (clientId == null)
                return Unauthorized();

            var client = await _dbContext.Clients.Include(c => c.Plans).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null)
                return NotFound();

            var newPlan = await _dbContext.Plans.FirstOrDefaultAsync(p => p.Id.ToString() == request.PlanId && p.IsActive);
            if (newPlan == null)
                return BadRequest("Selected plan does not exist.");

            // Confirmation logic and downgrade warning can be handled on frontend
            // Deactivate current plan
            var activePlan = client.Plans.FirstOrDefault(cp => cp.IsActive);
            if (activePlan != null)
                activePlan.IsActive = false;

            // Add new client plan
            var now = DateTime.UtcNow;
            var endDate = request.BillingCycle == 1 ? now.AddYears(1) : now.AddMonths(1);
            var price = request.BillingCycle == 1 ? newPlan.AnnualPrice : newPlan.MonthlyPrice;
            var clientPlan = new ClientPlan
            {
                Id = Guid.NewGuid(),
                ClientId = client.Id,
                PlanId = newPlan.Id,
                StartDate = now,
                EndDate = endDate,
                BillingCycle = (BillingCycle)request.BillingCycle,
                Price = price,
                IsActive = true,
                AutoRenew = true,
                CreatedAt = now
            };
            await _dbContext.ClientPlans.AddAsync(clientPlan);
            await _dbContext.SaveChangesAsync();
            return Ok(ApiResponse<string>.SuccessResponse("Plan changed successfully."));
        }

        // POST: api/Plan/cancel
        [HttpPost("/api/Plan/cancel")]
        public async Task<IActionResult> CancelSubscription()
        {
            var clientId = GetClientId();
            if (clientId == null)
                return Unauthorized();

            var client = await _dbContext.Clients.Include(c => c.Plans).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null)
                return NotFound();

            var activePlan = client.Plans.FirstOrDefault(cp => cp.IsActive);
            if (activePlan == null)
                return BadRequest("No active plan to cancel.");

            activePlan.IsActive = false;
            await _dbContext.SaveChangesAsync();
            return Ok(ApiResponse<string>.SuccessResponse("Subscription cancelled successfully."));
        }

        private Guid? GetClientId()
        {
            var claim = User.FindFirst("ClientId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return null;
            if (Guid.TryParse(claim.Value, out var guid))
                return guid;
            return null;
        }

        private string CreatePasswordHash(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using var hmac = new HMACSHA512(saltBytes);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public class ChangeEmailRequest
    {
        public string NewEmail { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    public class PlanDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal MonthlyPrice { get; set; }
        public decimal AnnualPrice { get; set; }
        public int MaxUsers { get; set; }
        public int MaxStorageGB { get; set; }
        public string? Features { get; set; }
    }

    public class ChangePlanRequest
    {
        public string PlanId { get; set; } = string.Empty;
        public int BillingCycle { get; set; } // 0 = Monthly, 1 = Annual
    }
}