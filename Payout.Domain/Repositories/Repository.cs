using Payout.Domain.IRepositories;
using System;
using System.Data.Entity;
using System.Linq;

namespace Payout.Domain.Repositories
{
    public class Repository : IRepository
    {
        private PayoutEntities _context;

        public Repository(PayoutEntities context)
        {
            _context = context;
        }

        #region securityModules
        public IQueryable<PAYOUTsecurityModule> GetSecurityModules()
        {
            return _context.PAYOUTsecurityModules
                .AsNoTracking();
        }

        public void InsertSecurityModule(PAYOUTsecurityModule securityModule)
        {
            _context.PAYOUTsecurityModules.Add(securityModule);
        }

        public void DeleteSecurityModule(int securityModuleId)
        {
            PAYOUTsecurityModule securityModule = _context.PAYOUTsecurityModules.Find(securityModuleId);
            _context.PAYOUTsecurityModules.Remove(securityModule);
        }

        public void UpdateStudent(PAYOUTsecurityModule securityModule)
        {
            _context.Entry(securityModule).State = EntityState.Modified;
        }
        #endregion
                
        public IQueryable<PAYOUTsecurityModuleControl> GetSecurityModuleControls()
        {
            return _context.PAYOUTsecurityModuleControls
                .AsNoTracking();
        }

        public IQueryable<PAYOUTsecurityControlAccess> GetSecurityControlAccesses()
        {
            return _context.PAYOUTsecurityControlAccesses
                .AsNoTracking();
        }

        public IQueryable<PAYOUTsecurityRoleModuleControl> GetSecurityRoleModuleControls()
        {
            return _context.PAYOUTsecurityRoleModuleControls
                .AsNoTracking();
        }

        public IQueryable<PAYOUTsecurityUserModuleControl> GetSecurityUserModuleControls()
        {
            return _context.PAYOUTsecurityUserModuleControls
                .AsNoTracking();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
