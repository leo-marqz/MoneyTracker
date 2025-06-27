
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyTracker.Models;

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

        [HttpGet("all")]
        [Authorize(Roles = SystemRole.ADMIN)]
        public ActionResult<IEnumerable<string>> Reports()
        {
            return Ok(new List<string>()
            {
                "reporte1.txt", "reporte2.txt", "reporte3.txt"
            });
        }

        [HttpGet("me")]
        [Authorize(Roles = SystemRole.USER)]
        public ActionResult<IEnumerable<string>> MyReports()
        {
            return Ok(new List<string>());
        }
    }
}