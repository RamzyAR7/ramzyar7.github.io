using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PermissionAuth.Context;
using PermissionAuth.Dtos;
using PermissionAuth.Exceptions;
using PermissionAuth.Models;
using PermissionAuth.Service;
using System;

namespace PermissionAuth.Controllers
{
    // Controller
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(GroupName ="Product")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// This EndPoint For Create NEW Product
        /// </summary>
        /// <param name="product"> product Dto</param>
        /// <returns>Product Id</returns>
        [HttpPost]
        [PermissionFilterFactory(Permission.ProductCreate)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            _context.Products.Add(product);

            //var entry = _context.Entry(product);
            //entry.State = EntityState.Deleted;
            //Console.WriteLine(entry.State);

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        /// <summary>
        /// This Endpoint for Get Product by Product Id
        /// </summary>
        /// <param name="id"> Product Id</param>
        /// <returns> Product obj</returns>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
               throw new ProductIsNotFoundException();
            }
            return Ok(product);
        }
    }
}
