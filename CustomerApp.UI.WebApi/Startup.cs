using CustomerApp.Core.ApplicationService;
using CustomerApp.Core.ApplicationService.Services;
using CustomerApp.Core.DomainService;
using CustomerApp.Infrastructure.DBInitialization;
using CustomerApp.Infrastructure.SQL;
using CustomerApp.Infrastructure.SQL.Repositories;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CustomerApp.UI.WebApi
{
    public class Startup
    {
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {   /*var loggerFactory = LoggerFactory.Create(builder => {
                    builder.AddConsole();
                }
            );
            services.AddDbContext<CustomerAppLiteContext>(
                opt =>
                {
                    opt.UseLoggerFactory(loggerFactory)
                        .AddInterceptors(new DBInterceptor())
                        .UseSqlite("Data Source=customerApp.db"); 

                });
            */
            var loggerFactory = LoggerFactory.Create(builder => {
                    builder.AddConsole();
                }
            );
            // If Dev Do this
            services.AddDbContext<CustomerAppDBContext>(
                opt =>
                {
                    opt
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                        .UseLoggerFactory(loggerFactory)
                        .UseSqlite("Data Source=custapp.db");
                }, ServiceLifetime.Transient);
            
            services.AddScoped<IHatRepository, HatSQLRepository>();
            services.AddScoped<IAddressRepository, AddressSQLRepository>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ICityRepository, CitySQLRepository>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<ICustomerRepository, CustomerSQLRepository>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddControllers().AddNewtonsoftJson(o => 
            {
                o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                // o.SerializerSettings.MaxDepth = 1;
            });
            
            services.AddCors(options =>
            {
                options.AddPolicy(name: "CustomerAppAllowSpecificOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddAuthentication(
                    CertificateAuthenticationDefaults.AuthenticationScheme)
                .AddCertificate();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var ctx = scope.ServiceProvider.GetService<CustomerAppDBContext>();
                    ctx.Database.EnsureDeleted();
                    ctx.Database.EnsureCreated();
                    var hatRepository = scope.ServiceProvider.GetService<IHatRepository>();
                    var custRepository = scope.ServiceProvider.GetService<ICustomerRepository>();
                    var addressRepository = scope.ServiceProvider.GetService<IAddressRepository>();
                    var cityRepository = scope.ServiceProvider.GetService<ICityRepository>();
                    new DBInitializer(hatRepository, cityRepository, custRepository, addressRepository).InitData();
                    /*var ctx = scope.ServiceProvider.GetService<CustomerAppLiteContext>(); 
                    ctx.Database.EnsureDeleted();
                    ctx.Database.EnsureCreated();
                    var custRepository = scope.ServiceProvider.GetService<ICustomerRepository>();
                    var addressRepository = scope.ServiceProvider.GetService<IAddressRepository>();
                    var cityRepository = scope.ServiceProvider.GetService<ICityRepository>();
                    new DBInitializer(cityRepository, custRepository, addressRepository).InitData();*/
                }
            }

            
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("CustomerAppAllowSpecificOrigins");
            // localhost/pets
            // localhost/customers
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
