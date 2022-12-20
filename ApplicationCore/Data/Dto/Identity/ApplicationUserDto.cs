namespace Farsica.Template.Data.Entity.Identity
{
	using System.Diagnostics.CodeAnalysis;

	public class ApplicationUserDto : Framework.Mapping.IRegister
	{
		public int Id { get; set; }

		public string UserName { get; set; }

		public string Email { get; set; }

		public bool EmailConfirmed { get; set; }

		public string PhoneNumber { get; set; }

		public bool PhoneNumberConfirmed { get; set; }

		public System.DateTimeOffset? RegistrationDate { get; set; }

		public void Register([NotNull] Framework.Mapping.TypeAdapterConfig config)
		{
			_ = config.ForType<ApplicationUser, ApplicationUserDto>();
		}
	}
}
