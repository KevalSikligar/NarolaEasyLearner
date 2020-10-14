using System;
using System.Collections.Generic;
using System.Text;
using EasyLearnerAdmin.Data.DbContext;
using EasyLearnerAdmin.Data.DbModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyLearnerAdmin.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
    {

        #region Db Sets
        DbSet<Grades> Grades { get; set; }
        DbSet<Lessons> Lessons { get; set; }
        DbSet<Students> Students { get; set; }
        DbSet<Tutors> Tutors { get; set; }
        DbSet<Exams> Exams { get; set; }
        DbSet<AttemptedExams> AttemptedExams { get; set; }
        DbSet<ExamsQuestions> ExamsQuestions { get; set; }
        DbSet<ExamLessons> ExamsLessons { get; set; }
        DbSet<Settings> Settings { get; set; }
        DbSet<SupportRequest> SupportRequests { get; set; }
        DbSet<SupportResponse> SupportResponses { get; set; }
        DbSet<QuestionRequest> QuestionRequests { get; set; }
        DbSet<QuestionResponse> QuestionResponses { get; set; }
        DbSet<SubscriptionType> SubscriptionTypes { get; set; }
        //DbSet<Subscription> Subscription { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<Friends> Friends { get; set; }
        DbSet<ErrorLog> ErrorLogs { get; set; }
        DbSet<Staff> Staffs { get; set; }
        DbSet<MenuAccess> MenuAccesses { get; set; }
        DbSet<StaffAccess> StaffAccesss { get; set; }
        DbSet<TutorRelevantLesson> TutorRelevantLessons { get; set; }
        DbSet<Payments> Payments { get; set; }

        DbSet<Membership> Memberships { get; set; }
        DbSet<Log> logs { get; set; }
        DbSet<InvitationCode> InvitationCodes { get; set; }

        #endregion


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            // Change Default filed datatype & length
            modelBuilder.Entity<ApplicationUser>().Property(c => c.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Role>().Property(c => c.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<UserClaim>().Property(x => x.ClaimType).HasMaxLength(50);
            modelBuilder.Entity<UserClaim>().Property(x => x.ClaimValue).HasMaxLength(200);

            modelBuilder.Entity<ApplicationUser>().Property(x => x.Email).HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.UserName).HasMaxLength(100);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.PhoneNumber).HasMaxLength(12);

          

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLazyLoadingProxies();
            base.OnConfiguring(optionsBuilder);
        }

    }
}
