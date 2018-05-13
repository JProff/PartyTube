using System;
using Microsoft.EntityFrameworkCore;
using PartyTube.DataAccess;

namespace PartyTube.RepositoryTests
{
    public class PartyTubeDbContextMock
    {
        public PartyTubeDbContextMock()
        {
            var options = new DbContextOptionsBuilder<PartyTubeDbContext>()
                         .UseInMemoryDatabase($"InMemoryDB{Guid.NewGuid()}")
                         .Options;
            Context = new PartyTubeDbContext(options);
            Context.Database.EnsureDeleted();
        }

        public PartyTubeDbContext Context { get; }
    }
}