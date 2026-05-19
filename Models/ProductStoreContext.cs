//ef-dbcontext

using Microsoft.EntityFrameworkCore;

namespace backend_netcore_dotnet06.Models {
    public class ProductStoreContext : DbContext {
        public ProductStoreContext() { }
        public ProductStoreContext(DbContextOptions<ProductStoreContext> options) : base (options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Lấy connection string từ appsettings.json
            optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=ProductStore;User Id=sa;Password=Cybersoft123@;TrustServerCertificate=True;");
        }
        //Khai báo bảng dạng class trên tầng code
        public DbSet<Product> Products { get; set; }
    }
}