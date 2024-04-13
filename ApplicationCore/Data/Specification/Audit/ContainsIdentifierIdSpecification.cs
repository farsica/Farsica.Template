namespace Farsica.Template.Data.Specification.Audit
{
    using Farsica.Framework.DataAccess.Audit;
    using Farsica.Framework.DataAccess.Specification;

    using System;
    using System.Linq.Expressions;

    public sealed class ContainsIdentifierIdSpecification(string? identifierId) : SpecificationBase<Audit>
    {
        public override Expression<Func<Audit, bool>> Expression() => identifierId is null ? t => true : t => t.AuditEntries!.Any(u => identifierId == u.IdentifierId);
    }
}
