//Tạo controller cho UserDTO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend_netcore_dotnet06.Helper;
using backend_netcore_dotnet06.Models.DBUser;
using backend_netcore_dotnet06.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_netcore_dotnet06.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDBContext _context;
        private readonly JwtAuthService _jwt;

        public UserController(UserDBContext context,JwtAuthService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO model)
        {
            //Kiểm tra username và email có tồn tại hay không
            var user = _context.Users.SingleOrDefault(item => item.Username == model.Username || item.Email == model.Email);
            if (user != null)
            {

                var res = new
                {
                    message = "Tài khoản hoặc email đã được đăng ký !",
                    satatus = 400
                };
                return StatusCode(400, res);
            }
            //Mặc định có 1 role -> User
            //Lưu ý: 1 nghiệp vụ chỉ savechange 1 lần
            //UserInsert:
            User newUser = new User();
            newUser.Email = model.Email;
            newUser.Phone = model.Phone;
            newUser.Deleted = false;
            newUser.Username = model.Username;
            newUser.Fullname = model.FullName;
            newUser.HashPassword = HelperFunction.HashPassword(model.Password);
            UserRole usRole = new UserRole();
            usRole.IdUser = newUser.Id;
            usRole.IdRole = CRole.User;
            usRole.Desc = $@"Set vai trò thông qua view đăng ký !";
            newUser.UserRoles.Add(usRole);
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            // try
            // {
            //     _context.Database.BeginTransaction();


            //     _context.Database.CommitTransaction();
            // }
            // catch (Exception ex)
            // {
            //     _context.Database.RollbackTransaction();
            // }
            return Ok("Đăng ký thành công !");

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLogin)
        {
            //Check login -> thành công tạo token
            User? usCheckLogin = _context.Users.SingleOrDefault(item => item.Username == userLogin.UserNameOrEmailOrPhone || item.Email == userLogin.UserNameOrEmailOrPhone || item.Phone == userLogin.UserNameOrEmailOrPhone);
            if (usCheckLogin == null)
            {
                return BadRequest("Không tìm thấy tài khoản !");
            }
            //check pass với pass hash
            if (!HelperFunction.VerifyPassword(userLogin.Password, usCheckLogin.HashPassword))
            {
                return BadRequest("Sai mật khẩu!");
            }

            //Nếu đúng thì tạo accessToken
            string token = _jwt.GenerateToken(userLogin);

            return Ok(token);


        }

        [HttpGet("GetUserInfo")]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            //Decode token (HttpContext)
            bool valid = HttpContext.Request.Headers.TryGetValue("Authorization", out var token);
            if (valid)
            {
                string tokenValue = token.ToString().Replace("Bearer ", "");
                string username = _jwt.DecodePayloadToken(tokenValue);
                //truy vấn vào bảng user để tra profile 
                var userResponse = _context.Users.SingleOrDefault(item => item.Username == username);
                if (userResponse != null)
                {
                    return StatusCode(200, new
                    {
                        message = "Lấy thông tin user thành công !",
                        data = new
                        {
                            username = userResponse.Username,
                            email = userResponse.Email,
                            phone = userResponse.Phone,
                            fullname = userResponse.Fullname
                        }
                    });
                }
                return StatusCode(404, new
                {
                    message = "Không tìm thấy thông tin user !"
                });

            }
            
            return Ok();
        }


    }
}