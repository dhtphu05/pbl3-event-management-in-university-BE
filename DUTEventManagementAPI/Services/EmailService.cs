using DUTEventManagementAPI.Models;
using DUTEventManagementAPI.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace DUTEventManagementAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public EmailService(
            EmailConfiguration emailConfig,
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _emailConfig = emailConfig;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        public async Task SendEmailWithTemplateAsync(Event eventModel, string[] recipients, string templatePath, string subject)
        {
            Console.WriteLine("Sending email with template...");

            try
            {
                var htmlContent = await RenderViewToStringAsync(templatePath, eventModel);
                Console.WriteLine($"Template rendered successfully. Content length: {htmlContent?.Length ?? 0}");

                if (string.IsNullOrEmpty(htmlContent))
                {
                    Console.WriteLine("Template content is null or empty");
                    throw new ArgumentNullException("Template content is null or empty");
                }
                if (recipients == null || !recipients.Any())
                {
                    Console.WriteLine("Recipient emails list is required");
                    throw new ArgumentNullException("Recipient emails list is required");
                }
                if (string.IsNullOrEmpty(subject))
                {
                    Console.WriteLine("Email subject is required");
                    throw new ArgumentNullException("Email subject is required");
                }
                Console.WriteLine("Sending email to the following recipients:");
                foreach (var recipient in recipients)
                {
                    if (string.IsNullOrEmpty(recipient))
                    {
                        throw new ArgumentNullException("Recipient email is required");
                    }
                    Console.WriteLine(recipient);
                }

                var message = new Message(recipients, subject, htmlContent);
                var emailMessage = CreateEmailMessageWithHtml(message);
                Send(emailMessage);
                Console.WriteLine("Email sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendEmailWithTemplateAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("DUT Event Management", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }

        private MimeMessage CreateEmailMessageWithHtml(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("DUT Event Management", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };
            return emailMessage;
        }

        private async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            Console.WriteLine($"Starting to render view: {viewName}");

            try
            {
                // Tạo HTTP context với request services
                var httpContext = new DefaultHttpContext();
                httpContext.RequestServices = _serviceProvider;

                // Tạo action context
                var actionContext = new ActionContext(
                    httpContext,
                    httpContext.GetRouteData() ?? new RouteData(),
                    new ActionDescriptor()
                );

                Console.WriteLine("Created action context");

                // Tìm view với đường dẫn tuyệt đối
                var viewResult = _viewEngine.GetView(null, viewName, false);

                if (viewResult.View == null)
                {
                    throw new FileNotFoundException($"View '{viewName}' not found. Searched locations: {string.Join(", ", viewResult.SearchedLocations)}");
                }

                Console.WriteLine($"Found view: {viewName}");

                // Tạo view data dictionary
                var modelMetadataProvider = _serviceProvider.GetRequiredService<IModelMetadataProvider>();
                var viewDictionary = new ViewDataDictionary<TModel>(modelMetadataProvider, new ModelStateDictionary())
                {
                    Model = model
                };

                Console.WriteLine("Created view data dictionary");

                // Render view
                using (var stringWriter = new StringWriter())
                {
                    var viewContext = new ViewContext(
                        actionContext,
                        viewResult.View,
                        viewDictionary,
                        new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                        stringWriter,
                        new HtmlHelperOptions()
                    );

                    Console.WriteLine("Created view context, starting render...");
                    await viewResult.View.RenderAsync(viewContext);
                    Console.WriteLine("View rendered successfully");

                    var result = stringWriter.ToString();
                    Console.WriteLine($"Rendered content length: {result.Length}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering view: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                    client.Send(mailMessage);
                }
                catch
                {
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}