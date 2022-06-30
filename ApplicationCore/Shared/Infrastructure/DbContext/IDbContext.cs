using Farsica.Framework.DataAnnotation;
using System;

namespace Farsica.Template.Shared.Infrastructure.DbContext
{
    [Injectable]
    public interface IDbContext : IDisposable
    {
    }
}
