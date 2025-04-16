using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using api.Infrastructure.Data;
using api.Core.Entities.SaaS;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BillingController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<BillingController> _logger;

        public BillingController(ApplicationDbContext db, ILogger<BillingController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: api/Billing
        [HttpGet]
        public async Task<IActionResult> GetBillingRecords()
        {
            var clientId = GetClientId();
            if (clientId == null)
                return Unauthorized();

            var records = await _db.BillingRecords
                .Where(b => b.ClientId == clientId)
                .OrderByDescending(b => b.BillingPeriodEnd)
                .Select(b => new BillingRecordDto
                {
                    Id = b.Id.ToString(),
                    InvoiceNumber = b.InvoiceNumber,
                    Amount = b.Amount,
                    Currency = b.Currency,
                    BillingPeriodStart = b.BillingPeriodStart,
                    BillingPeriodEnd = b.BillingPeriodEnd,
                    Status = b.Status.ToString(),
                    CreatedAt = b.CreatedAt,
                    PaidAt = b.PaidAt
                })
                .ToListAsync();

            return Ok(records);
        }

        // GET: api/Billing/{id}/invoice?format=pdf|csv
        [HttpGet("{id}/invoice")]
        public async Task<IActionResult> DownloadInvoice(string id, [FromQuery] string format = "pdf")
        {
            var clientId = GetClientId();
            if (clientId == null)
                return Unauthorized();

            var record = await _db.BillingRecords.FirstOrDefaultAsync(b => b.Id.ToString() == id && b.ClientId == clientId);
            if (record == null)
                return NotFound();

            // For demonstration, just return a dummy file. Replace with real invoice generation.
            var fileName = $"Invoice_{record.InvoiceNumber}.{format}";
            var fileContent = System.Text.Encoding.UTF8.GetBytes($"Invoice: {record.InvoiceNumber}\nAmount: {record.Amount} {record.Currency}");
            var contentType = format == "csv" ? "text/csv" : "application/pdf";
            return File(fileContent, contentType, fileName);
        }

        private Guid? GetClientId()
        {
            var claim = User.FindFirst("ClientId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return null;
            if (Guid.TryParse(claim.Value, out var guid))
                return guid;
            return null;
        }
    }

    public class BillingRecordDto
    {
        public string Id { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime BillingPeriodStart { get; set; }
        public DateTime BillingPeriodEnd { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
