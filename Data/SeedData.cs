public static class SeedData
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Users.Any())
        {
            context.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = "1234",
                Role = "Admin"
            });

            context.SaveChanges();
        }
    }
}