using System.Data.Entity;
using System.Reflection;
using Numero3.EntityFramework.Demo.DatabaseContext;
using Numero3.EntityFramework.Demo.DomainModel;

namespace Numero3.EntityFramework.Demo.OtherDatabaseContext
{
	public class UserClassManagementDbContext : DbContext
	{
		// Map our 'User' model by convention
        public DbSet<UserClass> UserClass { get; set; }

        public UserClassManagementDbContext(string nameOrConnectionString) :
	        base(nameOrConnectionString)
	    {
	        
	    }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Overrides for the convention-based mappings.
			// We're assuming that all our fluent mappings are declared in this assembly.
			modelBuilder.Configurations.AddFromAssembly(Assembly.GetAssembly(typeof(UserManagementDbContext)));
		}
	}
}
