using Ordering.Application.Model;

namespace Ordering.Application.Contracts.Infrastructure
{
    public interface IEmailServices
    {
        Task<bool> SendEmailAsync(Email email);
    }
}
