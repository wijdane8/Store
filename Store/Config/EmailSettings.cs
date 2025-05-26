namespace Store.Services
{
    public class EmailSettings
    {
        public string FromAddress { get; set; } = null!; // Already present, adding = null! for clarity on nullable reference types
        public string FromName { get; set; } = null!; // Adding = null! for clarity on nullable reference types
        public string? ContactRecipientEmail { get; set; } // This is fine as nullable
    }
}