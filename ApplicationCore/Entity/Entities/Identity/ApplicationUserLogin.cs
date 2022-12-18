namespace Farsica.Template.Entity.Entities.Identity
{
	using Farsica.Framework.Data;
	using Farsica.Framework.DataAccess.Entities;
	using Farsica.Framework.DataAnnotation;
	using Farsica.Framework.DataAnnotation.Schema;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using System.Diagnostics.CodeAnalysis;

	[Table(nameof(ApplicationUserLogin))]
	public class ApplicationUserLogin : IdentityUserLogin<long>, IEntity<ApplicationUserLogin, long>
	{
		[StringLength(128)]
		[Required]
		[Column(nameof(LoginProvider), DataType.UnicodeString)]
		public override string LoginProvider { get; set; }

		[StringLength(128)]
		[Required]
		[Column(nameof(ProviderKey), DataType.UnicodeString)]
		public override string ProviderKey { get; set; }

		[StringLength(128)]
		[Column(nameof(ProviderDisplayName), DataType.UnicodeString)]
		public override string ProviderDisplayName { get; set; }

		[Required]
		[Column(nameof(UserId), DataType.Long)]
		public override long UserId { get; set; }

		public virtual ApplicationUser User { get; set; }

		long IEntity<ApplicationUserLogin, long>.Id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public void Configure([NotNull] EntityTypeBuilder<ApplicationUserLogin> builder)
		{
			_ = builder.HasIndex(t => t.UserId)
				.HasDatabaseName(DbProviderFactories.GetFactory.GetObjectName($"IX_{nameof(ApplicationUserLogin)}_{nameof(UserId)}"));


			_ = builder.HasOne(t => t.User)
				.WithMany(t => t.UserLogins)
				.HasForeignKey(t => t.UserId);

			_ = builder.HasKey(t => new { t.LoginProvider, t.ProviderKey });
		}
	}
}