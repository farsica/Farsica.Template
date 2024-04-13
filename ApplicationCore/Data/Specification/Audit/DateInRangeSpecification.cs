namespace Farsica.Template.Data.Specification.Audit
{
    using Farsica.Framework.DataAccess.Audit;
    using Farsica.Framework.DataAccess.Specification;

    using System;
    using System.Linq.Expressions;

    public sealed class DateInRangeSpecification(DateTimeOffset? startDate, DateTimeOffset? endDate) : SpecificationBase<Audit>
    {
        private readonly DateTimeOffset startDate = startDate ?? DateTimeOffset.MinValue;
        private readonly DateTimeOffset endDate = endDate ?? DateTimeOffset.MaxValue;

        public override Expression<Func<Audit, bool>> Expression() => t => t.Date >= startDate && t.Date < endDate;
    }
}
