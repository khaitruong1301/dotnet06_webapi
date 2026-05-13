//Tạo controller cho UserDTO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_netcore_dotnet06.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace backend_netcore_dotnet06.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public static List<UserDTO> lstUserDTO = new List<UserDTO>()
        {
            new UserDTO() { Id = 1, Email = "user1@example.com", Name = "User 1", Password = "password1" },
            new UserDTO() { Id = 2, Email = "user2@example.com", Name = "User 2", Password = "password2" }
        };
        public UserController()
        {
        }

        [HttpGet("GetAllUserDTO")]
        public List<UserDTO> GetAllUserDTO()
        {
            return lstUserDTO;
        }
        [HttpGet("GetUserById/{id}")]
        public UserDTO? GetUserById([FromRoute]int id)
        {
            return lstUserDTO.SingleOrDefault(u => u.Id == id);
        }
        [HttpPost("AddUser")]
        public List<UserDTO> AddUser([FromBody]UserDTO user)
        {
            //Kiểm tra trung email hoặc id
            if (lstUserDTO.Any(u => u.Email == user.Email || u.Id == user.Id))
            {
                //Nếu có trùng thì trả về lỗi
                throw new Exception("Email hoặc Id đã tồn tại");
            }
            lstUserDTO.Add(user);
            return lstUserDTO;
        }
        [HttpDelete("DeleteUser/{id}")]
        public List<UserDTO> DeleteUser([FromRoute]int id)
        {
            var user = lstUserDTO.SingleOrDefault(u => u.Id == id);
            if (user != null)
            {
                lstUserDTO.Remove(user);
            }
            return lstUserDTO;
        }
        [HttpPut("UpdateUser/{id}")]
        public List<UserDTO> UpdateUser([FromRoute]int id, [FromBody]UserDTO updatedUser)
        {
            var user = lstUserDTO.SingleOrDefault(u => u.Id == id);
            if (user != null)
            {
                //Cập nhật thông tin user
                user.Email = updatedUser.Email;
                user.Name = updatedUser.Name;
                user.Password = updatedUser.Password;
            }
            return lstUserDTO;
        }
        [HttpGet("SearchUserByName")]
        public List<UserDTO> SearchUserByName([FromQuery]string name)
        {
            return lstUserDTO.Where(u => u.Name.Contains(name)).ToList();
        }
    }
}