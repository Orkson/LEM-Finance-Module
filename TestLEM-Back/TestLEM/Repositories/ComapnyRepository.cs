namespace TestLEM.Repositories
{
    public class ComapnyRepository : ICompanyRepository
    {
        private readonly LemDbContext dbContext;

        public ComapnyRepository(LemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public bool ChcekIfComapnyAlreadyExistsInDatabase(string comapnyName) => dbContext.Companies.Any(x => x.Name == comapnyName);
    }
}
