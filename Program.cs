using backend_netcore_dotnet06.Helper;
using backend_netcore_dotnet06.Models.DBQuanLyNhanVien;
using backend_netcore_dotnet06.Models;
using Microsoft.OpenApi;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using backend_netcore_dotnet06.Models.DBUser;
using backend_netcore_dotnet06.Middleware;
// using Microsoft.EntityFrameworkCore.Proxies;
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "FileServer"
});


//DI DbContext (EF - entity framework)
builder.Services.AddDbContext<ProductStoreContext>();

//DI QuanLyNhanVienContext
string? connectionStringQLNV = builder.Configuration.GetConnectionString("DBQuanLyNhanVienConnection");

//Layzy proxy
builder.Services.AddDbContext<QuanLyNhanVienContext>(options =>
{

    options.UseSqlServer(connectionStringQLNV);
    //cấu hình proxies
    // options.UseLazyLoadingProxies(true);
});

//DI cho userdb context
string? connectionStringUser = builder.Configuration.GetConnectionString("DBUser");
builder.Services.AddDbContext<UserDBContext>(options =>
{
    options.UseSqlServer(connectionStringUser);
});





//DI controller có [Route]
builder.Services.AddControllers();
//DI Swagger 

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "API documentation for .NET 10"
    });
    // Khai báo scheme Bearer -> tạo nút "Authorize" + ô nhập token trong Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token JWT vào ô dưới đây"
    });

    // Áp scheme cho toàn bộ endpoint -> hiện icon ổ khóa và tự gắn header Authorization khi gọi API
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});
//DI Automapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

//DI CORS cấp quyền GET cho domain 5000 và 5001
builder.Services.AddCors(options =>
{

    options.AddPolicy("AllowPost", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5501")
                .AllowAnyMethod()
                .AllowAnyHeader();

    });

    options.AddPolicy("AllowGETData", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500").WithMethods("GET");
    });
    

});

//DI authentication - authorization = jwt
var key = builder.Configuration["Jwt:Key"];           // Khóa bí mật để ký token
var issuer = builder.Configuration["Jwt:Issuer"];     // Issuer (bên phát hành token)
var audience = builder.Configuration["Jwt:Audience"]; // Audience (người nhận token)
// 2. Cấu hình Authentication sử dụng JWT Bearer
builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuerSigningKey = true, // Xác thực key bí mật của token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = true,// Xác thực Issuer 
        ValidIssuer = issuer, // Phải khớp với Issuer trong token
        ValidateAudience = true,    // Xác thực Audience
        ValidAudience = audience, // Phải khớp với Audience trong token
        ValidateLifetime = true, // Xác thực thời gian hết hạn của token
        ClockSkew = TimeSpan.Zero, // Bỏ qua độ trễ thời gian giữa server và client (ngăn lỗi thời gian)
        RoleClaimType = ClaimTypes.Role, // Ánh xạ claim role
        NameClaimType = "UserName",
    };
});



//DI jwt service
builder.Services.AddScoped<JwtAuthService>();


//DI custom middleware CountIpRequestMiddleware

builder.Services.AddTransient<CountIpRequestMiddleware>();

builder.Services.AddTransient<SubDomainMiddleware>();

var app = builder.Build();


//Nếu là localhost (môi trường dev mới có trang swagger)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
        options.RoutePrefix = "swagger";
    });
}

//Sử dụng middleware controller 
app.MapControllers();
//Xử lý cors cho domain AllowGETData
app.UseCors("AllowGETData");
app.UseCors("AllowPost");
//Xử lý ngoại lệ tập trung
// app.UseExceptionHandler(errorApp =>
// {
//     errorApp.Run(async context =>
//     {
//         // var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
//         // var exception = exceptionHandlerPathFeature?.Error;

//         // context.Response.ContentType = "application/json";
//         // context.Response.StatusCode = exception is not null ? 500 : 200;

//         //Lấy thông tin ip của client
//         var request = context.Request;
//         var ipAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString();
//         var userAgent = request.Headers["User-Agent"].ToString();
//         Console.WriteLine($"IP Address: {JsonSerializer.Serialize(ipAddress)}");
//         context.Response.ContentType = "application/json";  
//         var result = new
//         {
//             IsSuccess = false,
//             Message = "Chẳng có lỗi xảy ra",
//             IP = ipAddress,
//             UserAgent = userAgent
//         };
//         await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(result));
//     });
// });

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "FileServer")
    ),
    RequestPath = "/files"
});
//Sử dụng middleware tự viết CountIpRequestMiddleware để đếm số lượng request theo ip
app.UseMiddleware<CountIpRequestMiddleware>(); //Đếm số lượng request theo ip
app.UseMiddleware<SubDomainMiddleware>(); //Xử lý tên miền phụ


app.UseAuthentication(); //Xác thực (đăng nhập)
app.UseAuthorization(); //Phân quyền





app.Run();












