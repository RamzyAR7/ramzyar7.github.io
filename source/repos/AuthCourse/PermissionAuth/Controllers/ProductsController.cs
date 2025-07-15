using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PermissionAuth.Context;
using PermissionAuth.Dtos;
using PermissionAuth.Models;
using PermissionAuth.Service;
using System;

namespace PermissionAuth.Controllers
{
    // Controller
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [PermissionFilterFactory(Permission.ProductCreate)]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            _context.Products.Add(product);

            //var entry = _context.Entry(product);
            //entry.State = EntityState.Deleted;
            //Console.WriteLine(entry.State);

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpGet("{id}")]
        [PermissionFilterFactory(Permission.ProductRead)]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
    }
}
