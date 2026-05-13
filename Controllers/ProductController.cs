//api-controller-async
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using backend_netcore_dotnet06.Models;

namespace backend_netcore_dotnet06.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public static List<string> lstProduct = new List<string>() { "Product 1", "product 2", "product 3" };
        public static List<ProductDTO> lstProductDTO = new List<ProductDTO>()
        {
            new ProductDTO() { Id = 1, Name = "Product 1", Price = 10000 },
            new ProductDTO() { Id = 2, Name = "Product 2", Price = 20000 },
            new ProductDTO() { Id = 3, Name = "Product 3", Price = 15000 }
        };
        public ProductController()
        {
        }

        [HttpGet("GetAllProduct")]
        public List<string> GetAllProduct()
        {
            return lstProduct;
        }

        [HttpGet("GetAllProductDTO")]
        public List<ProductDTO> GetAllProductDTO()
        {
            return lstProductDTO;
        }

        [HttpGet("GetProductById/{id}")]
        public ProductDTO? GetProductById([FromRoute] int id)
        {
            return lstProductDTO.FirstOrDefault(p => p.Id == id);
        }

        [HttpPost("AddProduct")] //FormBody là người dùng nhập liệu từ form 
        public List<ProductDTO> AddProduct([FromBody] ProductDTO product)
        {
            lstProductDTO.Add(product);
            return lstProductDTO;
        }

        [HttpDelete("DeleteProduct/{id}")]
        public List<ProductDTO> DeleteProduct([FromRoute] int id)
        {
            //Lấy ra phần tử trong list từ id
            var productToRemove = lstProductDTO.FirstOrDefault(p => p.Id == id);
            //Kiểm tra nếu có phần tử đó thì xoá
            if (productToRemove != null)
            {
                lstProductDTO.Remove(productToRemove);
            }

            return lstProductDTO;
        }

        [HttpGet("SearchProductByName")]
        public List<ProductDTO> SearchProductByName([FromQuery] string name)
        {
            return lstProductDTO.Where(p => p.Name.Contains(name)).ToList();
        }

        [HttpPut("UpdateProduct")]
        public List<ProductDTO> UpdateProduct([FromBody] ProductDTO updatedProduct)
        {
            var productToUpdate = lstProductDTO.SingleOrDefault(p => p.Id == updatedProduct.Id);
            if (productToUpdate != null)
            {
                productToUpdate.Name = updatedProduct.Name;
                productToUpdate.Price = updatedProduct.Price;
            }
            return lstProductDTO;
        }

        [HttpPatch("DiscountProduct/{id}")]
        public List<ProductDTO> DiscountProduct([FromRoute] int id, [FromBody] decimal discountPercentage)
        {
            //TÌm trong list có sản phẩm đó hay không
            var productToDiscount = lstProductDTO.SingleOrDefault(p => p.Id == id);
            if (productToDiscount != null)
            {                    //Tính giá sau khi giảm giá
                var discountedPrice = productToDiscount.Price * (1 - discountPercentage / 100);
                productToDiscount.Price = discountedPrice;
            }
            return lstProductDTO;

        }




    }
}