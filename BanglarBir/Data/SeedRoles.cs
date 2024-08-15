using BanglarBir.Models;

namespace BanglarBir.Data
{
    public static class DbInitializer
    {
        public static void SeedRoles(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Volunteers.Any(v => v.Role == "Admin"))
            {
                context.Volunteers.Add(new Volunteer
                {
                    Name = "Admin",
                    EmailOrPhone = "admin@domain.com",
                    Password = "Admin@321", // You should hash passwords in production
                    Location = "HQ",
                    FbProfileUrl = "https://facebook.com/admin",
                    NIdOrStudentId = "AdminNID",
                    StudentIdOrNidPhoto = "path/to/admin/photo.jpg",
                    Role = "Admin"
                });

                context.SaveChanges();
            }
        }
    }
}
