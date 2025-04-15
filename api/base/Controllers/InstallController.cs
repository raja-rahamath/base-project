using api.Core.Entities;
using api.Core.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Data.Common;

namespace api.Controllers
{
    /// <summary>
    /// Controller for database installation operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowFrontend")]
    [Produces("application/json")]
    public class InstallController : ControllerBase
    {
        private readonly ILogger<InstallController> _logger;
        private readonly IDatabaseService _databaseService;
        private readonly IRepository<ConnectionConfig> _repository;

        /// <summary>
        /// Constructor for InstallController
        /// </summary>
        /// <param name="logger">The logger service</param>
        /// <param name="databaseService">The database service</param>
        /// <param name="repository">The connection configuration repository</param>
        public InstallController(
            ILogger<InstallController> logger, 
            IDatabaseService databaseService,
            IRepository<ConnectionConfig> repository)
        {
            _logger = logger;
            _databaseService = databaseService;
            _repository = repository;
        }

        /// <summary>
        /// Test endpoint to verify the API is working
        /// </summary>
        /// <returns>A success message</returns>
        [HttpGet("test")]
        [SwaggerOperation(
            Summary = "Test API connection", 
            Description = "Simple endpoint to verify the API is working",
            OperationId = "GetTest",
            Tags = new[] { "Install" }
        )]
        [SwaggerResponse(200, "API is working properly", typeof(ApiResponse<object>))]
        public IActionResult Test()
        {
            _logger.LogInformation("Test endpoint called at {Time}", DateTime.UtcNow);
            return Ok(ApiResponse<object>.SuccessResponse(new { message = "API is working!" }));
        }

        /// <summary>
        /// Get supported database types
        /// </summary>
        /// <returns>List of supported database types</returns>
        [HttpGet("types")]
        [SwaggerOperation(
            Summary = "Get supported database types",
            Description = "Returns a list of database types supported by this API",
            OperationId = "GetDatabaseTypes",
            Tags = new[] { "Install" }
        )]
        [SwaggerResponse(200, "List of supported database types", typeof(ApiResponse<IEnumerable<string>>))]
        public IActionResult GetDatabaseTypes()
        {
            var types = _databaseService.GetSupportedDatabaseTypes();
            return Ok(ApiResponse<IEnumerable<string>>.SuccessResponse(types, "Supported database types"));
        }

