using Farsica.Framework.Data;
using Farsica.Framework.DataAccess.Entities;
using Farsica.Framework.DataAnnotation;
using Farsica.Framework.DataAnnotation.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Farsica.Template.Entity.Entities.Identity
{
    //[Table(nameof(ApplicationUserToken))]
    [Table("UserToken")]
    public class ApplicationUserToken : IdentityUserToken<int>, IEntity<ApplicationUserToken, int>
    {
        [Required]
        [Column(nameof(UserId), DataType.Int)]
        public override int UserId { get; set; }

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

        int IEntity<ApplicationUserToken, int>.Id { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
        {
            builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            builder.HasOne(t => t.User)
                .WithMany(t => t.UserTokens)
                .HasForeignKey(t => t.UserId);
        }
    }
}