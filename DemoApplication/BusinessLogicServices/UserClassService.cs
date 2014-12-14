using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numero3.EntityFramework.Demo.DomainModel;
using Numero3.EntityFramework.Demo.OtherDatabaseContext;
using Numero3.EntityFramework.Interfaces;

namespace Numero3.EntityFramework.Demo.BusinessLogicServices
{
    public class UserClassService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        public UserClassService(IDbContextScopeFactory dbContextScopeFactory)
		{
			_dbContextScopeFactory = dbContextScopeFactory;
		}

		public void CreateUserClass(string userName, string className)
		{
			/*
			 * Typical usage of DbContextScope for a read-write business transaction. 
			 * It's as simple as it looks.
			 */
			using (var dbContextScope = _dbContextScopeFactory.Create())
			{

				//-- Build domain model
				var userClass = new UserClass()
				           {
							   Id = Guid.NewGuid(),
							   UserName = userName,
                               ClassName = className
				           };

                //dbContextScope.Users.Add(user);

			    dbContextScope.DbContexts.Get<UserClassManagementDbContext>().UserClass.Add(userClass);

				dbContextScope.SaveChanges();
			}
		}

        public int GetRowCount()
        {
            using (var dbContextScope = _dbContextScopeFactory.Create())
            {
                return dbContextScope.DbContexts.Get<UserClassManagementDbContext>().UserClass.Count();
            }
        }
    }
}
