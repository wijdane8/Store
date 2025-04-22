namespace Store.Services
{
    public class EmailSettings
    {
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string? ContactRecipientEmail { get; set; } // Add this property
    }
}