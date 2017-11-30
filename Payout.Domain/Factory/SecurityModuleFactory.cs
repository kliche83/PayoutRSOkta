
namespace Payout.Domain
{
    public class SecurityModuleFactory
    {
        public SecurityModuleDTO Create(PAYOUTsecurityModule securityModule)
        {
            if (securityModule != null)
            {
                return new SecurityModuleDTO()
                {
                    Id = securityModule.Id,                    
                    Description = securityModule.Description,
                    ParentId = securityModule.ParentId
                };
            }

            return new SecurityModuleDTO();
        }
    }
}
