using System.ComponentModel.DataAnnotations;

namespace WalletMobileApp.API.Models
{
    public class User2Balance
    {
        [Key]
        public Guid UserGuid { get; set; }
        public int Balance { get; set; }
    }
}
