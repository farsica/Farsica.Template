namespace Farsica.Template.Data.Specification.Audit
{
    using Farsica.Framework.DataAccess.Audit;
    using Farsica.Framework.DataAccess.Specification;

    using System.Linq.Expressions;

    public sealed class ContainsAuditTypeSpecification(AuditType? auditType) : SpecificationBase<Audit>
    {
        public override Expression<Func<Audit, bool>> Expression() => auditType is null ? t => true : t => t.AuditEntries!.Any(u => auditType.Equals(u.AuditType));
    }
}
