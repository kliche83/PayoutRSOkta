namespace Payout.Domain
{
    public class SecurityUserModuleControlFactory
    {
        public SecurityUserModuleControlDTO Create(PAYOUTsecurityUserModuleControl securityUserModuleControl)
        {
            if (securityUserModuleControl != null)
            {
                return new SecurityUserModuleControlDTO()
                {
                    Id = securityUserModuleControl.Id,
                    ModuleControlId = securityUserModuleControl.ModuleControlId,
                    UserId = securityUserModuleControl.UserId
                };
            }

            return new SecurityUserModuleControlDTO();
        }
    }
}
