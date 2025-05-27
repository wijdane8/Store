namespace Store.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string? Message { get; set; }  // Add this line

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
