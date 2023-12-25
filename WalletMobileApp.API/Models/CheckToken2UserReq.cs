namespace WalletMobileApp.API.Models
{
    public class CheckToken2UserReq
    {
        public Guid? UserGuid { get; set; }
        public Guid? Token { get; set; }
    }
}
