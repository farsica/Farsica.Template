using System.Collections.Generic;
using Farsica.Framework.Data;
using Farsica.Framework.DataAccess.Entities;
using Farsica.Framework.DataAnnotation;
using Farsica.Framework.DataAnnotation.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Farsica.Template.Entity.Entities.Identity
{
    //[Table(nameof(ApplicationRole))]
    [Table("Role")]
    public class ApplicationRole : IdentityRole<int>, IEntity<ApplicationRole, int>
    {
        public ApplicationRole()
        {
            UserRoles = new List<ApplicationUserRole>();
            RoleClaims = new List<ApplicationRoleClaim>();
        }

        [System.ComponentModel.DataAnnotations.Key]
        [Column(nameof(Id), DataType.Int)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

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

        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasIndex(t => t.NormalizedName)
                    .HasDatabaseName(DbProviderFactories.GetFactory.GetObjectName($"IX_{nameof(ApplicationRole)}_{nameof(NormalizedName)}"))
                    .IsUnique()
                    .HasFilter($"([{DbProviderFactories.GetFactory.GetObjectName(nameof(NormalizedName))}] IS NOT NULL)");
        }
    }
}
