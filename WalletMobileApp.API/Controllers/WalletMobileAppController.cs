using WalletMobileApp.API.Models;
using WalletMobileApp.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace WalletMobileApp.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class WalletMobileAppController : Controller
    {
        private readonly IUserService _userService;

        public WalletMobileAppController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [Route("register")]
        public IActionResult RegisterUser([FromBody]RegisterUserReq req) 
        {
            if (req.Email == null || req.Password == null)
                return Ok(new RegisterUserResponse() { Success = false, ErrorMessage = "ERROR: INVALID PARAMETERS" });
            var newUser = new User() { Email = req.Email, Password = req.Password, Username = req.UserName };
            try
            {
                var result = _userService.RegisterUser(newUser);
                return Ok(result);
            }
            catch (Exception e)
            {
                return Ok(new RegisterUserResponse() { Success = false, ErrorMessage = e.Message });
            }
        }
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginUserReq req) 
        {
            if(req.Email== null || req.Password == null)
                return Ok(new LoginResponse() { Success = false, ErrorMessage = "ERROR: INVALID PARAMETERS" });
            try
            {
                var res = _userService.Login(req.Email, req.Password);
                return Ok(res);
            }
            catch(Exception e)
            {
                return Ok(new LoginResponse() { Success = false, ErrorMessage = e.Message });
            }
        }
        [HttpPost]
        [Route("createtoken")]
        public IActionResult CreateToken2User([FromBody]CreateToken2UserReq req)
        {
            if(req == null) 
                return Ok(new CreateToken2UserResponse() { Success = false, ErrorMessage = "ERROR: INVALID PARAMETERS" });
            if(req.UserGuid==null || req.UserGuid== Guid.Empty) 
                Ok(new CreateToken2UserResponse() { Success = false, ErrorMessage = "ERROR: INVALID PARAMETERS" });
            try
            {
                var res = _userService.CreateToken2User(req);
                return Ok(res);
            }
            catch (Exception e)
            {
                return Ok(new CreateToken2UserResponse() { Success = false, ErrorMessage = e.Message });
            }
        }
        [HttpPost]
        [Route("checktoken")]
        public IActionResult CheckToken2User([FromBody]CheckToken2UserReq req)
        {
            if (req.Token == null || req.UserGuid == null)
                return Ok(new CheckToken2UserResponse() { Success = false, ErrorMessage = "ERROR: TOKEN OR UserGUID IS NULL" });
            try
            {
                var res = _userService.CheckToken2User((Guid)req.UserGuid, (Guid)req.Token);
                return Ok(res);
            }
            catch(Exception e)
            {
                return Ok(new CheckToken2UserResponse() { Success = false, ErrorMessage=e.Message });
            }
        }

        [HttpPost]
        [Route("refreshtoken")]
        public IActionResult RefreshToken2User([FromBody] RefreshToken2UserReq req)
        {
            if (req == null || req.Token == null || req.UserGuid == null)
                return Ok(new RefreshToken2UserResponse { Success = false, ErrorMessage = "ERROR: INVALID PARAMETERS" });
            try
            {
                var res = _userService.RefreshToken2User((Guid)req.UserGuid, (Guid)req.Token);
                return Ok(res);
            }
            catch(Exception e)
            {
                return Ok(new RefreshToken2UserResponse { Success = false, ErrorMessage = e.Message });
            }
        }
    }
}
