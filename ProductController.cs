using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace project_backend
{
    [ApiController]
    [Route("api/Products")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProduct()
        {
            // products.Add(new Product("mouse", 30f, "computer accessory"));
            // products.Add(new Product("keyboard", 40f, "computer accessory"));
            // products.Add(new Product("monitor", 150f, "computer accessory"));

            var products = _context.Products.ToList();
            return Ok(products);
        }
    }
}