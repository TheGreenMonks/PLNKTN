using PLNKTNv2.Persistence.Repositories;

namespace PLNKTNv2.Persistence
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : class;
    }
}