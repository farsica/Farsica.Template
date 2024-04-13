namespace Farsica.Template.Data.ViewModel.Identity
{
    using Farsica.Template.Data.Enumeration;

    using Farsica.Framework.Converter;

    using System.Text.Json.Serialization;

    public sealed class AuthenticationResponseViewModel
    {
        [JsonConverter(typeof(FlagsEnumerationConverter<Role>))]
        public Role? Roles { get; set; }
    }
}
