//api-controller-async
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_netcore_dotnet06.Helper;
using backend_netcore_dotnet06.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
//using backend_netcore_dotnet06.Models;

namespace backend_netcore_dotnet06.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductStoreContext _context;
        //Read
        public ProductController(ProductStoreContext context)
        {
            _context = context;
        }


        [HttpGet("GetAllProducts")]
        public async Task<ActionResult> GetAllProductsSQLRaw()
        {
            // List<Product> res = await _context.Products.FromSqlRaw("SELECT Id,Name,Alias,Price,Description,ImageUrl,Deleted,CreatedAt,UpdatedAt FROM Products").ToListAsync();
            List<ProductDTO> res = await _context.Database.SqlQueryRaw<ProductDTO>($@"SELECT Id,Name,Alias,Price FROM Products order by id desc offset 0 rows fetch next 10 rows only").ToListAsync();

            return Ok(res);
        }

        [HttpGet("GetAllProductsLinq")]
        public async Task<ActionResult> GetAllProductsLinq()
        {
            var res = await _context.Products.Skip(0).Take(10).Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Alias = p.Alias,
                Price = p.Price
            }).ToListAsync();
            return Ok(res);
        }
        [HttpGet("GetProductsPaging")]
        public async Task<ActionResult> GetProductsPaging([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var res = await _context.Products.Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Alias = p.Alias,
                Price = p.Price
            }).ToListAsync();
            return Ok(res);
        }
        [HttpGet("GetProductByIdRaw/{id}")]
        public async Task<ActionResult> GetProductByIdRaw([FromRoute] int id)
        {
            //Tạo ra biến sqlParameter để truyền tham số vào câu truy vấn raw sql type int value = id
            SqlParameter idPram = new SqlParameter("@id", id);
            var product = await _context.Database.SqlQueryRaw<ProductDTO>($"SELECT Id,Name,Alias,Price FROM Products WHERE Id = @id", idPram).SingleOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Alias = product.Alias,
                Price = product.Price
            };

            return Ok(productDto);
        }
        [HttpGet("GetProductByIdLinq/{id}")]
        public async Task<ActionResult> GetProductByIdLinq([FromRoute] int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Alias = product.Alias,
                Price = product.Price
            };

            return Ok(productDto);
        }

    }
}