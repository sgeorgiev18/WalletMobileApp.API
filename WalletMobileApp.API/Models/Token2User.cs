using System;

namespace WalletMobileApp.API.Models
{
    public class Token2User
    {
        public int Id { get; set; }
        public Guid UserGuid { get; set; }
        public int UserId { get; set; }
        public Guid Token { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}