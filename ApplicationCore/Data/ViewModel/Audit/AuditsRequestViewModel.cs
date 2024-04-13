namespace Farsica.Template.Data.ViewModel.Audit
{
    using Farsica.Framework.Converter;
    using Farsica.Framework.Data;
    using Farsica.Framework.DataAccess.Audit;
    using Farsica.Framework.DataAnnotation;
    using Farsica.Framework.ModelBinding;

    using Microsoft.AspNetCore.Mvc;

    using System.Text.Json.Serialization;

    using static Farsica.Template.Common.Constants;

    public sealed class AuditsRequestViewModel
    {
        [Display]
        public int? UserId { get; set; }

        [Display]
        [ModelBinder(BinderType = typeof(DateTimeOffsetQueryStringModelBinder))]
        public DateTimeOffset? StartDate { get; set; }

        [Display]
        [ModelBinder(BinderType = typeof(DateTimeOffsetQueryStringModelBinder))]
        public DateTimeOffset? EndDate { get; set; }

        [Display]
        [ModelBinder(BinderType = typeof(EnumerationQueryStringModelBinder<AuditType, byte>))]
        public IList<AuditType>? AuditTypes { get; set; }

        [Display]
        [JsonConverter(typeof(EnumStringConverter<EntityType>))]
        public IEnumerable<EntityType>? EntityTypes { get; set; }

        [Display]
        public string? IdentifierId { get; set; }

        [Display]
        public PagingDto? PagingDto { get; set; } = new() { PageFilter = new() };
    }
}
