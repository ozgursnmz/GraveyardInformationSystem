using Graveyard.API.Controllers;
using Graveyard.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Graveyard.Tests;

public class GravePlotsControllerTests
{
    // Tum parselleri listeler.
    [Fact]
    public async Task GetAll_ReturnsAllPlots()
    {
        using var db = TestDb.Create();
        db.GravePlots.AddRange(
            new GravePlot { PlotNumber = "P1", Status = "Available" },
            new GravePlot { PlotNumber = "P2", Status = "Occupied" });
        await db.SaveChangesAsync();

        var result = await new GravePlotsController(db).GetAll();

        Assert.Equal(2, result.Value!.Count());
    }

    // Olmayan bir parsel icin 404 (NotFound) doner.
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        using var db = TestDb.Create();

        var result = await new GravePlotsController(db).GetById("YOK");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    // POST ile eklenen parsel gercekten veritabanina yazilir.
    [Fact]
    public async Task Create_PersistsPlot()
    {
        using var db = TestDb.Create();
        var controller = new GravePlotsController(db);

        var plot = new GravePlot { PlotNumber = "P9", Status = "Available", Latitude = 36.8948, Longitude = 30.6240 };
        var result = await controller.Create(plot);

        Assert.IsType<CreatedAtActionResult>(result.Result);
        var saved = await db.GravePlots.FindAsync("P9");
        Assert.NotNull(saved);
        Assert.Equal(36.8948, saved!.Latitude);
    }

    // Arama filtresi durum (status) bazli dogru sonuc doner.
    [Fact]
    public async Task Search_FiltersByStatus()
    {
        using var db = TestDb.Create();
        db.GravePlots.AddRange(
            new GravePlot { PlotNumber = "P1", Status = "Available" },
            new GravePlot { PlotNumber = "P2", Status = "Occupied" },
            new GravePlot { PlotNumber = "P3", Status = "Available" });
        await db.SaveChangesAsync();

        var result = await new GravePlotsController(db).Search("Available", null, null);

        var list = result.Value!.ToList();
        Assert.Equal(2, list.Count);
        Assert.All(list, p => Assert.Equal("Available", p.Status));
    }

    // DELETE artik kalici silmez, arsivler (soft delete): satir kalir, IsArchived=1 olur
    // ve varsayilan (aktif) listede gorunmez.
    [Fact]
    public async Task Delete_ArchivesPlotInsteadOfRemoving()
    {
        using var db = TestDb.Create();
        db.GravePlots.Add(new GravePlot { PlotNumber = "P1", Status = "Available" });
        await db.SaveChangesAsync();

        var result = await new GravePlotsController(db).Delete("P1");

        Assert.IsType<NoContentResult>(result);
        var row = await db.GravePlots.FindAsync("P1");
        Assert.NotNull(row);                 // satir hala var
        Assert.True(row!.IsArchived);        // ama arsivlendi
        Assert.NotNull(row.ArchivedAt);

        // Aktif liste (archived=false) artik bu parseli dondurmez
        var active = (await new GravePlotsController(db).GetAll(false)).Value!;
        Assert.DoesNotContain(active, p => p.PlotNumber == "P1");

        // Arsiv listesi (archived=true) dondurur
        var archived = (await new GravePlotsController(db).GetAll(true)).Value!;
        Assert.Contains(archived, p => p.PlotNumber == "P1");
    }

    // Restore arsivlenmis parseli aktif hale geri getirir.
    [Fact]
    public async Task Restore_UnarchivesPlot()
    {
        using var db = TestDb.Create();
        db.GravePlots.Add(new GravePlot { PlotNumber = "P1", Status = "Available", IsArchived = true, ArchivedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var result = await new GravePlotsController(db).Restore("P1");

        Assert.IsType<NoContentResult>(result);
        var row = await db.GravePlots.FindAsync("P1");
        Assert.False(row!.IsArchived);
        Assert.Null(row.ArchivedAt);
    }
}
