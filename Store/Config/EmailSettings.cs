namespace Store.Services
{
    public class EmailSettings
    {
        public string FromAddress { get; set; } = null!; 
        public string FromName { get; set; } = null!; 
        public string? ContactRecipientEmail { get; set; } 
    }
}