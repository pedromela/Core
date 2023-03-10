using Rocs.Persistence.DAL;
using Rocs.Persistence.Repositories;

namespace Rocs.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RocsContext _context;

        public UnitOfWork(RocsContext context)
        {
            _context = context;
            Activities = new ActivityRepository(_context);
            Workers = new WorkerRepository(_context);
            Workers.Init();
            Save();
        }
        public IActivityRepository Activities { get; private set; }

        public IWorkerRepository Workers { get; private set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
