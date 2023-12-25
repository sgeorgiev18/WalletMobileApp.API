namespace WalletMobileApp.API.Models
{
    public class CheckToken2UserResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public Guid UserGuid { get; set; }
        public Guid Token { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
        
    }
}
