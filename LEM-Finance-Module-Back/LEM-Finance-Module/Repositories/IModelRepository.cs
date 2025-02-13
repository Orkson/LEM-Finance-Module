namespace TestLEM.Repositories
{
    public interface IModelRepository
    {
        bool ChcekIfModelAlreadyExistsInDatabase(string name, string serialNumber);
        int GetModelId(string modelName);
    }
}
