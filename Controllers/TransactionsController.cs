
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneyTracker.DTOs;
using MoneyTracker.Models;

namespace MoneyTracker.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;
        public TransactionsController(ILogger<TransactionsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<TransactionResponse>>> GetAllTransactions()
        {
            // Logic to retrieve all transactions
            _logger.LogInformation("Retrieving all transactions.");
            return Ok(new List<TransactionResponse>()); // Placeholder for actual data retrieval
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponse>> GetTransactionById(int id)
        {
            // Logic to retrieve a transaction by id
            _logger.LogInformation($"Retrieving transaction with ID: {id}");
            return Ok(new TransactionResponse()); // Placeholder for actual data retrieval
        }

        [HttpPost]
        public async Task<ActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            // Logic to create a new transaction
            _logger.LogInformation("Creating a new transaction.");
            var transaction = new Transaction
            {
                Title = request.Title,
                Description = request.Description,
                Amount = request.Amount,
                Type = request.Type,
                Method = request.Method
            };

            return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTransaction(int id, [FromBody] UpdateTransactionRequest request)
        {
            // Logic to update an existing transaction
            _logger.LogInformation($"Updating transaction with ID: {id}");
            var transaction = new Transaction
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                Amount = request.Amount,
                Type = request.Type,
                Method = request.Method
            };

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            // Logic to delete a transaction
            _logger.LogInformation($"Deleting transaction with ID: {id}");
            // Here you would typically mark the transaction as deleted or remove it from the database
            return NoContent();
        }
    }
}