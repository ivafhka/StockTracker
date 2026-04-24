using StockTracker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockTracker.Application.Interfaces
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : IDomainEvents;
    }
}
