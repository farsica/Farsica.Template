namespace Farsica.Template.Data.ViewModel.Identity
{
    using Farsica.Framework.DataAnnotation;

    public class ProfileSettingsRequestViewModel
    {
        [Display]
        [TimeZoneId]
        public string? TimeZoneId { get; set; }
    }
}
