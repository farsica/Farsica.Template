namespace Farsica.Template.Entity.Entities.Identity
{
	using Farsica.Framework.Data;
	using Farsica.Framework.DataAccess.Entities;
	using Farsica.Framework.DataAnnotation;
	using Farsica.Framework.DataAnnotation.Schema;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using System.Diagnostics.CodeAnalysis;

	[Table(nameof(ApplicationUserToken))]
	public class ApplicationUserToken : IdentityUserToken<long>, IEntity<ApplicationUserToken, long>
	{
		[Required]
		[Column(nameof(UserId), DataType.Long)]
		public override long UserId { get; set; }

		[StringLength(128)]
		[Required]
		[Column(nameof(LoginProvider), DataType.UnicodeString)]
		public override string LoginProvider { get; set; }

		[StringLength(128)]
		[Required]
		[Column(nameof(Name), DataType.UnicodeString)]
		public override string Name { get; set; }

		[StringLength(2500)]
		[Column(nameof(Value), DataType.UnicodeString)]
		public override string Value { get; set; }

		public virtual ApplicationUser User { get; set; }

		long IEntity<ApplicationUserToken, long>.Id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public void Configure([NotNull] EntityTypeBuilder<ApplicationUserToken> builder)
		{
			_ = builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

			_ = builder.HasOne(t => t.User)
				.WithMany(t => t.UserTokens)
				.HasForeignKey(t => t.UserId);
		}
	}
}