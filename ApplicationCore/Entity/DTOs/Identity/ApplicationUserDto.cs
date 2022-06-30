namespace Farsica.Template.Entity.Entities.Identity
{
    public class ApplicationUserDto : Farsica.Framework.Mapping.IRegister
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public System.DateTimeOffset? RegistrationDate { get; set; }

        public void Register(Farsica.Framework.Mapping.TypeAdapterConfig config)
        {
            config.ForType<ApplicationUser, ApplicationUserDto>();
        }
    }
}
