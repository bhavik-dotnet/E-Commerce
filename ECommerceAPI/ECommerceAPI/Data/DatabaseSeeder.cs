using ECommerceAPI.Data;

public static class DatabaseSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Users.Any())
        {
            var users = context.Users.ToList();
            foreach (var user in users)
            {
                if (user.CreatedDate == new DateTime(2025, 01, 01))
                {
                    user.CreatedDate = DateTime.UtcNow; // dynamic runtime value
                }
            }

            var categories = context.Categories.ToList();
            foreach (var category in categories)
            {
                if (category.CreatedDate == new DateTime(2025, 01, 01))
                {
                    category.CreatedDate = DateTime.UtcNow; // dynamic runtime value
                }
            }

            context.SaveChanges();
        }
    }
}
