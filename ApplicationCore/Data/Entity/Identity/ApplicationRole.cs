namespace Farsica.Template.Data.Entity.Identity
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using Farsica.Framework.Data;
	using Farsica.Framework.DataAccess.Entities;
	using Farsica.Framework.DataAnnotation;
	using Farsica.Framework.DataAnnotation.Schema;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;

	[Table(nameof(ApplicationRole))]
	public class ApplicationRole : IdentityRole<long>, IEntity<ApplicationRole, long>
	{
		public ApplicationRole()
		{
			UserRoles = new List<ApplicationUserRole>();
			RoleClaims = new List<ApplicationRoleClaim>();
		}

		[System.ComponentModel.DataAnnotations.Key]
		[Column(nameof(Id), DataType.Long)]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public override long Id { get; set; }

		[StringLength(256)]
		[Column(nameof(Name), DataType.UnicodeString)]
		[Required]
		public override string Name { get; set; }

		[Column(nameof(NormalizedName), DataType.UnicodeString)]
		[StringLength(256)]
		public override string NormalizedName { get; set; }

		[Column(nameof(ConcurrencyStamp), DataType.String)]
		[StringLength(50)]
		public override string ConcurrencyStamp { get; set; }

		public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

		public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }

		public void Configure([NotNull] EntityTypeBuilder<ApplicationRole> builder)
		{
			_ = builder.HasIndex(t => t.NormalizedName)
				.HasDatabaseName(DbProviderFactories.GetFactory.GetObjectName($"IX_{nameof(ApplicationRole)}_{nameof(NormalizedName)}"))
				.IsUnique()
				.HasFilter($"([{DbProviderFactories.GetFactory.GetObjectName(nameof(NormalizedName))}] IS NOT NULL)");
		}
	}
}
