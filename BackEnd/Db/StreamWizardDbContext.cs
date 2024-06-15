using BackEnd.Db.Interfaces;
using BackEnd.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BackEnd.Db
{
	public class StreamWizardDbContext(DbContextOptions options) : DbContext(options)
	{
		public DbSet<TwitchStreamer> TwitchStreamers => Set<TwitchStreamer>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TwitchStreamer>(entity =>
			{
				entity.Property(e => e.Name)
					.IsRequired()
					.HasMaxLength(512);

				entity.Property(e => e.DisplayName)
					.IsRequired()
					.HasMaxLength(512);

				entity.Property(e => e.LastLoginDateTime)
				   .IsRequired();

				entity.Property(e => e.AccessToken)
					.IsRequired()
					.HasMaxLength(512);

				entity.Property(e => e.RefreshToken)
					.IsRequired()
					.HasMaxLength(512);

				AddGenericFields<TwitchStreamer>(entity);
			});
			modelBuilder.Entity<TwitchStreamer>().HasIndex(t => new { t.Id }).IsUnique(true);

			Expression<Func<ISoftDeleteable, bool>> filterSoftDeleteable = bm => !bm.Deleted;
			Expression filter = null;
			foreach (var type in modelBuilder.Model.GetEntityTypes())
			{
				var param = Expression.Parameter(type.ClrType, "entity");
				if (typeof(ISoftDeleteable).IsAssignableFrom(type.ClrType))
				{
					filter = AddFilter(filter, ReplacingExpressionVisitor.Replace(filterSoftDeleteable.Parameters.First(), param, filterSoftDeleteable.Body));
				}

				if (filter != null)
				{
					type.SetQueryFilter(Expression.Lambda(filter, param));
				}
			}
		}

		private static Expression AddFilter(Expression filter, Expression newFilter)
		{
			if (filter == null)
			{
				filter = newFilter;
			}
			else
			{
				filter = Expression.And(filter, newFilter);
			}
			return filter;
		}

		public void AddGenericFields<T>(EntityTypeBuilder entity)
		{
			entity.Property("Id")

				  .IsRequired();

			if (typeof(ISoftDeleteable).IsAssignableFrom(typeof(T)))
			{
				entity.Property("Deleted")
					.IsRequired();
			}

			if (typeof(IDateTimeTrackable).IsAssignableFrom(typeof(T)))
			{
				entity.Property("CreationDateTime")
				   .IsRequired();

				entity.Property("LastModificationDateTime");
			}

			if (typeof(ITwitchOwnable).IsAssignableFrom(typeof(T)))
			{
				entity.Property("TwitchOwner")
					.HasMaxLength(20)
					.IsRequired();
			}
		}

		public override int SaveChanges()
		{
			SoftDelete();
			TimeTrack();
			return base.SaveChanges();
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			SoftDelete();
			TimeTrack();
			return await base.SaveChangesAsync(cancellationToken);
		}

		private void SoftDelete()
		{
			ChangeTracker.DetectChanges();
			var markedAsDeleted = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted);
			foreach (var item in markedAsDeleted)
			{
				if (item.Entity is ISoftDeleteable entity)
				{
					item.State = EntityState.Unchanged;
					entity.Deleted = true;
				}
			}
		}

		private void TimeTrack()
		{
			ChangeTracker.DetectChanges();
			var markedEntries = ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
			DateTime now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"));
			foreach (var item in markedEntries)
			{
				if (item.Entity is IDateTimeTrackable entity)
				{
					entity.LastModificationDateTime = now;
					if (item.State == EntityState.Added && entity.CreationDateTime == DateTime.MinValue)
					{
						entity.CreationDateTime = now;
					}
				}
			}
		}
	}
}
