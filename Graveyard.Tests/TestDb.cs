using Graveyard.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Graveyard.Tests;

// Her test icin izole, bellek-ici (InMemory) bir veritabani olusturur.
// Boylece gercek SQL Server'a ihtiyac duymadan controller mantigi test edilir.
public static class TestDb
{
    public static GraveyardDbContext Create()
    {
        var options = new DbContextOptionsBuilder<GraveyardDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // benzersiz -> testler birbirine karismaz
            .Options;
        return new GraveyardDbContext(options);
    }
}
