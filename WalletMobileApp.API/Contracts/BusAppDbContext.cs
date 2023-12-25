using WalletMobileApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace WalletMobileApp.API.Contracts
{
    public class BusAppDbContext : DbContext
    {
        public BusAppDbContext(DbContextOptions<BusAppDbContext> options) 
            : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Token2User> Token2User { get; set; }
        public DbSet<User2Balance> User2Balance { get; set; }
    }
}
