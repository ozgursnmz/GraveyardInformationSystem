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

    // DELETE var olan parseli siler, sonrasinda veritabaninda bulunmaz.
    [Fact]
    public async Task Delete_RemovesPlot()
    {
        using var db = TestDb.Create();
        db.GravePlots.Add(new GravePlot { PlotNumber = "P1", Status = "Available" });
        await db.SaveChangesAsync();

        var result = await new GravePlotsController(db).Delete("P1");

        Assert.IsType<NoContentResult>(result);
        Assert.False(await db.GravePlots.AnyAsync(p => p.PlotNumber == "P1"));
    }
}
