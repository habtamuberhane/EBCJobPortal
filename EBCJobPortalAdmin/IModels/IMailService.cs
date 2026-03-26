using EBCJobPortalAdmin.Models;

namespace EBCJobPortalAdmin.IModels
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);
        Task<bool> SendWithAttachmentsAsync(MailDataWithAttachments mailData, CancellationToken ct);
        // string GetEmailTemplate<T>(string emailTemplate, T emailTemplateModel);
    }
}
