using GabrielliWF.Data;
using GabrielliWF.Data.Entities;
using GabrielliWF.Models;
using Microsoft.EntityFrameworkCore;

namespace GabrielliWF.Services;

public class ImpegnoService(AppDbContext db, FactoryLookupService factory)
{
    public async Task<List<ImpegnoListItem>> GetListAsync()
    {
        var impegni = await db.Impegni
            .Include(i => i.Riferimenti)
            .Include(i => i.Workflows)
            .OrderByDescending(i => i.DataImpegno)
            .ToListAsync();

        var result = new List<ImpegnoListItem>();
        foreach (var imp in impegni)
        {
            var lamiera = await factory.GetLamieraByIdCrnAsync(imp.IDCRN);
            result.Add(new ImpegnoListItem
            {
                ID = imp.ID,
                IDCRN = imp.IDCRN,
                CrCod = lamiera?.CrCod ?? imp.IDCRN.ToString(),
                PaCod = lamiera?.PaCod,
                PaDsc = lamiera?.PaDsc,
                Quantita = imp.Quantita,
                DataImpegno = imp.DataImpegno,
                Stato = imp.Stato,
                NumWorkflow = imp.Workflows.Count,
                JdeQuality = imp.JdeQuality,
                QualityDesc = imp.QualityDesc,
                Riferimenti = imp.Riferimenti
                    .Select(r => new RefSummary { CtRag = r.CtRag, CoCod = r.CoCod })
                    .ToList()
            });
        }
        return result;
    }

    public async Task<WfImpegno?> GetByIdAsync(int id) =>
        await db.Impegni
            .Include(i => i.Riferimenti)
            .Include(i => i.Workflows).ThenInclude(w => w.Steps).ThenInclude(s => s.StepDef)
            .FirstOrDefaultAsync(i => i.ID == id);

    public async Task<int> CreateAsync(ImpegnoCreateModel model, string? utente)
    {
        var impegno = new WfImpegno
        {
            IDCRN = model.IdCrn!.Value,
            Quantita = model.Quantita!.Value,
            Note = model.Note,
            CreatoDa = utente,
            DataImpegno = DateTime.Now,
            JdeQuality = model.JdeQuality,
            QualityDesc = model.QualityDesc
        };

        foreach (var r in model.Riferimenti.Where(r => r.IdCli.HasValue && r.CoNum.HasValue && r.IdRow.HasValue))
        {
            impegno.Riferimenti.Add(new WfImpegnoRef
            {
                IDCLI = r.IdCli!.Value,
                CONUM = r.CoNum!.Value,
                IDROW = r.IdRow!.Value,
                Quantita = r.Quantita,
                CtRag = r.CtRag,
                CoCod = r.CoCod
            });
        }

        db.Impegni.Add(impegno);
        await db.SaveChangesAsync();
        return impegno.ID;
    }

    public async Task SetStatoAsync(int id, StatoImpegno stato)
    {
        var imp = await db.Impegni.FindAsync(id);
        if (imp is null) return;
        imp.Stato = stato;
        await db.SaveChangesAsync();
    }
}

public class ImpegnoListItem
{
    public int ID { get; set; }
    public int IDCRN { get; set; }
    public string CrCod { get; set; } = string.Empty;
    public string? PaCod { get; set; }
    public string? PaDsc { get; set; }
    public decimal Quantita { get; set; }
    public DateTime DataImpegno { get; set; }
    public StatoImpegno Stato { get; set; }
    public int NumWorkflow { get; set; }
    public string? JdeQuality { get; set; }
    public string? QualityDesc { get; set; }
    public List<RefSummary> Riferimenti { get; set; } = [];
}

public class RefSummary
{
    public string? CtRag { get; set; }
    public string? CoCod { get; set; }
}
