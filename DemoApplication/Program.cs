using System;
using System.Linq;
using System.Transactions;
using Numero3.EntityFramework.Demo.BusinessLogicServices;
using Numero3.EntityFramework.Demo.CommandModel;
using Numero3.EntityFramework.Demo.DatabaseContext;
using Numero3.EntityFramework.Demo.DomainModel;
using Numero3.EntityFramework.Demo.OtherDatabaseContext;
using Numero3.EntityFramework.Demo.Repositories;
using Numero3.EntityFramework.Implementation;

namespace Numero3.EntityFramework.Demo
{
	class Program
	{
		static void Main(string[] args)
		{

			//-- Poor-man DI - build our dependencies by hand for this demo

            //var dbcontextFactory = new UserManagementDbContextFactory("data source=192.168.0.11;initial catalog=DbContextScopeDemo1;persist security info=True;user id=test;password=111111;MultipleActiveResultSets=True;App=EntityFramework");
            var dbcontextFactory = new DbContextFactory("server=127.0.0.1;port=3306;AutoEnlist=true;user id=root;password=jooge2012;persistsecurityinfo=True;database=test;oldguids=True");

            var dbContextScopeFactory = new DbContextScopeFactory(dbcontextFactory);

			var ambientDbContextLocator = new AmbientDbContextLocator();
			var userRepository = new UserRepository(ambientDbContextLocator);

			var userCreationService = new UserCreationService(dbContextScopeFactory, userRepository);
			var userQueryService = new UserQueryService(dbContextScopeFactory, userRepository);
			var userEmailService = new UserEmailService(dbContextScopeFactory);
			var userCreditScoreService = new UserCreditScoreService(dbContextScopeFactory);

            var userClassService = new UserClassService(dbContextScopeFactory);

			try
			{
				Console.WriteLine("This demo application will create a database named DbContextScopeDemo in the default SQL Server instance on localhost. Edit the connection string in UserManagementDbContext if you'd like to create it somewhere else.");
				Console.WriteLine("Press enter to start...");
				Console.ReadLine();


                Console.WriteLine("一个dbScope中使用2个DbContext 创建 a user and userClass Terry+English...");

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Terry",
                    Email = "Terry@jooge.net",
                    WelcomeEmailSent = false,
                    CreatedOn = DateTime.UtcNow
                };

                using (var dbContextScope = dbContextScopeFactory.Create())
                {
                    dbContextScope.DbContexts.Get<UserManagementDbContext>().Users.Add(user);

                    userClassService.CreateUserClass(user.Name, "English");

                    dbContextScope.SaveChanges();
                }

                Console.WriteLine("检查数据库应该有相应的记录user表 和 userClass表");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();


                Console.WriteLine("2个dbcontext 使用事务的情况，");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();

                user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Mark",
                    Email = "Mark@jooge.net",
                    WelcomeEmailSent = false,
                    CreatedOn = DateTime.UtcNow
                };


			    int rowCount = 0;
			    try
			    {
			        using (var s = new TransactionScope())
			        {
			            using (var dbContextScope = dbContextScopeFactory.Create())
			            {

                            dbContextScope.DbContexts.Get<UserManagementDbContext>().Users.Add(user);
                            dbContextScope.SaveChanges();
			            }

			            rowCount = userClassService.GetRowCount();
			            userClassService.CreateUserClass(user.Name, "English");

                        throw new Exception("模拟错误");

			            s.Complete();
			        }
			    }
			    catch
			    {
			        
			    }

                var tempUsers = userQueryService.GetUsers(user.Id);
                Console.WriteLine("Found {0} persisted users. If this number is 0, we're all good. If this number is not 0, we have a big problem.", tempUsers.Count());

			    if (rowCount == userClassService.GetRowCount())
			    {
                    Console.WriteLine("userClass表的数量没有变化，是正确的，如果变化了就不对了");
			    }
			    else
			    {
                    Console.WriteLine("userClass表的数量变化了，是不对的。");
			    }

			    
				//-- Demo of typical usage for read and writes
				Console.WriteLine("Creating a user called Mary...");
				var marysSpec = new UserCreationSpec("Mary", "mary@example.com");
				userCreationService.CreateUser(marysSpec);
				Console.WriteLine("Done.\n");

				Console.WriteLine("Trying to retrieve our newly created user from the data store...");
				var mary = userQueryService.GetUser(marysSpec.Id);
				Console.WriteLine("OK. Persisted user: {0}", mary);

				Console.WriteLine("Press enter to continue...");
				Console.ReadLine();

				//-- Demo of nested DbContextScopes
				Console.WriteLine("Creating 2 new users called John and Jeanne in an atomic transaction...");
				var johnSpec = new UserCreationSpec("John", "john@example.com");
				var jeanneSpec = new UserCreationSpec("Jeanne", "jeanne@example.com");
				userCreationService.CreateListOfUsers(johnSpec, jeanneSpec);
				Console.WriteLine("Done.\n");

