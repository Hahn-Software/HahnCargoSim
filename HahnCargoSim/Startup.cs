using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HahnCargoSim.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HahnCargoSim.Services.Interfaces;
using System.Threading.Tasks;

namespace HahnCargoSim
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
      services.Configure<SimConfig>(Configuration.GetSection("SimConfig"));

      services.AddAuthentication(x =>
      {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(x =>
      {
        x.Events = new JwtBearerEvents
        {
          OnTokenValidated = context =>
          {
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
            if (context.Principal?.Identity?.Name == null)
            {
              context.Fail("Unauthorized");
            }
            else 
            {
              var user = userService.GetByUserName(context.Principal.Identity.Name);
              if (user == null)
              {
                context.Fail("Unauthorized");
              }
            }

            return Task.CompletedTask;
          }
        };
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("SimConfig").Get<SimConfig>().TokenSecret)),
          ValidateIssuer = false,
          ValidateAudience = false
        };
      });

      services.AddSingleton<ILoggerService, LoggerService>();
      services.AddSingleton<IUserService, UserService>();
      services.AddSingleton<IGridService, GridService>();
      services.AddSingleton<ICargoTransporterService, CargoTransporterService>();
      services.AddSingleton<IOrderService, OrderService>();
      services.AddSingleton<ISimService, SimService>();
      services.AddHostedService<SimServer>();

      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen();
      services.AddControllersWithViews();

      services.AddCors();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();
      app.UseCors(builder =>
      {
        builder
          .AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
      });

      
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller}/{action=Index}/{id?}");
      });


    }


  }
}
