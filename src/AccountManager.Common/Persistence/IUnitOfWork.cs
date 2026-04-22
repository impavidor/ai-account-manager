namespace AccountManager.Common.Persistence;

public interface IUnitOfWork
{
    Task SaveChanges();
}
