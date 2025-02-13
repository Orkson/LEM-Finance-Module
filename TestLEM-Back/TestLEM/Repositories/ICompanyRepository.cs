namespace TestLEM.Repositories
{
    public interface ICompanyRepository
    {
        bool ChcekIfComapnyAlreadyExistsInDatabase(string comapnyName);
    }
}
