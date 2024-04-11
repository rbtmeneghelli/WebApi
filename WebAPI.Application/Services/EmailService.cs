﻿using MailKit.Security;
using MimeKit.Text;
using MimeKit;

namespace WebAPI.Application.Services;

public class EmailService : GenericService, IEmailService
{
    EmailSettings _emailSettings { get; }
    private readonly IGenericRepository<EmailType> _iEmailTypeRepository;
    private readonly IGenericRepository<EmailDisplay> _iEmailDisplayRepository;

    public EmailService(
        EmailSettings emailSettings,
        INotificationMessageService notificationMessageService,
        IGenericRepository<EmailType> iEmailTypeRepository,
        IGenericRepository<EmailDisplay> iEmailDisplayRepository
        ) : base(notificationMessageService)
    {
        _emailSettings = emailSettings;
        _iEmailTypeRepository = iEmailTypeRepository;
        _iEmailDisplayRepository = iEmailDisplayRepository;
    }

    private void setEmailDomain()
    {
        var emailType = _iEmailTypeRepository.GetAll().Where(x => x.IsActive && x.Description.Equals(_emailSettings.Host, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        _emailSettings.PrimaryDomain = emailType is null ? _emailSettings.PrimaryDomain : emailType.SmtpConfig;
    }

    private MimeEntity BuildMessage(EmailConfig emailConfig, string appPath)
    {
        if (emailConfig.HasAttachment)
        {
            var builder = new BodyBuilder();
            builder.HtmlBody = emailConfig.Body;
            builder.Attachments.Add(Path.Combine(appPath, "Arquivos", "arquivo.pdf"));
            return builder.ToMessageBody();
        }
        else
        {
            return new TextPart(TextFormat.Html)
            {
                Text = emailConfig.Body
            };
        }
    }

    private async Task SendEmailAsync(EmailConfig emailConfig, string appPath)
    {
        setEmailDomain();
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(emailConfig.EmailFrom.Address);
        emailConfig.EmailTo.Split(';').ToList().ForEach(p => email.To.Add(MailboxAddress.Parse(p.Trim())));
        email.Subject = emailConfig.Subject;
        email.Priority = emailConfig.Priority;
        email.Body = BuildMessage(emailConfig, appPath);
        await ExecuteMailWithMailKitAsync(emailConfig, email);
    }

    private async Task ExecuteMailWithMailKitAsync(EmailConfig emailConfig, MimeMessage email)
    {
        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            try
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort, SecureSocketOptions.Auto);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailSettings.UsernameEmail, _emailSettings.UserPassword);
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Notify(ex.Message);
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }

    public async Task SendEmailToResetPswAsync(string userName, string appPath)
    {
        var emailDisplay = _iEmailDisplayRepository.GetAll().Include(x => x.EmailTemplate).Where(x => x.IsActive && x.Id == 1L).FirstOrDefault();

        if (GuardClauses.ObjectIsNotNull(emailDisplay))
        {
            EmailConfig emailConfig = new(emailDisplay, userName);
            await SendEmailAsync(emailConfig, appPath);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
