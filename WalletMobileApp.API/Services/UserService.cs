using WalletMobileApp.API.Contracts;
using WalletMobileApp.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace WalletMobileApp.API.Services
{
    public class UserService : IUserService
    {
        private readonly BusAppDbContext _dbContext;
        private const int keySize = 64;
        private const int iterations = 10000;
        public UserService(BusAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public LoginResponse Login(string email, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(x=>x.Email == email);
            if(user == null)
            {
                return new LoginResponse()
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password",
                    UserGuid = Guid.Empty,
                    Token= Guid.Empty
                };
            }
            var res = VerifyPassword(password, user.Password, user.PasswordSalt);
            if(!res)
            {
                return new LoginResponse()
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password",
                    UserGuid = Guid.Empty,
                    Token= Guid.Empty
                };
            }

            CreateToken2UserReq tokenReq = new CreateToken2UserReq() { UserGuid = user.UserGuid };
            var tokenCreation = CreateToken2User(tokenReq);

            if (!tokenCreation.Success)
            {
                return new LoginResponse()
                {
                    Success = false,
                    ErrorMessage = "Could not create Access Token",
                    UserGuid = user.UserGuid,
                    Token= Guid.Empty
                };
            }

            int userBalance = GetUserBalance(user.UserGuid);

            if(userBalance == -1)
                return new LoginResponse()
                {
                    Success = false,
                    ErrorMessage = "User does not exist in User2Balance Table",
                    UserGuid = user.UserGuid,
                    Token= Guid.Empty
                };

            return new LoginResponse()
            {
                Success = true,
                ErrorMessage = "OK",
                UserGuid = user.UserGuid,
                Balance = userBalance,
                Token= tokenCreation.Token
            };
        }

        public RegisterUserResponse RegisterUser(User user)
        {
            try
            {
                var userExists = _dbContext.Users.FirstOrDefault(x => x.Email == user.Email || x.Username == user.Username);
                if(userExists != null)
                {
                    return new RegisterUserResponse() { Success = false, ErrorMessage = "User with this email or username already exists" };
                }
                var hashedPassAndSalt = HashPassword(user.Password);
                user.Password = hashedPassAndSalt.Hash;
                user.PasswordSalt = hashedPassAndSalt.Salt;
                user.RoleId = 1;
                do
                {
                    user.UserGuid = Guid.NewGuid();
                } while (_dbContext.Users.Any(x => x.UserGuid == user.UserGuid));
                
                _dbContext.Users.Add(user);
                _dbContext.User2Balance.Add(new User2Balance() { UserGuid = user.UserGuid, Balance = 0 });
                _dbContext.SaveChanges();
                return new RegisterUserResponse() { Success = true, ErrorMessage = "OK" }; ;
            }
            catch(Exception ex)
            {
                return new RegisterUserResponse() { Success = false, ErrorMessage = ex.Message };
            }
        }

        public CreateToken2UserResponse CreateToken2User(CreateToken2UserReq req)
        {
            var user = _dbContext.Users.FirstOrDefault(x=>x.UserGuid == req.UserGuid);
            if(user == null)
            {
                return new CreateToken2UserResponse()
                {
                    Success = false,
                    ErrorMessage = "No user found with this UserGuid!",
                    UserGuid = null,
                    Token= Guid.Empty,
                    CreatedOn = null,
                    ExpiresOn= null
                };
            }
            try
            {
                var response = new CreateToken2UserResponse();
                do
                {
                    response.Token = Guid.NewGuid();
                } while (_dbContext.Token2User.Any(x => x.Token == response.Token));
                var createdOn = DateTime.UtcNow;
                var expiresOn = DateTime.UtcNow.AddHours(1);
                var newToken2User = new Token2User()
                {
                    UserId = user.Id,
                    UserGuid = user.UserGuid,
                    Token = (Guid)response.Token,
                    CreatedOn = createdOn,
                    ExpiresOn = expiresOn
                };
                _dbContext.Token2User.Add(newToken2User);
                response.Success = true;
                response.ErrorMessage = "OK";
                response.UserGuid = user.UserGuid;
                response.CreatedOn = createdOn;
                response.ExpiresOn = expiresOn;
                _dbContext.SaveChanges();
                return response;
            }
            catch(Exception ex)
            {
                return new CreateToken2UserResponse { Success = false, ErrorMessage = ex.Message };
            }
            
        }
        public CheckToken2UserResponse CheckToken2User(Guid userGuid, Guid token)
        {
            var user2Token = _dbContext.Token2User.FirstOrDefault(x => x.Token == token && x.UserGuid == userGuid);
            if (user2Token == null)
            {
                return new CheckToken2UserResponse { Success = false, ErrorMessage = "ERROR: NO SUCH TOKEN FOUND FOR THIS USER" };
            }
            if(user2Token.ExpiresOn > DateTime.UtcNow)
            {
                return new CheckToken2UserResponse { Success = false, ErrorMessage = "ERROR: TOKEN EXPIRED" };
            }
            if (user2Token != null && user2Token.ExpiresOn < DateTime.UtcNow)
                return new CheckToken2UserResponse
                {
                    Success = true,
                    ErrorMessage = "OK",
                    ExpiresOn = user2Token.ExpiresOn,
                    CreatedOn = user2Token.CreatedOn,
                    Token = user2Token.Token,
                    UserGuid = user2Token.UserGuid
                };
            return new CheckToken2UserResponse { Success = false, ErrorMessage = "ERROR: SOMETHING WENT WRONG" };
        }
        public RefreshToken2UserResponse RefreshToken2User(Guid userGuid, Guid token)
        {
            var user2Token = _dbContext.Token2User.FirstOrDefault(x => x.Token == token && x.UserGuid == userGuid);
            if (user2Token == null)            
                return new RefreshToken2UserResponse { Success = false, ErrorMessage = "ERROR: NO SUCH TOKEN FOUND FOR THIS USER" };
            
            if(user2Token.ExpiresOn > DateTime.UtcNow)
                return new RefreshToken2UserResponse { Success = false, ErrorMessage = "ERROR: TOKEN EXPIRED" };

            user2Token.ExpiresOn = DateTime.UtcNow;
            _dbContext.Token2User.Update(user2Token);
            return new RefreshToken2UserResponse { Success = true, ErrorMessage= "OK", ExpiresOn = user2Token.ExpiresOn, 
                CreatedOn = user2Token.CreatedOn, Token = user2Token.Token, UserGuid = user2Token.UserGuid };
        }
        protected internal HashSalt HashPassword(string password)
        {
            var saltBytes = new byte[keySize];
            var provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);

            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, iterations);
            var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            HashSalt hashSalt = new HashSalt { Hash = hashPassword, Salt = salt };
            return hashSalt;
        }

        protected internal bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword, saltBytes, iterations);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }

        protected internal int GetUserBalance(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return -1;

            var user2balance = _dbContext.User2Balance.FirstOrDefaultAsync(x=>x.UserGuid== userGuid).Result;

            return user2balance?.Balance ?? -1;
        }
        protected internal class HashSalt
        {
            public string Hash { get; set; }
            public string Salt { get; set; }
        }
    }
}
