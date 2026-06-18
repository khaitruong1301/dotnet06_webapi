//api-controller-async
using Microsoft.AspNetCore.Cors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using backend_netcore_dotnet06.Helper;
using backend_netcore_dotnet06.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
//using backend_netcore_dotnet06.Models;

namespace backend_netcore_dotnet06.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductStoreContext _context;
        private readonly IMapper _map;
        private readonly IWebHostEnvironment _environment;

        //Read
        public ProductController(ProductStoreContext context, IMapper map, IWebHostEnvironment environment)
        {
            _context = context;
            _map = map;
            _environment = environment;

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
            var res = await _context.Products.Where(item => item.Deleted == false).Skip(0).Take(10).Select(p => new ProductDTO
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
            SqlParameter idPram = new SqlParameter("@id", System.Data.SqlDbType.Int) { Value = id };
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

        //Tìm kiếm product 
        [HttpGet("SearchProductsSQLRaw")]
        public async Task<ActionResult> SearchProductsSQLRaw([FromQuery] string keyword)
        {
            keyword = HelperFunction.StringToSlug(keyword);
            SqlParameter paramTuKhoa = new SqlParameter("@keyword", System.Data.SqlDbType.NVarChar, 50) { Value = $"%{keyword}%" };
            var res = await _context.Database.SqlQueryRaw<ProductDTO>($@"SELECT Id,Name,Alias,Price,Description FROM Products WHERE Alias LIKE @keyword OR Description LIKE @keyword", paramTuKhoa).ToListAsync();
            return Ok(res);
        }

        [HttpGet("SearchProductsLinq")]
        public async Task<ActionResult> SearchProductsLinq([FromQuery] string keyword)
        {
            keyword = HelperFunction.StringToSlug(keyword);
            var res = await _context.Products.Where(p => p.Alias.Contains(keyword) || p.Description.Contains(keyword)).Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Alias = p.Alias,
                Price = p.Price,
                Description = p.Description
            }).OrderBy(n => n.Id).Skip(0).Take(10).ToListAsync();
            return Ok(res);
        }


        [HttpPost("AddProductSqlRaw")]
        public async Task<ActionResult> AddProductSqlRaw([FromBody] ProductInsertDTO newProduct)
        {
            SqlParameter paramName = new SqlParameter("@name", System.Data.SqlDbType.NVarChar, 100) { Value = newProduct.Name };
            SqlParameter paramAlias = new SqlParameter("@alias", System.Data.SqlDbType.NVarChar, 100) { Value = HelperFunction.StringToSlug(newProduct.Name) };
            SqlParameter paramPrice = new SqlParameter("@price", System.Data.SqlDbType.Decimal) { Value = newProduct.Price };
            SqlParameter paramDescription = new SqlParameter("@description", System.Data.SqlDbType.NVarChar, 250) { Value = newProduct.Description ?? (object)DBNull.Value };
            SqlParameter paramImgUrl = new SqlParameter("@imgUrl", System.Data.SqlDbType.NVarChar, 250) { Value = newProduct.ImageUrl ?? (object)DBNull.Value };
            await _context.Database.ExecuteSqlRawAsync("INSERT INTO Products (Name, Alias, Price, Description, ImageUrl,Deleted) VALUES (@name, @alias, @price, @description, @imgUrl, 0)", paramName, paramAlias, paramPrice, paramDescription, paramImgUrl);
            return StatusCode(201, "Add product successfully!");
        }

        [EnableCors("AllowPost")]

        [HttpPost("AddProductLinq")]
        public async Task<ActionResult> AddProductLinq([FromBody] ProductInsertDTO newProduct) // dto hoặc viewmodel 
        {
            //Cách 1:Map tay từ product DTO sang product model
            // Product prodModel = new Product(); //entity hoặc gọi là model 
            // prodModel.Name = newProduct.Name;
            // prodModel.Price = newProduct.Price;
            // prodModel.Alias = HelperFunction.StringToSlug(newProduct.Name);
            // prodModel.ImageUrl = newProduct.ImageUrl;
            // prodModel.Description = newProduct.Description;
            // prodModel.Deleted = false;
            // prodModel.CreatedAt = DateTime.Now;
            // prodModel.UpdatedAt = DateTime.Now;
            //Cách 2: Map bằng automaper Dùng để map các thuộc tính tên giống nhau nhưng khác Class (lớp đối tượng)
            Product prodModel = _map.Map<Product>(newProduct);
            // prodModel.Deleted = false;
            // prodModel.CreatedAt = DateTime.Now;
            // prodModel.UpdatedAt = DateTime.Now;
            _context.Products.Add(prodModel);
            await _context.SaveChangesAsync();
            return StatusCode(201, "Add product successfully!");
        }

        [HttpPut("UpdateProductFromSQLRaw")]
        public async Task<IActionResult> UpdateProductFromSQLRaw(ProductUpdateDTO prodUpdate)
        {
            SqlParameter paramId = new SqlParameter("@id", System.Data.SqlDbType.Int) { Value = prodUpdate.Id };
            SqlParameter paramName = new SqlParameter("@name", System.Data.SqlDbType.NVarChar, 100) { Value = prodUpdate.Name };
            SqlParameter paramAlias = new SqlParameter("@alias", System.Data.SqlDbType.NVarChar, 100) { Value = HelperFunction.StringToSlug(prodUpdate.Name) };
            SqlParameter paramPrice = new SqlParameter("@price", System.Data.SqlDbType.Decimal) { Value = prodUpdate.Price };
            SqlParameter paramDescription = new SqlParameter("@description", System.Data.SqlDbType.NVarChar, 250) { Value = prodUpdate.Description ?? (object)DBNull.Value };
            SqlParameter paramImgUrl = new SqlParameter("@imgUrl", System.Data.SqlDbType.NVarChar, 250) { Value = prodUpdate.ImageUrl ?? (object)DBNull.Value };
            await _context.Database.ExecuteSqlRawAsync(@$"Update Products SET Name = @name, Alias=@alias, Price = @price, Description = @description, ImageUrl = @imgUrl where Id = @id", paramId, paramName, paramAlias, paramPrice, paramDescription, paramImgUrl);
            return StatusCode(200, "Update product successfully");
        }

        [HttpPut("UpdateProductFromLinq")]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductUpdateDTO prodUpdate)
        {
            //Lấy ra dòng dữ liệu từ trong CSDL map ra C# (đồng thời kiểm tra có trong csdl hay không)
            Product? prodModel = await _context.Products.SingleOrDefaultAsync(item => item.Id == prodUpdate.Id);
            if (prodModel == null)
            {
                return NotFound(@$"Không tìm thấy sản phẩm với id {prodUpdate.Id}");
            }
            //  prodModel = _map.Map<Product>(prodUpdate); dùng cho insert vì nó new 1 vùng nhớ mới cho Product
            // prodModel.Name = prodUpdate.Name;
            // prodModel.Price = prodUpdate.Price;
            // prodModel.Alias = HelperFunction.StringToSlug(prodUpdate.Name);
            // prodModel.ImageUrl = prodUpdate.ImageUrl;
            // prodModel.Description = prodUpdate.Description;
            // _context.Products.Update(prodModel);

            //Map dữ liệu DTO vào entity đang được tracked (tránh conflict tracking)
            _map.Map(prodUpdate, prodModel);

            _context.Products.Update(prodModel);


            await _context.SaveChangesAsync();//Lưu vào db ứng với CSDL là commit transactione

            return StatusCode(200, "Update product successfully");
        }

        [HttpDelete("DeleteFromLinQ")]
        public async Task<ActionResult> DeleteFromLinQ(int id)
        {
            //Kiểm tra sản phẩm có tồn tại trong db hay không
            Product? prd = await _context.Products.SingleOrDefaultAsync(item => item.Id == id);
            if (prd == null)
            {
                return NotFound("Không tìm thấy id");
            }

            _context.Products.Remove(prd);
            _context.SaveChanges();
            return StatusCode(200, "Xoá thành công");
        }

        [HttpDelete("DeletetProduct")]
        public async Task<ActionResult> DeletetProduct(int id)
        {
            //Kiểm tra sản phẩm có tồn tại trong db hay không
            Product? prd = await _context.Products.SingleOrDefaultAsync(item => item.Id == id);
            if (prd == null)
            {
                return NotFound("Không tìm thấy id");
            }
            prd.Deleted = true;
            _context.SaveChanges();
            return StatusCode(200, "Xoá thành công");
        }

        //Viết api upload file và lưu vào wwwroot


        [Authorize(Roles = "Admin,User")]
        [HttpPost("Uploadfile")]
        public async Task<ActionResult> UploadFile(IFormFile files)
        {
            //Lưu file vào folder mặc định server 

            if (files != null && files.Length > 0)
            {
                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                // Nếu chưa có folder thì tạo
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Lấy tên file an toàn
                var fileName = Path.GetFileName(files.FileName);

                // Tạo tên file mới để tránh trùng
                var newFileName = $"{Guid.NewGuid()}_{fileName}";

                var filePath = Path.Combine(uploadPath, newFileName);//   

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await files.CopyToAsync(stream);
                }

            }
            return Ok("ok");

        }

    }
    //Viết action xoá sản phẩm

}
