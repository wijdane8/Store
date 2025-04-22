namespace Store.Areas.Identity.ViewModels
{
    public class LoginWith2faViewModel
    {
        public string TwoFactorCode { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}