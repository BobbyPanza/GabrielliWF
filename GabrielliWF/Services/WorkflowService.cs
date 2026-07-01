using GabrielliWF.Data;
using GabrielliWF.Data.Entities;
using GabrielliWF.Models;
using Microsoft.EntityFrameworkCore;

namespace GabrielliWF.Services;

public class WorkflowService(AppDbContext db)
{
    public async Task<WfWorkflow?> GetByIdAsync(int id) =>
        await db.Workflows
            .Include(w => w.Steps).ThenInclude(s => s.StepDef)
            .Include(w => w.Template)
            .Include(w => w.Impegno)
            .FirstOrDefaultAsync(w => w.ID == id);

    public async Task<int> CreateAsync(int idImpegno, WorkflowCreateModel model, string? utente)
    {
        var wf = new WfWorkflow
        {
            IDImpegno = idImpegno,
            Nome = model.Nome,
            IDTemplate = model.IDTemplateCaricato,
            CreatoDa = utente,
            DataCreazione = DateTime.Now
        };

        for (int i = 0; i < model.Steps.Count; i++)
        {
            var s = model.Steps[i];
            wf.Steps.Add(new WfStep
            {
                Ordine = i + 1,
                IDStepDef = s.IDStepDef,
                NomeCustom = s.IDStepDef.HasValue ? null : s.NomeStep,
                IDAttributo = s.IDAttributo
            });
        }

        db.Workflows.Add(wf);
        await db.SaveChangesAsync();
        return wf.ID;
    }

    public async Task AddStepAsync(int idWorkflow, WorkflowStepModel model)
    {
        var maxOrd = await db.Steps.Where(s => s.IDWorkflow == idWorkflow).MaxAsync(s => (int?)s.Ordine) ?? 0;
        db.Steps.Add(new WfStep
        {
            IDWorkflow = idWorkflow,
            Ordine = maxOrd + 1,
            IDStepDef = model.IDStepDef,
            NomeCustom = model.IDStepDef.HasValue ? null : model.NomeStep,
            IDAttributo = model.IDAttributo
        });
        await db.SaveChangesAsync();
    }

    public async Task RegisterProgressAsync(StepProgressModel model, string? utente)
    {
        var step = await db.Steps.FindAsync(model.IDStep);
        if (step is null) return;

        step.Stato = model.Stato!.Value;
        step.IDAttributoValore = model.IDAttributoValore;
        step.ValoreRegistrato = model.ValoreRegistrato;
        step.Nota = model.Nota;
        step.DataProssimoStep = model.DataProssimoStep;
        step.DataEsecuzione = DateTime.Now;
        step.RegistratoDa = utente;
        step.DataRegistrazione = DateTime.Now;

        await db.SaveChangesAsync();

        // auto-complete workflow if all steps are done
        var workflow = await db.Workflows
            .Include(w => w.Steps)
            .FirstOrDefaultAsync(w => w.ID == step.IDWorkflow);
        if (workflow is not null && workflow.Steps.All(s => s.Stato != StatoStep.Aperto))
        {
            workflow.Stato = StatoWorkflow.Completato;
            await db.SaveChangesAsync();
        }
    }

    public async Task SetStatoWorkflowAsync(int id, StatoWorkflow stato)
    {
        var wf = await db.Workflows.FindAsync(id);
        if (wf is null) return;
        wf.Stato = stato;
        await db.SaveChangesAsync();
    }

    public async Task DeleteStepAsync(int stepId)
    {
        var step = await db.Steps.FindAsync(stepId);
        if (step is null) return;
        db.Steps.Remove(step);
        await db.SaveChangesAsync();
    }
}
