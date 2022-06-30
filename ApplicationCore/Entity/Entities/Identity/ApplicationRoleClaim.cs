﻿using Farsica.Framework.Data;
using Farsica.Framework.DataAccess.Entities;
using Farsica.Framework.DataAnnotation;
using Farsica.Framework.DataAnnotation.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Farsica.Template.Entity.Entities.Identity
{
    //[Table(nameof(ApplicationRoleClaim))]
    [Table("RoleClaim")]
    public class ApplicationRoleClaim : IdentityRoleClaim<int>, IEntity<ApplicationRoleClaim, int>
    {
        [System.ComponentModel.DataAnnotations.Key]
        [Column(nameof(Id), DataType.Int)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Required]
        [Column(nameof(RoleId), DataType.Int)]
        public override int RoleId { get; set; }

        [StringLength(128)]
        [Column(nameof(ClaimType), DataType.String)]
        public override string ClaimType { get; set; }

        [StringLength(128)]
        [Column(nameof(ClaimValue), DataType.UnicodeString)]
        public override string ClaimValue { get; set; }

        public virtual ApplicationRole Role { get; set; }

        public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
        {
            builder.HasIndex(t => t.RoleId);
            builder.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId);
        }
    }
}
