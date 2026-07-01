using GabrielliWF.Data;
using GabrielliWF.Data.Entities;
using GabrielliWF.Models;
using Microsoft.EntityFrameworkCore;

namespace GabrielliWF.Services;

public class TemplateService(AppDbContext db)
{
    public async Task<List<WfTemplate>> GetAllAsync() =>
        await db.Templates.Where(t => !t.IsObsolete).OrderBy(t => t.Nome).ToListAsync();

    public async Task<WfTemplate?> GetByIdAsync(int id) =>
        await db.Templates.Include(t => t.Steps).ThenInclude(s => s.StepDef).FirstOrDefaultAsync(t => t.ID == id);

    public async Task<int> SaveFromWorkflowAsync(WorkflowCreateModel model, string? utente)
    {
        var tpl = new WfTemplate
        {
            Nome = model.NomeTemplate!,
            DataCreazione = DateTime.Now,
            CreatoDa = utente
        };

        for (int i = 0; i < model.Steps.Count; i++)
        {
            var s = model.Steps[i];
            tpl.Steps.Add(new WfTemplateStep
            {
                Ordine = i + 1,
                IDStepDef = s.IDStepDef,
                NomeCustom = s.IDStepDef.HasValue ? null : s.NomeStep,
                IDAttributo = s.IDAttributo
            });
        }

        db.Templates.Add(tpl);
        await db.SaveChangesAsync();
        return tpl.ID;
    }

    public async Task<List<StepDefDto>> GetOrCreateStepDefsAsync(List<WorkflowStepModel> steps)
    {
        var result = new List<StepDefDto>();
        foreach (var s in steps.Where(s => !s.IDStepDef.HasValue && !string.IsNullOrWhiteSpace(s.NomeStep)))
        {
            var existing = await db.StepDefs.FirstOrDefaultAsync(d => d.Nome == s.NomeStep.Trim());
            if (existing is null)
            {
                existing = new WfStepDef { Nome = s.NomeStep.Trim(), DataCreazione = DateTime.Now };
                db.StepDefs.Add(existing);
                await db.SaveChangesAsync();
            }
            s.IDStepDef = existing.ID;
            result.Add(new StepDefDto { Id = existing.ID, Nome = existing.Nome, Descrizione = existing.Descrizione });
        }
        return result;
    }

    public async Task<List<WfStepDef>> GetStepDefsAsync(string? filter = null) =>
        await db.StepDefs
            .Where(d => !d.IsObsolete && (filter == null || d.Nome.Contains(filter)))
            .OrderBy(d => d.Nome)
            .ToListAsync();

    public async Task MarkObsoleteAsync(int id)
    {
        var tpl = await db.Templates.FindAsync(id);
        if (tpl is null) return;
        tpl.IsObsolete = true;
        await db.SaveChangesAsync();
    }
}
