namespace DUTEventManagementAPI.Extensions
{
    public static class FluentEmailExtensions
    {
        public static void AddFluentEmail(this IServiceCollection services,
            ConfigurationManager configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");

            var defaultFromEmail = emailSettings["DefaultSender"];
            var host = emailSettings["Host"];
            var port = emailSettings.GetValue<int>("Port");  
            var userName = emailSettings["UserName"];
            var password = emailSettings["Password"];

            Console.WriteLine($"DefaultFromEmail: {defaultFromEmail}");
            Console.WriteLine($"Host: {host}");
            Console.WriteLine($"Port: {port}");
            Console.WriteLine($"UserName: {userName}");
            Console.WriteLine($"Password: {password}");

            services.AddFluentEmail(defaultFromEmail)
                .AddSmtpSender(host, port, userName, password);
        }
    }
}
