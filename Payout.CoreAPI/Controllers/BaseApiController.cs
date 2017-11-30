using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payout.Domain;
using Payout.Domain.IRepositories;


namespace Payout.CoreAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/BaseApi")]
    public class BaseApiController : Controller
    {
        SecurityModuleFactory securityModuleFactory;
        SecurityModuleControlFactory securityModuleControlFactory;
        SecurityRoleModuleControlFactory securityRoleModuleControlFactory;

        IRepository repository;

        public BaseApiController(IRepository repoParam)
        {
            repository = repoParam;
        }


        protected IRepository Repository
        {
            get
            {
                return repository;
            }
        }

        protected SecurityModuleFactory SecurityModuleFactory
        {
            get
            {
                if (securityModuleFactory == null)
                    securityModuleFactory = new SecurityModuleFactory();
                return securityModuleFactory;
            }
        }

        protected SecurityModuleControlFactory SecurityModuleControlFactory
        {
            get
            {
                if (securityModuleControlFactory == null)
                    securityModuleControlFactory = new SecurityModuleControlFactory();
                return securityModuleControlFactory;
            }
        }

        protected SecurityRoleModuleControlFactory SecurityRoleModuleControlFactory
        {
            get
            {
                if (securityRoleModuleControlFactory == null)
                    securityRoleModuleControlFactory = new SecurityRoleModuleControlFactory();
                return securityRoleModuleControlFactory;
            }
        }
    }
}