				Console.WriteLine("Trying to retrieve our newly created users from the data store...");
				var createdUsers = userQueryService.GetUsers(johnSpec.Id, jeanneSpec.Id);
				Console.WriteLine("OK. Found {0} persisted users.", createdUsers.Count());

				Console.WriteLine("Press enter to continue...");
				Console.ReadLine();

				//-- Demo of nested DbContextScopes in the face of an exception. 
				// If any of the provided users failed to get persisted, none should get persisted. 
				Console.WriteLine("Creating 2 new users called Julie and Marc in an atomic transaction. Will make the persistence of the second user fail intentionally in order to test the atomicity of the transaction...");
				var julieSpec = new UserCreationSpec("Julie", "julie@example.com");
				var marcSpec = new UserCreationSpec("Marc", "marc@example.com");
				try
				{
					userCreationService.CreateListOfUsersWithIntentionalFailure(julieSpec, marcSpec);
					Console.WriteLine("Done.\n");
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					Console.WriteLine();
				}

				Console.WriteLine("Trying to retrieve our newly created users from the data store...");
				var maybeCreatedUsers = userQueryService.GetUsers(julieSpec.Id, marcSpec.Id);
				Console.WriteLine("Found {0} persisted users. If this number is 0, we're all good. If this number is not 0, we have a big problem.", maybeCreatedUsers.Count());

				Console.WriteLine("Press enter to continue...");
				Console.ReadLine();

				//-- Demo of DbContextScope within an async flow
				Console.WriteLine("Trying to retrieve two users John and Jeanne sequentially in an asynchronous manner...");
				// We're going to block on the async task here as we don't have a choice. No risk of deadlocking in any case as console apps
				// don't have a synchronization context.
				var usersFoundAsync = userQueryService.GetTwoUsersAsync(johnSpec.Id, jeanneSpec.Id).Result;
				Console.WriteLine("OK. Found {0} persisted users.", usersFoundAsync.Count());

				Console.WriteLine("Press enter to continue...");
				Console.ReadLine();

				//-- Demo of explicit database transaction. 
				Console.WriteLine("Trying to retrieve user John within a READ UNCOMMITTED database transaction...");
				// You'll want to use SQL Profiler or Entity Framework Profiler to verify that the correct transaction isolation
				// level is being used.
				var userMaybeUncommitted = userQueryService.GetUserUncommitted(johnSpec.Id);
				Console.WriteLine("OK. User found: {0}", userMaybeUncommitted);

				Console.WriteLine("Press enter to continue...");
				Console.ReadLine();

				//-- Demo of disabling the DbContextScope nesting behaviour in order to force the persistence of changes made to entities
				// This is a pretty advanced feature that you can safely ignore until you actually need it.
				Console.WriteLine("Will simulate sending a Welcome email to John...");

				using (var parentScope = dbContextScopeFactory.Create())
				{
					var parentDbContext = parentScope.DbContexts.Get<UserManagementDbContext>();

					// Load John in the parent DbContext
					var john = parentDbContext.Users.Find(johnSpec.Id);
					Console.WriteLine("Before calling SendWelcomeEmail(), john.WelcomeEmailSent = " + john.WelcomeEmailSent);

					// Now call our SendWelcomeEmail() business logic service method, which will
					// update John in a non-nested child context
					userEmailService.SendWelcomeEmail(johnSpec.Id);

					// Verify that we can see the modifications made to John by the SendWelcomeEmail() method
					Console.WriteLine("After calling SendWelcomeEmail(), john.WelcomeEmailSent = " + john.WelcomeEmailSent);

					// Note that even though we're not calling SaveChanges() in the parent scope here, the changes
					// made to John by SendWelcomeEmail() will remain persisted in the database as SendWelcomeEmail()
					// forced the creation of a new DbContextScope.
				}

				Console.WriteLine("Press enter to continue...");
				Console.ReadLine();

				//-- Demonstration of DbContextScope and parallel programming
				Console.WriteLine("Calculating and storing the credit score of all users in the database in parallel...");
				userCreditScoreService.UpdateCreditScoreForAllUsers();
				Console.WriteLine("Done.");

                Console.WriteLine("Trying to retrieve users by where clause.");
                var users = userQueryService.GetUserByWhereClause(" WelcomeEmailSent = 1 ");
                Console.WriteLine("OK. WelcomeEmailSent user count: {0}", users.Count());


                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();


			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			Console.WriteLine();
			Console.WriteLine("The end.");
			Console.WriteLine("Press enter to exit...");
			Console.ReadLine();
		}
	}
}
