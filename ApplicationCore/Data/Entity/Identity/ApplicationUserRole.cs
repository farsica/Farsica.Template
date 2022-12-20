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

	[Table(nameof(ApplicationUserRole))]
	public class ApplicationUserRole : IdentityUserRole<long>, IEntity<ApplicationUserRole, long>
	{
		[Required]
		[Column(nameof(UserId), DataType.Long)]
		public override long UserId { get; set; }

		[Required]
		[Column(nameof(RoleId), DataType.Int)]
		public override long RoleId { get; set; }

		public virtual ApplicationRole Role { get; set; }

		public virtual ApplicationUser User { get; set; }

		long IEntity<ApplicationUserRole, long>.Id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public void Configure([NotNull] EntityTypeBuilder<ApplicationUserRole> builder)
		{
			_ = builder.HasKey(t => new { t.UserId, t.RoleId });

			_ = builder.HasIndex(t => t.RoleId)
				.HasDatabaseName(DbProviderFactories.GetFactory.GetObjectName($"IX_{nameof(ApplicationUserRole)}_{nameof(RoleId)}"));

			_ = builder.HasOne(t => t.Role)
				.WithMany(t => t.UserRoles)
				.HasForeignKey(t => t.RoleId);

			_ = builder.HasOne(t => t.User)
				.WithMany(t => t.UserRoles)
				.HasForeignKey(t => t.UserId);
		}
	}
}