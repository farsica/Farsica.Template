namespace Farsica.Template.Test.DomainService
{
	using Farsica.Template.Shared.Service;

	public class UserServiceUnitTest
	{
		private readonly IUserService userService;

		public UserServiceUnitTest(IUserService userService)
		{
			this.userService = userService;
		}

		[Fact]
		public async Task GetUsers()
		{
			var response = await userService.GetUsers();

			Assert.True(response.OperationResult == Framework.Core.Constants.OperationResult.Succeeded);
			Assert.NotNull(response.Data);
		}
	}
}