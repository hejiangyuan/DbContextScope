using System.Data.Entity;
using Numero3.EntityFramework.Interfaces;

namespace Numero3.EntityFramework.Demo.DatabaseContext
{
    public class UserManagementDbContextFactory : IDbContextFactory
    {
        private string _connName;

        public UserManagementDbContextFactory() : this("")
        {
        }

        public UserManagementDbContextFactory(string connName)
        {
            _connName = connName;
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            return new UserManagementDbContext(this._connName) as TDbContext;
        }
    }
}
