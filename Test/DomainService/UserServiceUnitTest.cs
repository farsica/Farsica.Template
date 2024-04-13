namespace Farsica.Template.Test.DomainService
{
    using Farsica.Template.Shared.Service;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable xUnit1041 // Fixture arguments to test classes must have fixture sources
    public class UserServiceUnitTest(IIdentityService identityService)
#pragma warning restore xUnit1041 // Fixture arguments to test classes must have fixture sources
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        [Fact]
        public async Task GetUsersAsync()
        {
            var response = await identityService.GetUsersAsync();

            Assert.Equal(Framework.Core.Constants.OperationResult.Succeeded, response.OperationResult);
        }
    }
}
