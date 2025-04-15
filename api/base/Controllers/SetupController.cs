using api.Application.Services;
using api.Core.Entities.SaaS;
using api.Infrastructure.Data;
using api.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    /// <summary>
    /// Controller for system setup operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowFrontend")]
    [Produces("application/json")]
    public class SetupController : ControllerBase
    {
        private readonly ILogger<SetupController> _logger;
        private readonly ApplicationDbContext _dbContext;

        /// <summary>
        /// Constructor for SetupController
        /// </summary>
        /// <param name="logger">The logger service</param>
        /// <param name="dbContext">The database context</param>
        public SetupController(ILogger<SetupController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Initialize default plans in the database
        /// </summary>
        /// <returns>Result of the initialization</returns>
        [HttpPost("initialize-plans")]
        [SwaggerOperation(
            Summary = "Initialize default subscription plans",
            Description = "Creates the default set of subscription plans in the database if they don't exist",
            OperationId = "InitializePlans",
            Tags = new[] { "Setup" }
        )]
        [SwaggerResponse(200, "Plans initialized successfully", typeof(ApiResponse<object>))]
        [SwaggerResponse(500, "Server error during initialization", typeof(ApiResponse<object>))]
        public async Task<IActionResult> InitializePlans()
        {
            try
            {
                _logger.LogInformation("Initializing default subscription plans");

                // Check if plans already exist
                var existingPlans = _dbContext.Plans.Any();
                if (existingPlans)
                {
                    return Ok(ApiResponse<object>.SuccessResponse(
                        new { message = "Plans already exist in the database" }));
                }

                // Create basic plan
                var basicPlan = new Plan
                {
                    Id = Guid.NewGuid(),
                    Name = "Basic",
                    Description = "Essential features for small businesses",
                    MonthlyPrice = 29.99m,
                    AnnualPrice = 299.99m,
                    MaxUsers = 10,
                    MaxStorageGB = 5,
                    Features = "{\"feature1\":true,\"feature2\":true,\"feature3\":false,\"feature4\":false}",
                    IsActive = true,
                    DisplayOrder = 1
                };

                // Create standard plan
                var standardPlan = new Plan
                {
                    Id = Guid.NewGuid(),
                    Name = "Standard",
                    Description = "Advanced features for growing businesses",
                    MonthlyPrice = 79.99m,
                    AnnualPrice = 799.99m,
                    MaxUsers = 50,
                    MaxStorageGB = 25,
                    Features = "{\"feature1\":true,\"feature2\":true,\"feature3\":true,\"feature4\":false}",
                    IsActive = true,
                    DisplayOrder = 2
                };

                // Create premium plan
                var premiumPlan = new Plan
                {
                    Id = Guid.NewGuid(),
                    Name = "Premium",
                    Description = "Full features for enterprises",
                    MonthlyPrice = 149.99m,
                    AnnualPrice = 1499.99m,
                    MaxUsers = 100,
                    MaxStorageGB = 100,
                    Features = "{\"feature1\":true,\"feature2\":true,\"feature3\":true,\"feature4\":true}",
                    IsActive = true,
                    DisplayOrder = 3
                };

                // Add plans to database
                await _dbContext.Plans.AddRangeAsync(basicPlan, standardPlan, premiumPlan);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully initialized default subscription plans");

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { 
                        message = "Plans initialized successfully", 
                        plans = new[] { 
                            new { id = basicPlan.Id, name = basicPlan.Name, price = basicPlan.MonthlyPrice },
                            new { id = standardPlan.Id, name = standardPlan.Name, price = standardPlan.MonthlyPrice },
                            new { id = premiumPlan.Id, name = premiumPlan.Name, price = premiumPlan.MonthlyPrice }
                        }
                    }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing default plans");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}