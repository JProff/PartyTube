using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PartyTube.DataAccess;

namespace PartyTube.Web
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PartyTubeDbContext>
    {
        #region Implementation of IDesignTimeDbContextFactory<out PartyTubeDbContext>

        public PartyTubeDbContext CreateDbContext(string[] args)
        {
            const string dbFile = "PartyTubeDb.db";
            var builder = new DbContextOptionsBuilder<PartyTubeDbContext>();
            var connectionString =
                $"Data Source={Path.Combine(Directory.GetCurrentDirectory(), dbFile)}";

            builder.UseSqlite(connectionString);

            return new PartyTubeDbContext(builder.Options);
        }

        #endregion
    }
}