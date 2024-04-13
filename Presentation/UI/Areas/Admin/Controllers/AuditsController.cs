namespace Farsica.Template.UI.Web.Areas.Admin.Controllers
{
    using Farsica.Template.Data.Enumeration;
    using Farsica.Template.Data.Specification.Audit;
    using Farsica.Template.Data.ViewModel.Audit;

    using Asp.Versioning;

    using Farsica.Framework.Core;
    using Farsica.Framework.Data;
    using Farsica.Framework.DataAccess.Audit;
    using Farsica.Framework.DataAccess.Specification;
    using Farsica.Framework.DataAccess.Specification.Impl;
    using Farsica.Framework.DataAnnotation;
    using Farsica.Framework.Identity;

    using Microsoft.AspNetCore.Mvc;

    using NUlid;

    using System.Diagnostics.CodeAnalysis;

    using static Farsica.Template.Common.Constants;

    [Framework.DataAnnotation.Area(nameof(Role.Admin), "مدیریت")]
    [Route("api/v{version:apiVersion}/[area]/[controller]")]
    [ApiVersion("1.0")]
    [Permission(Roles = [nameof(Role.Admin)])]
    [Display(Name = "رخداد ها")]
    public class AuditsController(Lazy<ILogger<AuditsController>> logger, Lazy<IAuditService> auditService) : ApiControllerBase<AuditsController>(logger)
    {
        [HttpGet, Produces(typeof(ApiResponse<ListDataSource<AuditListResponseViewModel>>))]
        [Display(Name = "لیست رخداد ها")]
        public async Task<IActionResult> GetAudits([NotNull, FromQuery] AuditsRequestViewModel request)
        {
            try
            {
                var specification = new UserIdEqualsSpecification(request.UserId)
                    .And(new DateInRangeSpecification(request.StartDate, request.EndDate))
                    .And(new ContainsIdentifierIdSpecification(request.IdentifierId))
                    .And(new ContainsEntityTypeSpecification(request.EntityTypes));

                if (request.AuditTypes is not null && request.AuditTypes.Count > 0)
                {
                    ISpecification<Audit> auditTypeSpecification = new ContainsAuditTypeSpecification(request.AuditTypes[0]);
                    for (var i = 1; i < request.AuditTypes.Count; i++)
                    {
                        auditTypeSpecification = auditTypeSpecification.Or(new ContainsAuditTypeSpecification(request.AuditTypes[i]));
                    }
                    specification = specification.And(auditTypeSpecification);
                }

                var result = await auditService.Value.GetAuditsAsync(new ListRequestDto<Audit> { PagingDto = request.PagingDto, Specification = specification });
                return Ok(new ApiResponse<ListDataSource<AuditListResponseViewModel>>
                {
                    Errors = result.Errors,
                    Data = result.Data.List is null ? new() : new ListDataSource<AuditListResponseViewModel>
                    {
                        List = result.Data.List.Select(t => new AuditListResponseViewModel
                        {
                            Id = t.Id,
                            User = t.User,
                            Date = t.Date.DateTime,
                            IpAddress = t.IpAddress,
                        }),
                        TotalRecordsCount = result.Data.TotalRecordsCount,
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<ListDataSource<AuditListResponseViewModel>> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }

        [HttpGet("{auditId}"), Produces(typeof(ApiResponse<AuditResponseViewModel>))]
        [Display(Name = "جزئیات رخداد")]
        public async Task<IActionResult> Get([FromRoute] Ulid auditId)
        {
            try
            {
                var result = await auditService.Value.Get(new IdEqualsSpecification<Audit, Ulid>(auditId));
                return Ok(new ApiResponse<AuditResponseViewModel>
                {
                    Errors = result.Errors,
                    Data = result.Data is null ? null : new AuditResponseViewModel
                    {
                        User = result.Data.User,
                        Date = result.Data.Date.DateTime,
                        IpAddress = result.Data.IpAddress,
                        UserAgent = result.Data.UserAgent,
                        AuditEntries = result.Data.AuditEntries?.Select(t => new AuditEntryViewModel
                        {
                            AuditType = t.AuditType,
                            EntityType = (EntityType)t.EntityType,
                            IdentifierId = t.IdentifierId,
                            AuditEntryProperties = t.AuditEntryProperties?.Select(u => new AuditEntryPropertyViewModel
                            {
                                PropertyName = u.PropertyName,
                                OldValue = u.OldValue,
                                NewValue = u.NewValue,
                            })
                        })
                    }
                });
            }
            catch (Exception exc)
            {
                Logger.Value.LogException(exc);

                return Ok(new ApiResponse<AuditResponseViewModel> { Errors = new[] { new Error { Message = exc.Message } } });
            }
        }
    }
}
