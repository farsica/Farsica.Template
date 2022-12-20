namespace Farsica.Template.Data.Entity.Identity
{
	using Farsica.Framework.Data;
	using Farsica.Framework.DataAccess.Entities;
	using Farsica.Framework.DataAnnotation;
	using Farsica.Framework.DataAnnotation.Schema;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using System.Diagnostics.CodeAnalysis;

	[Table(nameof(ApplicationUserClaim))]
	public class ApplicationUserClaim : IdentityUserClaim<long>, IEntity<ApplicationUserClaim, int>
	{
		[System.ComponentModel.DataAnnotations.Key]
		[Column(nameof(Id), DataType.Int)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public override int Id { get; set; }

		[Required]
		[Column(nameof(UserId), DataType.Long)]
		public override long UserId { get; set; }

		[StringLength(1000)]
		[Column(nameof(ClaimType), DataType.String)]
		public override string ClaimType { get; set; }

		[StringLength(1000)]
		[Column(nameof(ClaimValue), DataType.UnicodeString)]
		public override string ClaimValue { get; set; }

		public virtual ApplicationUser User { get; set; }

		public void Configure([NotNull] EntityTypeBuilder<ApplicationUserClaim> builder)
		{
			_ = builder.HasIndex(t => t.UserId)
				.HasDatabaseName(DbProviderFactories.GetFactory.GetObjectName($"IX_{nameof(ApplicationUserClaim)}_{nameof(UserId)}"));

			_ = builder.HasOne(t => t.User)
				.WithMany(t => t.UserClaims)
				.HasForeignKey(t => t.UserId);
		}
	}
}