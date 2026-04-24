using System;
using System.Collections.Generic;
using System.Text;

namespace StockTracker.Application.Interfaces
{
    public interface INotificationSender
    {
        Task SendAsync(
            Guid userId,
            string subject,
            string message,
            NotificationChannel channel,
            CancellationToken cancellationToken = default);
    }

    public enum NotificationChannel
    {
        Email = 1,
        Telegram = 2
    }
}
