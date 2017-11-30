using System.Linq;

namespace Payout.Domain.IRepositories
{
    public interface IRepository
    {
        IQueryable<PAYOUTsecurityModule> GetSecurityModules();
        void InsertSecurityModule(PAYOUTsecurityModule securityModule);
        void DeleteSecurityModule(int securityModuleId);
        void UpdateStudent(PAYOUTsecurityModule securityModule);
        void Save();

        IQueryable<PAYOUTsecurityModuleControl> GetSecurityModuleControls();
        IQueryable<PAYOUTsecurityControlAccess> GetSecurityControlAccesses();
        IQueryable<PAYOUTsecurityRoleModuleControl> GetSecurityRoleModuleControls();
        IQueryable<PAYOUTsecurityUserModuleControl> GetSecurityUserModuleControls();
    }
}
