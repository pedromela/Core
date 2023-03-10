using System;
using Rocs.Persistence.Repositories;

namespace Rocs.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IActivityRepository Activities { get; }
        IWorkerRepository Workers { get; }
        int Save();
    }
}
