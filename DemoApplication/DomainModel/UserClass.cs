using System;

namespace Numero3.EntityFramework.Demo.DomainModel
{
	// Anemic model to keep this demo application simple.
    public class UserClass
	{
		public Guid Id { get; set; }
		public string UserName { get; set; }
		public string ClassName { get; set; }

		public override string ToString()
		{
            return String.Format("Id: {0} | UserName: {1} | ClassName: {2} ", Id, UserName, ClassName);
		}
	}
}
