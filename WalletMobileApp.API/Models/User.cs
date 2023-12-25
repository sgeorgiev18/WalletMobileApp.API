namespace WalletMobileApp.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public Guid UserGuid { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public int RoleId { get; set; }
    }
}
