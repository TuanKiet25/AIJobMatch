using AIJobMatch.Application;
using AIJobMatch.Application.IRepositories;
using AIJobMatch.Application.IServices;
using AIJobMatch.Application.Services;
using AIJobMatch.Application.ViewModels.Requests;
using AIJobMatch.Domain.Entities;
using AIJobMatch.Infrastructure.Filter;
using AIJobMatch.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIJobMatch.Infrastructure
{
    public static class DI
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            // Đăng ký AppDbContext
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });

            services.AddScoped<PremiumCheckFilter>();
            // Đăng ký repositiries
            #region Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISubscriptionPlansRepository, SubscriptionPlansRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<ICandidateProfileRepository, CandidateProfileRepository>();
            services.AddScoped<IEducationRepository, EducationRepository>();
            services.AddScoped<IWorkExperienceRepository, WorkExperienceRepository>();
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            #endregion
            // Đăng ký services
            #region services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
            services.AddHttpClient<ITurnstileService, TurnstileService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IJobPostingService, JobPostingService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICVService, CVService>();
            services.AddScoped<IAiCvService, AiCvService>();
            services.AddScoped<IValidateService, ValidateService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<IJobApplicationService, JobApplicationService>();
            #endregion
            //Đăng ký auto mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            // Đăng ký JWT authentication
            //var jwtSettings = new JwtSettings();
            //configuration.Bind(JwtSettings.SectionName, jwtSettings);
            //services.AddSingleton(jwtSettings); // Dùng AddSingleton vì cài đặt không thay đổi
            // Cấu hình dịch vụ Authentication của .NET Core

            // Đăng ký MailSettings
            //services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            // Đăng ký CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                                  policy =>
                                  {
                                      // Cho phép origin của frontend được truy cập
                                      policy.AllowAnyOrigin()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });
            //đăng ký HttpContextAccessor
            services.AddHttpContextAccessor();
            return services;
        }
    }
}
