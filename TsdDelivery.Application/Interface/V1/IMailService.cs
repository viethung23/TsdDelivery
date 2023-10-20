using TsdDelivery.Application.Models.Mail;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Application.Interface.V1;

public interface IMailService
{
    Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    Task<string> GetEmailTemplateForgotPassword(string nameTemplate,string clientHost,string code,User user);
}