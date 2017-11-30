using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payout.Domain;
using Payout.Domain.IRepositories;
using System.Data;

namespace Payout.CoreAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/RSSecurity")]
    public class RSSecurityController : BaseApiController
    {
        private IRepository repository;

        public RSSecurityController(IRepository repoParam) : base(repoParam)
        {
            repository = repoParam;
        }

        [HttpGet]
        //[HttpPost]
        [Route("SecurityModuleGet")]
        public IActionResult SecurityModuleGet()
        {
            IQueryable<PAYOUTsecurityModule> Retailers = Repository.GetSecurityModules();
            //return Ok(Retailers.ToList().OrderBy(i => i.Id).Select(i => SecurityModuleFactory.Create(i)));
            return new ObjectResult(Retailers.ToList().OrderBy(i => i.Id).Select(i => SecurityModuleFactory.Create(i)));
        }

        [HttpPost]
        [Route("SecurityModuleCreate")]
        public IActionResult SecurityModuleCreate([Bind("Description, ParentId")]
                PAYOUTsecurityModule securityModule)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repository.InsertSecurityModule(securityModule);
                    repository.Save();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            return Ok();
            //return View(student);
        }
    }
}