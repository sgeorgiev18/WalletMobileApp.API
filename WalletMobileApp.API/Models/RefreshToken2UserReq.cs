namespace WalletMobileApp.API.Models
{
    public class RefreshToken2UserReq
    {
        public Guid? UserGuid { get; set; }
        public Guid? Token { get; set; }
    }
}
