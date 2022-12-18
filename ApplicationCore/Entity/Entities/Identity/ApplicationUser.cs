namespace Farsica.Template.Entity.Entities.Identity
{
	using Farsica.Framework.Data;
	using Farsica.Framework.DataAccess.Entities;
	using Farsica.Framework.DataAnnotation;
	using Farsica.Framework.DataAnnotation.Schema;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	[Table(nameof(ApplicationUser))]
	public class ApplicationUser : IdentityUser<long>, IEntity<ApplicationUser, long>
	{
		public ApplicationUser()
		{
			UserRoles = new List<ApplicationUserRole>();
			UserLogins = new List<ApplicationUserLogin>();
			UserClaims = new List<ApplicationUserClaim>();
			UserTokens = new List<ApplicationUserToken>();
		}

		[System.ComponentModel.DataAnnotations.Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
		[Column(nameof(Id), DataType.Long)]
		public override long Id { get; set; }

		[StringLength(256)]
		[Required]
		[Column(nameof(UserName), DataType.UnicodeString)]
		public override string UserName { get; set; }

		[StringLength(256)]
		[Column(nameof(NormalizedUserName), DataType.UnicodeString)]
		public override string NormalizedUserName { get; set; }

		[StringLength(256)]
		[Column(nameof(Email), DataType.UnicodeString)]
		public override string Email { get; set; }

		[StringLength(256)]
		[Column(nameof(NormalizedEmail), DataType.UnicodeString)]
		public override string NormalizedEmail { get; set; }

		[Required]
		[Column(nameof(EmailConfirmed), DataType.Boolean)]
		public override bool EmailConfirmed { get; set; }

		[StringLength(512)]
		[Column(nameof(PasswordHash), DataType.UnicodeString)]
		public override string PasswordHash { get; set; }

		[StringLength(50)]
		[Column(nameof(SecurityStamp), DataType.String)]
		public override string SecurityStamp { get; set; }

		[StringLength(50)]
		[Column(nameof(ConcurrencyStamp), DataType.String)]
		public override string ConcurrencyStamp { get; set; }

		[StringLength(50)]
		[Column(nameof(PhoneNumber), DataType.String)]
		public override string PhoneNumber { get; set; }

		[Required]
		[Column(nameof(PhoneNumberConfirmed), DataType.Boolean)]
		public override bool PhoneNumberConfirmed { get; set; }

		[Required]
		[Column(nameof(TwoFactorEnabled), DataType.Boolean)]
		public override bool TwoFactorEnabled { get; set; }

		[Column(nameof(LockoutEnd), DataType.DateTimeOffset)]
		public override System.DateTimeOffset? LockoutEnd { get; set; }

		[Required]
		[Column(nameof(LockoutEnabled), DataType.Boolean)]
		public override bool LockoutEnabled { get; set; }

		[Required]
		[Column(nameof(AccessFailedCount), DataType.Int)]
		public override int AccessFailedCount { get; set; }

		[Column(nameof(RegistrationDate), DataType.DateTimeOffset)]
		public System.DateTimeOffset? RegistrationDate { get; set; }


		public virtual ICollection<ApplicationUserClaim> UserClaims { get; set; }
		public virtual ICollection<ApplicationUserLogin> UserLogins { get; set; }
		public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
		public virtual ICollection<ApplicationUserToken> UserTokens { get; set; }

		public void Configure([NotNull] EntityTypeBuilder<ApplicationUser> builder)
		{
			DbProviderFactory factory = DbProviderFactories.GetFactory;

			_ = builder.HasIndex(e => e.NormalizedEmail)
				.HasDatabaseName(factory.GetObjectName($"IX_{nameof(ApplicationUser)}_{nameof(NormalizedEmail)}"));

			_ = builder.HasIndex(e => e.NormalizedUserName)
				.HasDatabaseName(factory.GetObjectName($"IX_{nameof(ApplicationUser)}_{nameof(NormalizedUserName)}"))
				.IsUnique()
				.HasFilter($"([{factory.GetObjectName(nameof(NormalizedUserName))}] IS NOT NULL)");
		}
	}
}
