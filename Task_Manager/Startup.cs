using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Task_Manager.DB;
using NLog;

namespace Task_Manager
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Register the Swagger generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
            });

            // Чтение строки подключения к базе данных из файла конфигурации
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
                });
            }

            // Загрузка конфигурации NLog из файла
            LogManager.LoadConfiguration("E:\\RestApi Task Maneger\\Task_Manager\\Task_Manager\\NLog\\nlog.config");

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
