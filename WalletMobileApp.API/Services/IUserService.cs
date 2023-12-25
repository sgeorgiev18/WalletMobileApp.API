using WalletMobileApp.API.Models;

namespace WalletMobileApp.API.Services
{
    public interface IUserService
    {
        public RegisterUserResponse RegisterUser(User user);
        public LoginResponse Login(string email, string password);
        public CreateToken2UserResponse CreateToken2User(CreateToken2UserReq req);
        public CheckToken2UserResponse CheckToken2User(Guid userGuid, Guid token);
        public RefreshToken2UserResponse RefreshToken2User(Guid userGuid, Guid token);
    }
}
