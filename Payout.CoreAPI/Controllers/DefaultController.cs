using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payout.CoreAPI.Models;

namespace Payout.CoreAPI.Controllers
{
    
    public class DefaultController : Controller
    {
        private readonly AppSettingsCls.URLKeys _optionURLKeys;

        public DefaultController(IOptions<AppSettingsCls.URLKeys> optionURLKeys)
        {
            _optionURLKeys = optionURLKeys.Value;
        }

        public IActionResult Index()
        {            
            string PostURL = string.Format(_optionURLKeys.ClientURL);
            return Redirect(PostURL);
        }
    }
}