using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Numero3.EntityFramework.Demo.DomainModel;

namespace Numero3.EntityFramework.Demo.Repositories
{
	public interface IUserRepository 
	{
		User Get(Guid userId);
		Task<User> GetAsync(Guid userId);
		void Add(User user);
        IEnumerable<User> GetList(string whereClause);
	}
}