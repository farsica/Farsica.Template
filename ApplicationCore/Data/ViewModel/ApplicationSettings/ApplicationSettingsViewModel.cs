namespace Farsica.Template.Data.ViewModel.ApplicationSettings
{
    using Farsica.Framework.DataAnnotation;

    public sealed class ApplicationSettingsViewModel
    {
        #region General

        [Display]
        public int GridPageSize { get; set; } = 10;

        [Display]
        public bool SwaggerEnabled { get; set; }

        [Display]
        public string? DefaultTimeZoneId { get; set; }

        #endregion
    }
}
