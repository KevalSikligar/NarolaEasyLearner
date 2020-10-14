using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EasyLearnerAdmin.Data;
using EasyLearnerAdmin.Data.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using EasyLearner.Service.Interface;
using EasyLearner.Service.Implementation;
using AutoMapper;
using Twilio;

namespace EasyLearnerAdmin
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
        {
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddControllersWithViews();
            //services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Configuration);
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.ini
            services.AddIdentity<ApplicationUser, Role>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<RoleManager<Role>>();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ClaimsPrincipalFactory>();

            #region Identity Configuration
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;
            });

            //Seting the Account Login page
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Identity/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Identity/Account/Login
                options.LogoutPath = "/Identity/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });
            //Seting the Post Configure
            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
            {
                //configure your other properties
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/Identity/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Identity/Account/Login
                options.LogoutPath = "/Identity/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });
            services.Configure<CookieTempDataProviderOptions>(options =>
            {
                options.Cookie.IsEssential = true;
            });
            #endregion

            services.AddAuthentication
            (CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie();

            #region Dependency Injection
            #region L
            services.AddScoped<ILessonService, LessonRepository>();
            #endregion

            #region G
            services.AddScoped<IGradeService, GradeRepository>();
            #endregion

            #region I
            services.AddScoped<IInvitationCodeService, InvitationCodeRepository>();
            #endregion

            #region S
            services.AddScoped<ISettingService,SettingRepository>();
            services.AddScoped<IStaffService, StaffRepository>();
            services.AddScoped<IStaffAccessService, StaffAccessRepostitory>();
            services.AddScoped<ISubscriptionService, SubscriptionRepository>();
            services.AddScoped<ISubscriptionTypeService, SubscriptionTypeRepository>();
            #endregion

            #region M
            services.AddScoped<IMenuAccessService, MenuAccessRepository>();
            services.AddScoped<IMessageService, MessageRepository>();
            services.AddScoped<IMembershipService, MembershipRepository>();

            #endregion

            #region L
            services.AddScoped<ILessonService, LessonRepository>();
            #endregion

            #region S
            services.AddScoped<IStudentService, StudentRepository>();
            services.AddScoped<ISupportRequestService, SupportRequestRepository>();
            services.AddScoped<ISupportResponseService, SupportResponseRepository>();
            #endregion

            #region T
            services.AddScoped<ITutorService, TutorRepository>();
            services.AddScoped<ITutorRelevantLesson, TutorRelevantLessonRepository>();

            #endregion

            #region E
            services.AddScoped<IErrorLogService, ErrorLogRepository>();
            services.AddScoped<IExamService, ExamRepository>();
            services.AddScoped<IExamsQuestionService, ExamsQuestionRepository>();
            services.AddScoped<IExamLessonService, ExamsLessonsRepository>();
            #endregion

            #region U
            services.AddScoped<IUserService, UserRepository>();
            #endregion

            #region F
            services.AddScoped<IFriendsService,FriendsRepository>();
            #endregion

            #region P
            services.AddScoped<IPaymentService, PaymentRepository>();
            #endregion

            #region Q
            services.AddScoped<IQuestionRequestService, QuestionRequestRepository>();
            services.AddScoped<IQuestionResponseService, QuestionResponseRepository>();

            #endregion

            #region L
            services.AddScoped<ILogService, LogRepository>();
            #endregion

            #endregion






            services.AddMvc();
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            /**Add Automapper**/
            services.AddAutoMapper(typeof(Startup));
            services.AddSession(opts =>
            {
                opts.Cookie.IsEssential = true; // make the session cookie Essential
            });

            //set login as default page
            services.AddMvc()
             .AddRazorPagesOptions(options =>
            {
                options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddSessionStateTempDataProvider();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddSession();

            #region Configure App Settings

            /**Email Settings**/
            // services.Configure<EmailSettingsGmail>(Configuration.GetSection("EmailSettingsGmail"));

            /**Settings**/
            #endregion

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<Role> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            EasyLearnerIdentityDataInitializer.SeedData(userManager, roleManager);
        }
    }
}
