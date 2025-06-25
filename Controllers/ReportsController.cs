
using Microsoft.AspNetCore.Mvc;

namespace MoneyTracker.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ReportsController : ControllerBase
    {
        /*
        * GET: api/reports/csv/transactions/me -> my transactions (requires authentication)
        * GET: api/reports/csv/transactions/{userId} -> transactions by user (admin only)
        * GET: api/reports/csv/transactions/last30days -> transactions from the last 30 days (requires authentication)
        * GET: api/reports/csv/transactions/all -> all transactions (admin only)
        */
    }
}