using Farsica.Framework.Data;
using Farsica.Framework.DataAccess.Entities;
using Farsica.Framework.DataAnnotation;
using Farsica.Framework.DataAnnotation.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Farsica.Template.Entity.Entities.Identity
{
    //[Table(nameof(ApplicationUserRole))]
    [Table("UserRole")]
    public class ApplicationUserRole : IdentityUserRole<int>, IEntity<ApplicationUserRole, int>
    {
        [Required]
        [Column(nameof(UserId), DataType.Int)]
        public override int UserId { get; set; }

        [Required]
        [Column(nameof(RoleId), DataType.Int)]
        public override int RoleId { get; set; }

        public virtual ApplicationRole Role { get; set; }

        public virtual ApplicationUser User { get; set; }

        int IEntity<ApplicationUserRole, int>.Id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
        {
            builder.HasKey(t => new { t.UserId, t.RoleId });

            builder.HasIndex(t => t.RoleId)
                .HasDatabaseName(DbProviderFactories.GetFactory.GetObjectName($"IX_{nameof(ApplicationUserRole)}_{nameof(RoleId)}"));

            builder.HasOne(t => t.Role)
                .WithMany(t => t.UserRoles)
                .HasForeignKey(t => t.RoleId);

            builder.HasOne(t => t.User)
                .WithMany(t => t.UserRoles)
                .HasForeignKey(t => t.UserId);
        }
    }
}