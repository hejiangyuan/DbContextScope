using System.Data.Entity;
using Numero3.EntityFramework.Interfaces;

namespace Numero3.EntityFramework.Demo.DatabaseContext
{
    public class UserManagementDbContextFactory : IDbContextFactory
    {
        private string _nameOrConnectionString;

        public UserManagementDbContextFactory() : this("")
        {
        }

        public UserManagementDbContextFactory(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            return new UserManagementDbContext(this._nameOrConnectionString) as TDbContext;
        }
    }
}
