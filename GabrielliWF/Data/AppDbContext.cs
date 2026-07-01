using GabrielliWF.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GabrielliWF.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<WfStepDef> StepDefs { get; set; }
    public DbSet<WfTemplate> Templates { get; set; }
    public DbSet<WfTemplateStep> TemplateSteps { get; set; }
    public DbSet<WfImpegno> Impegni { get; set; }
    public DbSet<WfImpegnoRef> ImpegnoRefs { get; set; }
    public DbSet<WfWorkflow> Workflows { get; set; }
    public DbSet<WfStep> Steps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WfStepDef>().ToTable("WF_StepDef");

        modelBuilder.Entity<WfTemplate>().ToTable("WF_Template");

        modelBuilder.Entity<WfTemplateStep>(e =>
        {
            e.ToTable("WF_TemplateStep");
            e.HasOne(s => s.Template).WithMany(t => t.Steps).HasForeignKey(s => s.IDTemplate);
            e.HasOne(s => s.StepDef).WithMany(d => d.TemplateSteps).HasForeignKey(s => s.IDStepDef);
        });

        modelBuilder.Entity<WfImpegno>(e =>
        {
            e.ToTable("WF_Impegno");
            e.Property(x => x.Stato).HasConversion<short>();
        });

        modelBuilder.Entity<WfImpegnoRef>(e =>
        {
            e.ToTable("WF_ImpegnoRef");
            e.HasOne(r => r.Impegno).WithMany(i => i.Riferimenti).HasForeignKey(r => r.IDImpegno);
        });

        modelBuilder.Entity<WfWorkflow>(e =>
        {
            e.ToTable("WF_Workflow");
            e.Property(x => x.Stato).HasConversion<short>();
            e.HasOne(w => w.Impegno).WithMany(i => i.Workflows).HasForeignKey(w => w.IDImpegno);
            e.HasOne(w => w.Template).WithMany(t => t.Workflows).HasForeignKey(w => w.IDTemplate);
        });

        modelBuilder.Entity<WfStep>(e =>
        {
            e.ToTable("WF_Step");
            e.Property(x => x.Stato).HasConversion<short>();
            e.HasOne(s => s.Workflow).WithMany(w => w.Steps).HasForeignKey(s => s.IDWorkflow);
            e.HasOne(s => s.StepDef).WithMany(d => d.WorkflowSteps).HasForeignKey(s => s.IDStepDef);
        });
    }
}
