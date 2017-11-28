using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Payout.CoreAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Linq;


namespace Payout.CoreAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<PayoutUser> _userManager;
        private readonly RoleManager<PayoutRole> _roleManager;
        private ILogger<AuthController> _logger;
        private readonly SignInManager<PayoutUser> _signInManager;
        private readonly AppSettingsCls.URLKeys _optionURLKeys;
        private readonly AppSettingsCls.OktaKeys _optionOktaKeys;
        private readonly AppSettingsCls.MailCredentials _optionMailCredentials;

        public AccountController(UserManager<PayoutUser> userManager, SignInManager<PayoutUser> signInManager, RoleManager<PayoutRole> roleManager
            , ILogger<AuthController> logger, 
            IOptions<AppSettingsCls.URLKeys> optionURLKeys,
            IOptions<AppSettingsCls.OktaKeys> optionOktaKeys,
            IOptions<AppSettingsCls.MailCredentials> optionMailCredentials)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _signInManager = signInManager;
            _optionURLKeys = optionURLKeys.Value;
            _optionOktaKeys = optionOktaKeys.Value;
            _optionMailCredentials = optionMailCredentials.Value;
        }

        [HttpPost("FirstSeedAdminRolesandUser")]
        [Route("FirstSeedITAdmin")]
        public async Task<IActionResult> FirstSeedAdminRolesandUser()
        {
            try
            {
                bool x = await _roleManager.RoleExistsAsync("ITAdmin");
                if (!x)
                {
                    // first we create Admin rool    
                    var role = new PayoutRole();
                    role.Name = "PayoutITAdmin";
                    role.CreatedBy = "System";
                    role.IsDisabled = false;
                    await _roleManager.CreateAsync(role);

                    //Here we create a Admin super user who will maintain the website
                    var user = new PayoutUser();
                    user.UserName = "carlos@innovage.net";
                    user.Email = "carlos@innovage.net";
                    user.FirstName = "Carlos";
                    user.LastName = "Sanchez";
                    user.CreatedBy = "System";
                    user.IsDisabled = false;
                    user.IsOkta = false;

                    string userPWD = "#Qwrty123!";
                    IdentityResult chkUser = await _userManager.CreateAsync(user, userPWD);

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

        [HttpPost("CreateUser")]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] PayoutUser model)
        {
            try
            {
                PayoutUser payoutUser = await _userManager.FindByEmailAsync(model.Email);
                if (payoutUser == null)
                {
                    model.UserName = model.Email;
                    model.IsDisabled = false;                    
                    IdentityResult chkUser = await _userManager.CreateAsync(model);

                    if (chkUser.Succeeded)
                    {

                        if (model.IsOkta == true)
                        {
                            payoutUser = _userManager.FindByEmailAsync(model.Email).Result;
                            OktaRequests.CreateUserInOkta(_optionOktaKeys.URL, _optionOktaKeys.APIKey, payoutUser);
                        }
                        else
                        {
                            string code = _userManager.GenerateEmailConfirmationTokenAsync(model).Result;
                            //code = System.Web.HttpUtility.UrlEncode(code);

                            var callbackUrl = Url.Action(
                                           "ConfirmEmail", "Account",
                                           new { userId = model.Id, code = code },
                                           protocol: Request.Scheme);


                            string Body = string.Format(@"Please confirm your account by clicking this 
                                <a href=""{0}"">link</a>", callbackUrl);

                            Helpers.SendEmail(_optionMailCredentials, model.Email, "Payout RS - password confirmation message", Body);
                        }

                        return Ok(chkUser);
                    }

                    foreach (var error in chkUser.Errors)
                    {
                        ModelState.AddModelError("error", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("error", "Username already exist");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while creating User: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "error while creating User: " + ex.Message);
            }

            return BadRequest();
        }

        [HttpPost("DeleteUser")]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] PayoutUser model)
        {
            try
            {
                PayoutUser payoutUser = await _userManager.FindByEmailAsync(model.Email);                
                var resultUpd = await _userManager.DeleteAsync(payoutUser);

                if (resultUpd.Succeeded)
                {
                    return Ok(resultUpd);
                }

                foreach (var error in resultUpd.Errors)
                {
                    ModelState.AddModelError("error", error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while deleting User: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "error while deleting User: " + ex.Message);
            }

            return BadRequest();
        }

        [HttpPost("UpdateUser")]
        [Route("UpdateUser")]
        //public async Task<IActionResult> UpdateUser([FromBody] UpdateIdentity objUpdateIdentity)
        public IActionResult UpdateUser([FromBody] UpdateIdentity objUpdateIdentity)
        {
            try                
            {
                PayoutUser payoutUser = _userManager.FindByEmailAsync(objUpdateIdentity.id).Result;
                PropertyInfo prop = payoutUser.GetType().GetProperty(objUpdateIdentity.colName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    prop.SetValue(payoutUser, objUpdateIdentity.colValue, null);
                }

                dynamic resultUpd = null;
                if (objUpdateIdentity.colName == "IsOkta") //First updates Okta groups if Col is Okta checkbox
                {
                    resultUpd = ChangeOktaUserGroups(objUpdateIdentity.id, objUpdateIdentity.colValue);
                }
                
                if (resultUpd == null || ((StatusCodeResult)resultUpd).StatusCode != 400) //Then updates RS Database
                {
                    resultUpd = _userManager.UpdateAsync(payoutUser).Result;
                    if (resultUpd.Succeeded)
                        return Ok(resultUpd);                                        
                }

                //foreach (var error in resultUpd.Errors)
                //{
                //    ModelState.AddModelError("error", error.Description);
                //}
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while updating User: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "error while updating User: " + ex.Message);
            }

            return BadRequest();
        }

        [HttpPost("CreateRole")]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] PayoutRole model)
        {
            try
            {
                bool x = await _roleManager.RoleExistsAsync(model.Name);
                if (!x)
                {
                    var role = new PayoutRole();
                    model.IsDisabled = false;
                    IdentityResult chkRole = await _roleManager.CreateAsync(model);

                    if (chkRole.Succeeded)
                    {
                        if (model.Name.ToLower().Contains("payout"))
                        {                            
                            if (OktaRequests.AddGroup(_optionOktaKeys.URL, _optionOktaKeys.APIKey, model.Name))
                            {
                                string GroupId = OktaRequests.GetGroupIdFromOkta(_optionOktaKeys.URL, _optionOktaKeys.APIKey, model.Name);

                                if(GroupId != "")
                                    if(OktaRequests.AssignGroupToApp(_optionOktaKeys.URL, _optionOktaKeys.APIKey, GroupId, _optionOktaKeys.ClientId))
                                        return Ok(chkRole);
                            }
                        }
                    }

                    foreach (var error in chkRole.Errors)
                    {
                        ModelState.AddModelError("error", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("error", "Role already exist");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while creating Role: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "error while creating Role: " + ex.Message);
            }

            return BadRequest();
        }
        
        [HttpPost("UpdateRole")]
        [Route("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateIdentity objUpdateIdentity)
        {
            try
            {                
                PayoutRole payoutRole = await _roleManager.FindByNameAsync(objUpdateIdentity.id);
                PropertyInfo prop = payoutRole.GetType().GetProperty(objUpdateIdentity.colName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    prop.SetValue(payoutRole, objUpdateIdentity.colValue, null);
                }

                //payoutRole.IsDisabled = true;

                var resultUpd = await _roleManager.UpdateAsync(payoutRole);                

                if (resultUpd.Succeeded)
                {
                    return Ok(resultUpd);
                }

                foreach (var error in resultUpd.Errors)
                {
                    ModelState.AddModelError("error", error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while updating Role: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "error while updating Role: " + ex.Message);
            }

            return BadRequest();
        }

        [HttpPost("DeleteRole")]
        [Route("DeleteRole")]
        public async Task<IActionResult> DeleteRole([FromBody] PayoutRole model)
        {
            try
            {
                PayoutRole payoutRole = await _roleManager.FindByNameAsync(model.Name);
                var resultDel = await _roleManager.DeleteAsync(payoutRole);

                if (resultDel.Succeeded)
                {
                    if (OktaRequests.DeleteGroup(_optionOktaKeys.URL, _optionOktaKeys.APIKey, model.Name))
                    {
                        return Ok(resultDel);
                    }
                }

                foreach (var error in resultDel.Errors)
                {
                    ModelState.AddModelError("error", error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while deleting Role: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "error while deleting Role: " + ex.Message);
            }

            return BadRequest();
        }

        [HttpPost("SetUserRole")]
        [Route("SetUserRole")]        
        public IActionResult SetUserRole([FromBody] UserRoleViewModel objUserRoleViewModel)
        {
            try
            {
                PayoutUser payoutUser = _userManager.FindByEmailAsync(objUserRoleViewModel.Username).Result;

                var roles = _userManager.GetRolesAsync(payoutUser).Result;
                var result = _userManager.RemoveFromRolesAsync(payoutUser, roles.ToArray()).Result;

                Dictionary<string, string> OktaUserId = OktaRequests.GetUserFromOkta(_optionOktaKeys.URL, _optionOktaKeys.APIKey, objUserRoleViewModel.Username);
                if (objUserRoleViewModel.IsOkta)
                {                    
                    Dictionary<string, string> GroupsUser = OktaRequests.getGroupsFromUserOkta(_optionOktaKeys.URL, OktaUserId.FirstOrDefault().Value, _optionOktaKeys.APIKey);
                    foreach (string GroupId in GroupsUser.Values)
                    {
                        if(!OktaRequests.RemoveUserFromGroup(_optionOktaKeys.URL, _optionOktaKeys.APIKey, GroupId, OktaUserId.FirstOrDefault().Value))
                            return StatusCode((int)HttpStatusCode.InternalServerError, "error unassigning role from user in Okta");
                    }
                }
                    
                result =  _userManager.AddToRoleAsync(payoutUser, objUserRoleViewModel.Role).Result;

                if (objUserRoleViewModel.IsOkta)
                {
                    string OktaGroup = OktaRequests.GetGroupIdFromOkta(_optionOktaKeys.URL, _optionOktaKeys.APIKey, objUserRoleViewModel.Role);
                    if (!OktaRequests.AddUserToGroup(_optionOktaKeys.URL, _optionOktaKeys.APIKey, OktaGroup, OktaUserId.FirstOrDefault().Value))
                        return StatusCode((int)HttpStatusCode.InternalServerError, "error while setting role to user in Okta");
                }

                if (result.Succeeded)
                {
                    return Ok(result);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("error", error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"error while setting role to user: {ex}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "error while setting role to user: " + ex.Message);
            }

            return BadRequest();
        }
                
        public IActionResult ChangeOktaUserGroups(string UserName, bool IsChecked = false)
        {
            //string IdProvider = ((ClaimsIdentity)HttpContext.User.Identity).Claims.Where(x => x.Type.Contains("nameidentifier")).Select(x => x.Value).FirstOrDefault();
            Dictionary<string, string> OktaGroupsFromUser = OktaRequests.getGroupsFromUserOkta(_optionOktaKeys.URL, UserName, _optionOktaKeys.APIKey);
            Dictionary<string,string> OktaUser = OktaRequests.GetUserFromOkta(_optionOktaKeys.URL, _optionOktaKeys.APIKey, UserName);

            if (OktaUser.Count == 0)
            {
                PayoutUser User = _userManager.FindByEmailAsync(UserName).Result;
                OktaRequests.CreateUserInOkta(_optionOktaKeys.URL, _optionOktaKeys.APIKey, User);

                OktaUser = OktaRequests.GetUserFromOkta(_optionOktaKeys.URL, _optionOktaKeys.APIKey, UserName);
            }
                

            foreach (KeyValuePair<string, string> UserRole in OktaGroupsFromUser)
            {
                if (!OktaRequests.RemoveUserFromGroup(_optionOktaKeys.URL, _optionOktaKeys.APIKey, UserRole.Value, OktaUser.First().Value))                    
                    return BadRequest(); 
            }

            if(IsChecked)
            {
                string RS_Role = _userManager.GetRolesAsync(_userManager.FindByEmailAsync(UserName).Result).Result.FirstOrDefault();
                string OktaGroup = OktaRequests.GetGroupIdFromOkta(_optionOktaKeys.URL, _optionOktaKeys.APIKey, RS_Role);

                if (!OktaRequests.AddUserToGroup(_optionOktaKeys.URL, _optionOktaKeys.APIKey, OktaGroup, OktaUser.First().Value))
                    return BadRequest();
            }

            return Ok();
        }

        [Route("Login")]
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Challenge(OpenIdConnectDefaults.AuthenticationScheme);
            }

            //((Microsoft.AspNetCore.Mvc.StatusCodeResult)requestResult).StatusCode
            var requestResult = ValidateOktaUserRole();

            try
            {
                if (((ObjectResult)requestResult).StatusCode == 200 && ((ObjectResult)requestResult).Value.ToString() == "NotInGroup")
                {
                    //HttpContext.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(string.Empty), null);
                    //return Redirect(string.Format(_optionURLKeys.ClientURL + "/Login.aspx?Param1={0}", "NotInGroup"));
                    return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
                }
            }
            catch
            {

            }

            return RedirectToAction("token", "api/Auth");
        }

        [Route("LoginCredentials")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, true, false);

            PayoutUser user = await _userManager.FindByEmailAsync(userName);
            dynamic roleResult = await _userManager.GetRolesAsync(user);
            roleResult = roleResult[0];

            if (result.Succeeded)
            {
                var claims = new List<Claim>();                
                claims.Add(new Claim("preferred_username", userName));
                claims.Add(new Claim("preferred_username", userName));
                claims.Add(new Claim(ClaimTypes.Role, roleResult));

                HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
                System.Threading.Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

                return RedirectToAction("token", "api/Auth");
            }
            else
            {
                //string PostURL = string.Format(_optionURLKeys.ClientURL + "/Login.aspx");
                string PostURL = string.Format(_optionURLKeys.ClientURL + "/Login.aspx?Param1={0}", "IncorrectCredentials");
                return Redirect(PostURL);
            }            
        }

        [Route("Logout")]
        public IActionResult Logout()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
            }

            //string PostURL = string.Format(_optionURLKeys.ClientURL + "/Login.aspx");
            string PostURL = _optionURLKeys.ClientURL + "/Login.aspx";
            return Redirect(PostURL);
        }

        private IActionResult ValidateOktaUserRole()
        {
            string UserName = ((ClaimsIdentity)HttpContext.User.Identity).FindFirst("preferred_username").Value;            
            PayoutUser payoutUser = _userManager.FindByEmailAsync(UserName).Result;
            if (payoutUser != null)
            {
                try
                {
                    string OktaUserId = ((ClaimsIdentity)HttpContext.User.Identity).Claims.Where(x => x.Type.Contains("nameidentifier")).Select(x => x.Value).FirstOrDefault();
                    string UserRole = OktaRequests.getGroupsFromUserOkta(_optionOktaKeys.URL, OktaUserId, _optionOktaKeys.APIKey).Keys.FirstOrDefault();
                    var IsInRoleResult = _userManager.IsInRoleAsync(payoutUser, UserRole).Result;

                    if (!IsInRoleResult)
                        SetUserRole(new UserRoleViewModel { Username = payoutUser.Email, Role = UserRole });
                }
                catch
                {
                    return Ok("NotInGroup");                    
                }                
            }            

            return BadRequest();
        }

        [HttpPost("ConfirmEmail")]
        [Route("ConfirmEmail")]
        public ActionResult ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            
            var result = _userManager.ConfirmEmailAsync(_userManager.FindByIdAsync(userId).Result, code).Result;
            if (result.Succeeded)
            {
                code = System.Web.HttpUtility.UrlEncode(code);

                string URL = string.Format(_optionURLKeys.ClientURL + "/AccountResetPassword.aspx?UserId={0}&Code={1}", userId, code);
                return Redirect(URL);
            }
            
            return StatusCode((int)HttpStatusCode.InternalServerError, " wrong account validation link");
        }

        [HttpPost("SendEmailResetPassword")]
        [Route("SendEmailResetPassword")]
        public ActionResult SendEmailResetPassword(string Email)
        {
            PayoutUser payoutUser = _userManager.FindByEmailAsync(Email).Result;
            if (payoutUser != null)
            {
                string code = _userManager.GeneratePasswordResetTokenAsync(payoutUser).Result;
                code = System.Web.HttpUtility.UrlEncode(code);

                string callbackUrl = string.Format(_optionURLKeys.ClientURL + "/AccountResetPassword.aspx?UserId={0}&Code={1}", payoutUser.Id, code);

                //var callbackUrl = Url.Action(
                //                       "ConfirmEmail", "Account",
                //                       new { userId = payoutUser.Id, code = code },
                //                       protocol: Request.Scheme);

                string Body = string.Format(@"A Password reset was requested from your account. To proceed, please click on the following link
                                <a href=""{0}"">link</a>", callbackUrl);

                Helpers.SendEmail(_optionMailCredentials, Email, "Payout RS - password reset message", Body);


                string URL = _optionURLKeys.ClientURL + "/AccountForgotPassword.aspx?OkMessage=True";
                return Redirect(URL);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, "Error sending email for password reseting");
        }

        [HttpPost("ResetPassword")]
        [Route("ResetPassword")]
        public ActionResult ResetPassword(string UserId, string Token, string NewPassword)
        {
            PayoutUser payoutUser = _userManager.FindByIdAsync(UserId).Result;

            if (payoutUser != null)
            {
                var result = _userManager.ConfirmEmailAsync(payoutUser, Token).Result;
                if (result.Succeeded)
                {
                    string code = _userManager.GeneratePasswordResetTokenAsync(payoutUser).Result;
                    result = _userManager.ResetPasswordAsync(payoutUser, code, NewPassword).Result;
                    if (result.Succeeded)
                    {
                        string URL = _optionURLKeys.ClientURL + "/Login.aspx";
                        return Redirect(URL);
                    }
                }
                else
                {
                    Token = System.Web.HttpUtility.UrlDecode(Token);
                    result = _userManager.ResetPasswordAsync(payoutUser, Token, NewPassword).Result;
                    string OkMessage = "False";
                    if (result.Succeeded)
                    {
                        OkMessage = "True";
                    }
                    string URL = _optionURLKeys.ClientURL + "/AccountResetPassword.aspx?OkMessage=" + OkMessage;
                    return Redirect(URL);
                }
                return BadRequest();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError, "Error reseting password");
        }
    }
}