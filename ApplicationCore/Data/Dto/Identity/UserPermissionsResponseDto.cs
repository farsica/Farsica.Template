namespace Farsica.Template.Data.Dto.Identity
{
    using Farsica.Template.Data.Enumeration;

    public sealed class UserPermissionsResponseDto
    {
        public IEnumerable<string?>? Claims { get; set; }

        public Role? Roles { get; set; }
    }
}
