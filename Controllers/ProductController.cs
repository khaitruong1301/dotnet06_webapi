//api-controller-async
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_netcore_dotnet06.Helper;
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
            new ProductDTO() { Id = 1, Name = "Điện thoại iphone 17 pro max", Price = 10000 },
            new ProductDTO() { Id = 2, Name = "Samsung galaxy Note 25 ultra ", Price = 20000 },
            new ProductDTO() { Id = 3, Name = "Xiaomi mi 17 pro max", Price = 15000 }
        };
        
        static ProductController()
        {
            foreach (var item in lstProductDTO)
            {
                item.Alias = HelperFunction.StringToSlug(item.Name);
            }
        }

        public ProductController()
        {
         


        }

        [HttpGet("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct()
        {
            return Ok(lstProduct);
        }

        // [HttpGet("GetAllProductDTO")]
        // public async Task<IActionResult> GetAllProductDTO()
        // {
        //     var response = new ResponseTypeDTO<List<ProductDTO>>()
        //     {
        //         StatusCode = 200,
        //         Message = "Lấy danh sách sản phẩm thành công",
        //         Content = lstProductDTO,
        //         DateTime = DateTime.Now
        //     };
        //     return Ok(response);
        // }

        [HttpGet("GetAllProductDTO")]
        public async Task<IActionResult> GetAllProductDTO()
        {
            var response = new ResponseTypeDTO<List<ProductDTO>>()
            {
                StatusCode = 200,
                Message = "Lấy danh sách sản phẩm thành công",
                Content = lstProductDTO,
                DateTime = DateTime.Now
            };
            return StatusCode(StatusCodes.Status200OK, response);
        }



        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            //Kiểm tra xem id có tồn tại trong list hay không, nếu có thì trả về phần tử đó nếu không có trả lỗi 400
            ProductDTO? prodDetail = await Task.FromResult(lstProductDTO.SingleOrDefault(item => item.Id == id));
            var response = new ResponseTypeDTO<dynamic>()
            {
                StatusCode = 400,
                Message = "Không tìm thấy sản phẩm với id: " + id,
                Content = null,
                DateTime = DateTime.Now
            };
            if (prodDetail == null)
            {
                response.Content = "Không tìm thấy sản phẩm với id: " + id;

                return StatusCode(StatusCodes.Status400BadRequest, response); //status code 400
            }
            else
            {
                response.Content = prodDetail;
                return StatusCode(StatusCodes.Status200OK, response); //status code 200
            }

        }



        [HttpPost("AddProduct")] //FormBody là người dùng nhập liệu từ form 
        public async Task<IActionResult> AddProduct([FromBody] ProductDTO product)
        {
            //Kiểm tra id có tồn tại trong list chưa ? nếu tồn tại thì trả về 400 nếu chưa tồn tại thì thêm vào list và trả về 200
            var existingProduct = lstProductDTO.SingleOrDefault(p => p.Id == product.Id);

            if (existingProduct != null)
            {
                var responseData = new ResponseTypeDTO<string>()
                {
                    StatusCode = 400,
                    Message = "Sản phẩm với id: " + product.Id + " đã tồn tại",
                    Content = null,
                    DateTime = DateTime.Now
                };
                return StatusCode(StatusCodes.Status400BadRequest, responseData);
            }
            product.Alias = HelperFunction.StringToSlug(product.Name);
    
            lstProductDTO.Add(product);
            var response = new ResponseTypeDTO<List<ProductDTO>>()
            {
                StatusCode = 200,
                Message = "Thêm sản phẩm thành công",
                Content = lstProductDTO,
                DateTime = DateTime.Now
            };
            return StatusCode(StatusCodes.Status200OK, response);
        }



        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            //Lấy ra phần tử trong list từ id
            var productToRemove = lstProductDTO.FirstOrDefault(p => p.Id == id);
            //Kiểm tra nếu có phần tử đó thì xoá
            if (productToRemove != null)
            {
                lstProductDTO.Remove(productToRemove);
                var response = new ResponseTypeDTO<List<ProductDTO>>()
                {
                    StatusCode = 200,
                    Message = "Xoá sản phẩm thành công",
                    Content = lstProductDTO,
                    DateTime = DateTime.Now
                };
                return StatusCode(StatusCodes.Status200OK, response);
            }
            var responseData = new ResponseTypeDTO<string>()
            {
                StatusCode = 404,
                Message = "Không tìm thấy sản phẩm với id: " + id,
                Content = null,
                DateTime = DateTime.Now
            };
            return StatusCode(StatusCodes.Status404NotFound, responseData);
        }


        //nguyễn VăN A => nguyen-van-a
        //list : nguyễn văn a => nguyen-van-a => alias: nguyen-van-a

        [HttpGet("SearchProductByName")]
        public async Task<IActionResult> SearchProductByName([FromQuery] string keyword) //ĐIỆN THOẠI -> dien-thoai
        {
            string name = HelperFunction.StringToSlug(keyword);
            var lstSearchProduct = lstProductDTO.Where(p => p.Alias.Contains(name)).ToList();
            if (lstSearchProduct.Count == 0)
            {
                var responseData = new ResponseTypeDTO<string>()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy sản phẩm với tên: " + keyword,
                    Content = null,
                    DateTime = DateTime.Now
                };
                return StatusCode(StatusCodes.Status404NotFound, responseData);
            }
            else
            {
                var response = new ResponseTypeDTO<List<ProductDTO>>()
                {
                    StatusCode = 200,
                    Message = @$"Tìm thấy {lstSearchProduct.Count} sản phẩm với tên: {keyword}",
                    Content = lstSearchProduct,
                    DateTime = DateTime.Now
                };
                return StatusCode(StatusCodes.Status200OK, response);
            } 
            
        }

        [HttpPut("UpdateProduct")]
        public async Task<ActionResult> UpdateProduct([FromBody] ProductDTO updatedProduct)
        {
            var productToUpdate = lstProductDTO.SingleOrDefault(p => p.Id == updatedProduct.Id);
            if (productToUpdate == null)
            {
                var responseData = new ResponseTypeDTO<string>()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy sản phẩm với id: " + updatedProduct.Id,
                    Content = null,
                    DateTime = DateTime.Now
                };
                return StatusCode(StatusCodes.Status404NotFound, responseData);
            }else
            {
                productToUpdate.Name = updatedProduct.Name;
                productToUpdate.Price = updatedProduct.Price;
                productToUpdate.Alias = HelperFunction.StringToSlug(updatedProduct.Name);
                var response = new ResponseTypeDTO<List<ProductDTO>>()
                {
                    StatusCode = 200,
                    Message = "Cập nhật sản phẩm thành công",
                    Content = lstProductDTO,
                    DateTime = DateTime.Now
                };
                return StatusCode(StatusCodes.Status200OK, response);
            }
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