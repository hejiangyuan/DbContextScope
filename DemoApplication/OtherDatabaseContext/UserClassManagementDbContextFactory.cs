using System.Data.Entity;
using Numero3.EntityFramework.Interfaces;

namespace Numero3.EntityFramework.Demo.OtherDatabaseContext
{
    public class UserClassManagementDbContextFactory : IDbContextFactory
    {
        private string _nameOrConnectionString;

        public UserClassManagementDbContextFactory() : this("db1")
        {
        }

        public UserClassManagementDbContextFactory(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            return new UserClassManagementDbContext(this._nameOrConnectionString) as TDbContext;
        }
    }
}
