namespace Farsica.Template.Data.ViewModel.Audit
{
    using Farsica.Framework.Converter;
    using Farsica.Framework.DataAccess.Audit;

    using System.Text.Json.Serialization;

    using static Farsica.Template.Common.Constants;

    public sealed class AuditEntryViewModel
    {
        [JsonConverter(typeof(EnumerationConverter<AuditType, byte>))]
        public AuditType? AuditType { get; set; }

        [JsonConverter(typeof(EnumStringConverter<EntityType>))]
        public EntityType EntityType { get; set; }

        public string? IdentifierId { get; set; }

        public IEnumerable<AuditEntryPropertyViewModel>? AuditEntryProperties { get; set; }
    }
}
