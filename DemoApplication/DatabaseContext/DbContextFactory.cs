using System;
using System.Data.Entity;
using Numero3.EntityFramework.Interfaces;

namespace Numero3.EntityFramework.Demo.DatabaseContext
{
    public class DbContextFactory : IDbContextFactory
    {
        private string _nameOrConnectionString;

        public DbContextFactory(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            return Activator.CreateInstance(typeof(TDbContext), _nameOrConnectionString) as TDbContext;
        }
    }
}
