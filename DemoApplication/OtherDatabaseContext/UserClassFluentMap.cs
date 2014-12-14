using System.Data.Entity.ModelConfiguration;
using Numero3.EntityFramework.Demo.DomainModel;

namespace Numero3.EntityFramework.Demo.OtherDatabaseContext
{
	/// <summary>
	/// Defines the convention-based mapping overrides for the User model. 
	/// </summary>
    public class UserClassFluentMap : EntityTypeConfiguration<UserClass>
	{
        public UserClassFluentMap()
        {
            ToTable("UserClass");

			Property(m => m.UserName).IsRequired();
			Property(m => m.ClassName).IsRequired();
		}
	}
}
