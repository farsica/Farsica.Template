namespace Farsica.Template.Data.Specification.Audit
{
    using Farsica.Framework.DataAccess.Audit;
    using Farsica.Framework.DataAccess.Specification;

    using System;
    using System.Linq.Expressions;

    public sealed class UserIdEqualsSpecification(int? userId) : SpecificationBase<Audit>
    {
        public override Expression<Func<Audit, bool>> Expression() => userId is null ? t => true : t => userId.Value.ToString() == t.UserId;
    }
}
