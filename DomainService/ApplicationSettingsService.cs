namespace Farsica.Template.DomainService
{
    using Farsica.Template.Data.Dto.ApplicationSettings;
    using Farsica.Template.Data.Entity;
    using Farsica.Template.Shared.Service;

    using Farsica.Framework.Caching;
    using Farsica.Framework.Core;
    using Farsica.Framework.Data;
    using Farsica.Framework.DataAccess.UnitOfWork;
    using Farsica.Framework.Service;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Threading.Tasks;

    using Constants = Framework.Core.Constants;

    public class ApplicationSettingsService(Lazy<IUnitOfWorkProvider> unitOfWorkProvider, Lazy<IHttpContextAccessor> httpContextAccessor, Lazy<ILogger<ApplicationSettingsService>> logger, Lazy<ICacheProvider> cacheProvider)
        : ServiceBase<ApplicationSettingsService>(unitOfWorkProvider, httpContextAccessor, logger), IApplicationSettingsService
    {
        private const string ApplicationSettingsCacheKey = "ApplicationSettings";

        public async Task<ResultData<ApplicationSettingsDto>> GetApplicationSettingsAsync()
        {
            try
            {
                var settings = await cacheProvider.Value.GetAsync<ApplicationSettingsDto?>(ApplicationSettingsCacheKey, async () =>
                {
                    var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                    var lst = await uow.GetRepository<ApplicationSettings, string>().GetManyQueryable().Select(t => new
                    {
                        t.Id,
                        t.Value,
                    }).ToListAsync();
                    ApplicationSettingsDto dto = new();
                    var properties = typeof(ApplicationSettingsDto).GetProperties();
                    for (var i = 0; i < properties.Length; i++)
                    {
                        var stringValue = lst.Find(t => t.Id.Equals(properties[i].Name, StringComparison.OrdinalIgnoreCase))?.Value;
                        if (string.IsNullOrEmpty(stringValue))
                        {
                            continue;
                        }

                        if (properties[i].PropertyType == typeof(IList<string>))
                        {
                            var values = stringValue.Split(Constants.DelimiterAlternate, StringSplitOptions.RemoveEmptyEntries);
                            properties[i].SetValue(dto, values.ToList());
                        }
                        else
                        {
                            var value = stringValue.ValueOf(properties[i].PropertyType, default);
                            properties[i].SetValue(dto, value);
                        }
                    }

                    return dto;
                });

                return new(Constants.OperationResult.Succeeded) { Data = settings };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(Constants.OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message } } };
            }
        }

        public async Task<ResultData<bool>> ModifyApplicationSettingsAsync(ApplicationSettingsDto settingsDto)
        {
            try
            {
                var uow = UnitOfWorkProvider.Value.CreateUnitOfWork();
                var repository = uow.GetRepository<ApplicationSettings, string>();
                var lst = await repository.GetManyQueryable(tracking: true).ToListAsync();

                var properties = typeof(ApplicationSettingsDto).GetProperties();
                for (var i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(settingsDto)?.ToString();
                    if (properties[i].PropertyType == typeof(IList<string>))
                    {
                        var listValues = properties[i].GetValue(settingsDto) as IList<string>;
                        value = string.Join(Constants.DelimiterAlternate, listValues!.Select(t => t.ToString()));
                    }

                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    var item = lst.Find(t => t.Id == properties[i].Name);
                    if (item is null)
                    {
                        repository.Add(new ApplicationSettings { Id = properties[i].Name, Value = value, });
                    }
                    else
                    {
                        item.Value = value;
                        _ = repository.Update(item);
                    }
                }
                for (var i = 0; i < lst.Count; i++)
                {
                    if (Array.TrueForAll(properties, t => t.Name != lst[i].Id))
                    {
                        repository.Remove(lst[i]);
                    }
                }

                _ = await uow.SaveChangesAsync();
                await cacheProvider.Value.SetAsync(ApplicationSettingsCacheKey, settingsDto);

                return new(Constants.OperationResult.Succeeded) { Data = true };
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);
                return new(Constants.OperationResult.Failed) { Errors = new[] { new Error { Message = exc.Message } } };
            }
        }
    }
}
