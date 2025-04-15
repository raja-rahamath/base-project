using api.Core.Entities.SaaS;
using api.Infrastructure.Data;
using api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Cryptography;
using System.Text;

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
        
        /// <summary>
        /// Constructor for ClientController
        /// </summary>
        /// <param name="logger">The logger service</param>
        /// <param name="dbContext">The database context</param>
        public ClientController(ILogger<ClientController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        
        /// <summary>
        /// Register a new client
        /// </summary>
        /// <param name="request">The client registration request</param>
        /// <returns>The newly created client information</returns>
        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Register a new client",
            Description = "Creates a new client with an admin user and assigns a plan",
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
                
                // Get default plan (first active plan)
                var defaultPlan = await _dbContext.Plans
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.DisplayOrder)
                    .FirstOrDefaultAsync();
                
                if (defaultPlan == null)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("No active plans found"));
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
                
                // Calculate subscription dates
                DateTime startDate = DateTime.UtcNow;
                DateTime endDate = request.BillingCycle == BillingCycle.Annual 
                    ? startDate.AddYears(1) 
                    : startDate.AddMonths(1);
                
                // Create client plan
                var clientPlan = new ClientPlan
                {
                    Id = Guid.NewGuid(),
                    ClientId = client.Id,
                    PlanId = defaultPlan.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    BillingCycle = request.BillingCycle,
                    Price = request.BillingCycle == BillingCycle.Annual ? defaultPlan.AnnualPrice : defaultPlan.MonthlyPrice,
                    IsActive = true,
                    AutoRenew = true,
                    CreatedAt = DateTime.UtcNow
                };
                
                // Begin transaction
                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                
                try
                {
                    // Add client
                    await _dbContext.Clients.AddAsync(client);
                    
                    // Add user
                    await _dbContext.Users.AddAsync(user);
                    
                    // Add client plan
                    await _dbContext.ClientPlans.AddAsync(clientPlan);
                    
                    // Save changes
                    await _dbContext.SaveChangesAsync();
                    
                    // Commit transaction
                    await transaction.CommitAsync();
                    
                    _logger.LogInformation("Client {ClientId} registered successfully", client.Id);
                    
                    // Prepare response
                    var response = new ClientRegistrationResponse
                    {
                        ClientId = client.Id,
                        CompanyName = client.CompanyName,
                        Email = client.Email,
                        UserId = user.Id,
                        PlanName = defaultPlan.Name,
                        SubscriptionEndDate = clientPlan.EndDate
                    };
                    
                    return CreatedAtAction(nameof(GetClient), new { id = client.Id }, 
                        ApiResponse<ClientRegistrationResponse>.SuccessResponse(response, "Client registered successfully"));
                }
                catch (Exception ex)
                {
                    // Rollback transaction
                    await transaction.RollbackAsync();
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
                    .Include(c => c.Plans)
                    .ThenInclude(cp => cp.Plan)
                    .FirstOrDefaultAsync(c => c.Id == id);
                
                if (client == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Client not found"));
                }
                
                var activePlan = client.Plans.FirstOrDefault(cp => cp.IsActive);
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
                    
                    Plan = activePlan != null ? new ClientPlanInfo
                    {
                        PlanId = activePlan.PlanId,
                        PlanName = activePlan.Plan.Name,
                        StartDate = activePlan.StartDate,
                        EndDate = activePlan.EndDate,
                        BillingCycle = activePlan.BillingCycle.ToString(),
                        Price = activePlan.Price,
                        AutoRenew = activePlan.AutoRenew
                    } : null,
                    
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
                var clients = await _dbContext.Clients
                    .Include(c => c.Plans)
                    .ThenInclude(cp => cp.Plan)
                    .ToListAsync();
                
                var response = clients.Select(client => 
                {
                    var activePlan = client.Plans.FirstOrDefault(cp => cp.IsActive);
                    
                    return new ClientSummaryResponse
                    {
                        Id = client.Id,
                        CompanyName = client.CompanyName,
                        Email = client.Email,
                        Status = client.Status.ToString(),
                        CreatedAt = client.CreatedAt,
                        PlanName = activePlan?.Plan.Name ?? "No Plan",
                        SubscriptionEndDate = activePlan?.EndDate
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
        
        private void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using var hmac = new HMACSHA512();
            var saltBytes = hmac.Key;
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            
            passwordSalt = Convert.ToBase64String(saltBytes);
            passwordHash = Convert.ToBase64String(hashBytes);
        }
    }
}