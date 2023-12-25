namespace WalletMobileApp.API.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public Guid Token { get; set; }
        public Guid UserGuid { get; set; }
        public string ErrorMessage { get; set; }
        public int Balance { get; set; }
    }
}
