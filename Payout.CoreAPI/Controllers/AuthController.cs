using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Payout.CoreAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Payout.CoreAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<PayoutUser> _userManager;
        private readonly SignInManager<PayoutUser> _signInManager;
        private readonly RoleManager<PayoutRole> _roleManager;
        private IPasswordHasher<PayoutUser> _passwordHasher;
        private ILogger<AuthController> _logger;
        private readonly AppSettingsCls.ConfigJwtSecurityToken _optionsToken;
        private readonly AppSettingsCls.URLKeys _optionURLKeys;
        private readonly AppSettingsCls.OktaKeys _optionOktaKeys;

        public AuthController(UserManager<PayoutUser> userManager, SignInManager<PayoutUser> signInManager, RoleManager<PayoutRole> roleManager
            , IPasswordHasher<PayoutUser> passwordHasher, ILogger<AuthController> logger, 
            IOptions<AppSettingsCls.ConfigJwtSecurityToken> optionsAccessor, 
            IOptions<AppSettingsCls.URLKeys> optionURLKeys,
            IOptions<AppSettingsCls.OktaKeys> optionOktaKeys)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _optionsToken = optionsAccessor.Value;
            _optionURLKeys = optionURLKeys.Value;
            _optionOktaKeys = optionOktaKeys.Value;
        }
                
        [HttpPost("CreateToken")]
        [Route("token")]
        public async Task<IActionResult> CreateToken()
        {
            try
            {
                string UserName = "";
                string OktaUserId = "";
                string UserRole = "";
                if (System.Threading.Thread.CurrentPrincipal != null && ((ClaimsIdentity)System.Threading.Thread.CurrentPrincipal.Identity).Claims.Count() > 0)
                {
                    UserName = ((ClaimsIdentity)System.Threading.Thread.CurrentPrincipal.Identity).FindFirst("preferred_username").Value;
                    UserRole = ((ClaimsIdentity)System.Threading.Thread.CurrentPrincipal.Identity).Claims.Where(x => x.Type.Contains("identity/claims/role")).Select(x => x.Value).FirstOrDefault();
                }
                else
                {
                    UserName = ((ClaimsIdentity)HttpContext.User.Identity).FindFirst("preferred_username").Value;                    
                }

                PayoutUser user = await _userManager.FindByNameAsync(UserName);

                if (((ClaimsIdentity)HttpContext.User.Identity).Claims.Count() > 0)
                {
                    OktaUserId = ((ClaimsIdentity)HttpContext.User.Identity).Claims.Where(x => x.Type.Contains("nameidentifier")).Select(x => x.Value).FirstOrDefault();
                    UserRole = OktaRequests.getGroupsFromUserOkta(_optionOktaKeys.URL, OktaUserId, _optionOktaKeys.APIKey).Keys.FirstOrDefault();

                    if (user == null)
                    {
                        user = new PayoutUser();
                        user.Email = UserName;
                        user.UserName = UserName;
                        user.FirstName = ((ClaimsIdentity)HttpContext.User.Identity).FindFirst("given_name").Value;
                        user.LastName = ((ClaimsIdentity)HttpContext.User.Identity).FindFirst("family_name").Value;
                        
                        var result = CreateUserFromOkta(user, UserRole).Result;
                    }
                }

                var userClaims = await _userManager.GetClaimsAsync(user);                
                return Redirect(CreateToken(user, UserRole).Result);                
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while creating token: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "error while creating token");
            }
        }

        private async Task<string> CreateToken(PayoutUser user, string UserRole)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(ClaimTypes.Role, UserRole),
                        new Claim(ClaimTypes.GivenName, user.FirstName.Trim() + " " + user.LastName.Trim())
                    }.Union(userClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_optionsToken.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _optionsToken.Issuer,
                audience: _optionsToken.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signingCredentials
                );

            string EncodedToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            EncodedToken = WebUtility.UrlEncode(EncodedToken);

            string EncodedExpiration = jwtSecurityToken.ValidTo.ToString();

            return string.Format(_optionURLKeys.ClientURL + "/Login.aspx?Param1={0}", EncodedToken);
        }



        public async Task<IActionResult> CreateUserFromOkta(PayoutUser user, string strRole)
        {
            try
            {
                PayoutRole role = await _roleManager.FindByNameAsync(strRole);

                if (role != null)
                {                                        
                    user.CreatedBy = "Okta";
                    user.IsOkta = true;
                    user.IsDisabled = false;
                    IdentityResult chkUser = await _userManager.CreateAsync(user);

                    //Add default User to Role Admin    
                    if (chkUser.Succeeded)
                    {
                        var result1 = await _userManager.AddToRoleAsync(user, role.Name);

                        if (result1.Succeeded)
                        {
                            return Ok(result1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while creating Admin: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "error while creating Admin: " + ex.Message);
            }

            return BadRequest();
        }
    }
}