        /// <summary>
        /// Get all saved database configurations
        /// </summary>
        /// <returns>List of database configurations</returns>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all database configurations",
            Description = "Retrieves all saved database configurations",
            OperationId = "GetConfigurations",
            Tags = new[] { "Install" }
        )]
        [SwaggerResponse(200, "List of database configurations", typeof(ApiResponse<IEnumerable<ConnectionConfig>>))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var configs = await _repository.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<ConnectionConfig>>.SuccessResponse(configs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database configurations");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
        
        /// <summary>
        /// Checks if a database or user already exists
        /// </summary>
        /// <param name="request">The connection request details</param>
        /// <returns>True if the database or user exists</returns>
        [HttpPost("check")]
        [SwaggerOperation(
            Summary = "Check if database or user exists",
            Description = "Checks if a database or user already exists with the specified configuration",
            OperationId = "CheckDatabaseExists",
            Tags = new[] { "Install" }
        )]
        [SwaggerResponse(200, "Result of check operation", typeof(ApiResponse<bool>))]
        [SwaggerResponse(400, "Invalid request parameters", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Server error during check", typeof(ApiResponse<object>))]
        public async Task<IActionResult> CheckExists([FromBody] InstallRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrEmpty(request.DbType) || 
                    string.IsNullOrEmpty(request.ServiceIP) || 
                    string.IsNullOrEmpty(request.Port) || 
                    string.IsNullOrEmpty(request.DbName) ||
                    string.IsNullOrEmpty(request.RootUser) ||
                    string.IsNullOrEmpty(request.RootPassword) ||
                    string.IsNullOrEmpty(request.AdminUser))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Required fields are missing"));
                }

                var exists = await _databaseService.CheckDatabaseOrUserExistsAsync(request);
                return Ok(ApiResponse<bool>.SuccessResponse(exists, 
                    exists ? "Database or user already exists" : "Database and user do not exist"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking database existence");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Creates a new database and user with the specified configuration
        /// </summary>
        /// <param name="request">The installation request details</param>
        /// <returns>Success response with installation details</returns>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Create a new database and user",
            Description = "Creates a new database and associated user with the specified configuration",
            OperationId = "CreateDatabaseAndUser",
            Tags = new[] { "Install" }
        )]
        [SwaggerResponse(200, "Database and user created successfully", typeof(ApiResponse<object>))]
        [SwaggerResponse(400, "Invalid request parameters or database/user already exists", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Server error during installation", typeof(ApiResponse<object>))]
        public async Task<IActionResult> Install([FromBody] InstallRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrEmpty(request.DbType) || 
                    string.IsNullOrEmpty(request.ServiceIP) || 
                    string.IsNullOrEmpty(request.Port) || 
                    string.IsNullOrEmpty(request.DbName) ||
                    string.IsNullOrEmpty(request.RootUser) ||
                    string.IsNullOrEmpty(request.RootPassword) ||
                    string.IsNullOrEmpty(request.AdminUser) ||
                    string.IsNullOrEmpty(request.AdminPassword))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("All fields are required"));
                }

                _logger.LogInformation("Processing installation request for {DbType} database {DbName}", 
                    request.DbType, request.DbName);

                // Check if database or user already exists
                if (await _databaseService.CheckDatabaseOrUserExistsAsync(request))
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Database or user already exists"));
                }

                // Create database and user
                var success = await _databaseService.CreateDatabaseAndUserAsync(request);
                
                if (success)
                {
                    return Ok(ApiResponse<object>.SuccessResponse(
                        new { 
                            message = "Database and user created successfully",
                            note = "If your password didn't meet MySQL's policy requirements, it may have been strengthened automatically. Please check the server logs for details.",
                            saasTables = new {
                                clients = "cor_clients",
                                users = "cor_users_ref",
                                plans = "cor_plans",
                                clientPlans = "cor_client_plans",
                                clientRenewals = "cor_client_renewals",
                                paymentMethods = "cor_payment_methods"
                            },
                            defaultUrl = $"http://{request.ServiceIP}:{request.Port}/{request.DbName}"
                        }));
                }
                else
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Failed to create database and user"));
                }
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Database error during installation: {Message}", ex.Message);
                
                // Customize error message based on database type
                if (request.DbType.ToLower() == "mysql" && ex.Message.Contains("Access denied"))
                {
                    return StatusCode(500, ApiResponse<object>.ErrorResponse(
                        ex.Message, 
                        "Authentication failed. Please check your MySQL credentials and ensure the user has sufficient privileges. " +
                        "For MySQL 8+, you may need to configure the server to accept the authentication method used."
                    ));
                }
                
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, "Database error occurred"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database installation: {Message}", ex.Message);
                
                // Provide more useful error messages for common scenarios
                if (ex.Message.Contains("No such host is known"))
                {
                    return StatusCode(500, ApiResponse<object>.ErrorResponse(
                        ex.Message, 
                        $"Could not connect to database server at {request.ServiceIP}:{request.Port}. Please verify the server address and port."
                    ));
                }
                else if (ex.Message.Contains("Connection refused") || ex.Message.Contains("Connection timeout"))
                {
                    return StatusCode(500, ApiResponse<object>.ErrorResponse(
                        ex.Message, 
                        $"Could not connect to database server at {request.ServiceIP}:{request.Port}. Please ensure the server is running and accepting connections."
                    ));
                }
                else if (ex.Message.Contains("password") && ex.Message.Contains("policy"))
                {
                    return StatusCode(500, ApiResponse<object>.ErrorResponse(
                        ex.Message, 
                        "The password does not meet MySQL's policy requirements. Please use a stronger password with at least 8 characters, " +
                        "including uppercase and lowercase letters, numbers, and special characters."
                    ));
                }
                
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}