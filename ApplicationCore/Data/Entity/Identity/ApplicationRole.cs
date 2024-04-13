﻿namespace Farsica.Template.Data.Entity.Identity
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Farsica.Template.Data.Enumeration;

    using Farsica.Framework.Data;
    using Farsica.Framework.DataAccess.Entities;
    using Farsica.Framework.DataAnnotation;
    using Farsica.Framework.DataAnnotation.Schema;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    [Table(nameof(ApplicationRole))]
    public class ApplicationRole : IdentityRole<int>, IEntity<ApplicationRole, int>
    {
        public ApplicationRole()
        {
            UserRoles = [];
            RoleClaims = [];
        }

        [System.ComponentModel.DataAnnotations.Key]
        [Column(nameof(Id), DataType.Int)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        [Column(nameof(Name), DataType.UnicodeString)]
        [Required]
        [StringLength(256)]
        public override string? Name { get; set; }

        [Column(nameof(NormalizedName), DataType.UnicodeString)]
        [StringLength(256)]
        [Required]
        public override string? NormalizedName { get; set; }

        [Column(nameof(ConcurrencyStamp), DataType.String)]
        [Required]
        [StringLength(50)]
        public override string? ConcurrencyStamp { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }

        public ICollection<ApplicationRoleClaim> RoleClaims { get; set; }

        public void Configure([NotNull] EntityTypeBuilder<ApplicationRole> builder)
        {
            _ = builder.HasIndex(t => t.NormalizedName)
                .HasDatabaseName(DbProviderFactories.GetFactory.GetObjectName($"IX_{nameof(ApplicationRole)}_{nameof(NormalizedName)}"))
                .IsUnique()
                .HasFilter($"([{DbProviderFactories.GetFactory.GetObjectName(nameof(NormalizedName), pluralize: false)}] IS NOT NULL)");

            List<ApplicationRole> seedData =
            [
                new ApplicationRole { Id = 1, Name = nameof(Role.Admin), NormalizedName = nameof(Role.Admin).ToUpperInvariant(), ConcurrencyStamp = "85465B3B-E646-49BC-AAC6-D07C450B3AE3", },
            ];
            _ = builder.HasData(seedData);
        }
    }
}
