using Farsica.Framework.Data;
using Farsica.Framework.DataAccess.Entities;
using Farsica.Framework.DataAnnotation;
using Farsica.Framework.DataAnnotation.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Farsica.Template.Entity.Entities.Identity
{
    //[Table(nameof(ApplicationUserLogin))]
    [Table("UserLogin")]
    public class ApplicationUserLogin : IdentityUserLogin<int>, IEntity<ApplicationUserLogin, int>
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
        [Column(nameof(UserId), DataType.Int)]
        public override int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        int IEntity<ApplicationUserLogin, int>.Id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
        {
            builder.HasIndex(t => t.UserId)
                .HasDatabaseName(DbProviderFactories.GetFactory.GetObjectName($"IX_{nameof(ApplicationUserLogin)}_{nameof(UserId)}"));


            builder.HasOne(t => t.User)
                .WithMany(t => t.UserLogins)
                .HasForeignKey(t => t.UserId);

            builder.HasKey(t => new { t.LoginProvider, t.ProviderKey });
        }
    }
}