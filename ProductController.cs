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
        [HttpGet]
        public IActionResult GetProduct()
        {
            List<Product> products = new List<Product>();
            products.Add(new Product("mouse", 30f, "computer accessory"));
            products.Add(new Product("keyboard", 40f, "computer accessory"));
            products.Add(new Product("monitor", 150f, "computer accessory"));
            
            string response = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            return Ok(response);
        }

    }
}