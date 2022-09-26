namespace RPMFuelService.Data
{
    public static class DbInitializer
    {
        public static void Initialize(FuelDbContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
