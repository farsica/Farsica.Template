namespace Farsica.Template.Shared.Service
{
    using Farsica.Template.Data.Dto.ApplicationSettings;

    using Farsica.Framework.Data;
    using Farsica.Framework.DataAnnotation;

    [Injectable]
    public interface IApplicationSettingsService
    {
        Task<ResultData<ApplicationSettingsDto>> GetApplicationSettingsAsync();

        Task<ResultData<bool>> ModifyApplicationSettingsAsync(ApplicationSettingsDto settingsDto);
    }
}
