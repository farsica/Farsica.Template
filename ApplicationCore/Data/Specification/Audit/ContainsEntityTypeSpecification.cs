namespace Farsica.Template.Data.Specification.Audit
{
    using Farsica.Template.Common;

    using Farsica.Framework.DataAccess.Audit;
    using Farsica.Framework.DataAccess.Specification;

    using System;
    using System.Linq.Expressions;

    public class ContainsEntityTypeSpecification(IEnumerable<Constants.EntityType>? entityTypes) : SpecificationBase<Audit>
    {
        private readonly List<int>? entityTypes = entityTypes?.Select(t => (int)t).ToList();

        public override Expression<Func<Audit, bool>> Expression() => entityTypes is null ? t => true : t => t.AuditEntries!.Any(e => entityTypes.Contains(e.EntityType));
    }
}
