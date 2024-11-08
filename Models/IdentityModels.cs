using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ChurchSystem.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string NameSurname { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public int Age { get; set; }

        public string number { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }




        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<ChurchFunds> ChurchFunds { get; set; }
        public virtual DbSet<Donations> Donations { get; set; }  
        public virtual DbSet<ChurchLeaders> ChurchLeaders { get; set; } 
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<ItemDonations> ItemDonations { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<basket> Basket { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OnlineSermons> OnlineSermons { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<EventProgram> EventsProgram { get; set; }
        public virtual DbSet<Attendance> Attendance { get; set; }      
        public virtual DbSet<ChurchVenue> ChurchVenues { get; set; }
        public virtual DbSet<ChurchVenueBooking> ChurchVenueBookings { get; set; }
        public virtual DbSet<EventDecorLayOuts > EventDecorLayOuts { get; set; }
        public virtual DbSet<VenueBookingEvent> VenueBookingEvent { get;set; }
        public virtual DbSet<VenueImages> VenueImages { get; set;}
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseMaterial> CoursesMaterial { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentCourse> StudentCourse { get; set;}
        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<Quiz> Quizzes { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<StudentQuizResult> StudentQuizResult { get; set;}
        public virtual DbSet<QuizAttempt> QuizAttempt { get; set; }
        public virtual DbSet<VenueChecklist> VenueChecklist { get;set; }
        public virtual DbSet<BibleStudyClasses> BibleStudyClasses { get; set; }
        public virtual DbSet<images> images { get; set; }
        public virtual DbSet<classAttendance> classAttendance { get; set; }
        public virtual DbSet<Books> books { get; set; }
        public virtual DbSet<BookWishList> bookWishList { get; set;}
        public virtual DbSet<BookLoan> bookLoan { get; set; }   
        public virtual DbSet<EventSurvey> EventSurveys { get; set; }

        }

    }

    
