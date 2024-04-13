namespace Farsica.Template.Data.Specification.Identity
{
    using Farsica.Template.Data.Entity.Identity;

    using Farsica.Framework.DataAccess.Specification;

    using System.Linq.Expressions;

    public sealed class ClaimTypeEqualsSpecification(string claimType) : SpecificationBase<ApplicationUserClaim>
    {
        public override Expression<Func<ApplicationUserClaim, bool>> Expression() => (t) => t.ClaimType == claimType;
    }
}
