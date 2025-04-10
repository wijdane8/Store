namespace Store.Models
{
    public class ContactFormModel
    {
        public string? Email { get; internal set; }
        public object Name { get; internal set; }
        public object Subject { get; internal set; }
        public object Message { get; internal set; }
    }
}