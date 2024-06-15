using BackEnd.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BackEnd
{
	public class Program
	{
		public static void Main(string[] args)
		{
			WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

			builder.Services.AddDbContext<StreamWizardDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DbKey"));
			});

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = "https://localhost:7116",
					ValidAudience = "https://localhost:7116",
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"]))
				};
			});

			builder.Services.AddAuthorizationBuilder()
				.SetDefaultPolicy(new AuthorizationPolicyBuilder()
						.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
						.RequireAuthenticatedUser()
						.Build());

			string corsPolicy = "corsPolicy";
			builder.Services.AddCors(options =>
			{
				options.AddPolicy(name: corsPolicy,
				policy =>
				{
					policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
				});
			});

			builder.Services.AddControllers();

			var app = builder.Build();

			app.UseHttpsRedirection();
			app.UseCors(corsPolicy);
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
