namespace Store.Services
{
    public class SmtpSettings
    {
        public string Server { get; set; } = null!;
        public int Port { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool EnableSsl { get; set; } 
    }
}
