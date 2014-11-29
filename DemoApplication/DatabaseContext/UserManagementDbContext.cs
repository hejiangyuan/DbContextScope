using System.Data.Entity;
using System.Reflection;
using Numero3.EntityFramework.Demo.DomainModel;

namespace Numero3.EntityFramework.Demo.DatabaseContext
{
	public class UserManagementDbContext : DbContext
	{
		// Map our 'User' model by convention
		public DbSet<User> Users { get; set; }

        public UserManagementDbContext(string nameOrConnectionString) : 
            base(nameOrConnectionString)
		{}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			// Overrides for the convention-based mappings.
			// We're assuming that all our fluent mappings are declared in this assembly.
			modelBuilder.Configurations.AddFromAssembly(Assembly.GetAssembly(typeof(UserManagementDbContext)));
		}
	}
}
