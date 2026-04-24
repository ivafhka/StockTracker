using System;
using System.Collections.Generic;
using System.Text;

namespace StockTracker.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangeAsync(CancellationToken cancellationToken = default);
    }
}
