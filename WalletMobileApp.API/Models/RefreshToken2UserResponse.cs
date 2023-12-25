namespace WalletMobileApp.API.Models
{
    public class RefreshToken2UserResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public Guid UserGuid { get; set; }
        public Guid Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
