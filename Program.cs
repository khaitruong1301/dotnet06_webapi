using backend_netcore_dotnet06.Helper;
using backend_netcore_dotnet06.Models;
using Microsoft.OpenApi;
using AutoMapper;
var builder = WebApplication.CreateBuilder(args);


//DI DbContext (EF - entity framework)
builder.Services.AddDbContext<ProductStoreContext>();



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
});
//DI Automapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());



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

app.Run();





