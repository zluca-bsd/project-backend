using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace project_backend
{
    [ApiController]
    [Route("api/Customers")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;            
        }

        [HttpGet]
        public IActionResult GetCustomer()
        {
            var customers = _context.Customers.ToList();
            return Ok(customers);
        }
    }
}