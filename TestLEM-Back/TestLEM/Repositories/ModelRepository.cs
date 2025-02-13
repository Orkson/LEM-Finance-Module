namespace TestLEM.Repositories
{
    public class ModelRepository : IModelRepository
    {
        private readonly LemDbContext dbContext;

        public ModelRepository(LemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public bool ChcekIfModelAlreadyExistsInDatabase(string name, string serialNumber) => dbContext.Models.Any(x => x.SerialNumber == serialNumber || x.Name == name);
        public int GetModelId(string modelName) => dbContext.Models.First(x => x.Name == modelName).Id;
    }
}
