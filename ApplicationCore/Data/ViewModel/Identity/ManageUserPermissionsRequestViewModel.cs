namespace Farsica.Template.Data.ViewModel.Identity
{
    using Farsica.Template.Data.Enumeration;

    using Farsica.Framework.Converter;
    using Farsica.Framework.DataAnnotation;

    using System.Text.Json.Serialization;

    public sealed class ManageUserPermissionsRequestViewModel
    {
        [Display]
        public IEnumerable<string>? Claims { get; set; }

        [Display]
        [JsonConverter(typeof(FlagsEnumerationConverter<Role>))]
        public Role? Roles { get; set; }
    }
}
