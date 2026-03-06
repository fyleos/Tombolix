using Microsoft.EntityFrameworkCore;
using TradeUp.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TradeUp.Shared.Models;

namespace TradeUp.Server.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public DbSet<SharedUserOption> SharedUserOptions { get; set; }
    public DbSet<ApplicationFeature> ApplicationFeatures { get; set; }

    public DbSet<DrawItem> DrawItems { get; set; }
    public DbSet<DrawResult> DrawResults { get; set; }
    public DbSet<DrawContext> DrawContexts { get; set; }
    public DbSet<DrawData> DrawDatas { get; set; }
}
