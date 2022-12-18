namespace Farsica.Template.Shared.Infrastructure.DbContext
{
	using Farsica.Framework.DataAnnotation;
	using System;

	[Injectable]
	public interface IDbContext : IDisposable
	{
	}
}
