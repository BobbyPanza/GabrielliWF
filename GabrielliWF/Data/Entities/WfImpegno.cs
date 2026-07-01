namespace GabrielliWF.Data.Entities;

public class WfImpegno
{
    public int ID { get; set; }
    public int IDCRN { get; set; }
    public decimal Quantita { get; set; }
    public DateTime DataImpegno { get; set; } = DateTime.Now;
    public string? Note { get; set; }
    public string? CreatoDa { get; set; }
    public StatoImpegno Stato { get; set; } = StatoImpegno.Aperto;
    public string? JdeQuality { get; set; }
    public string? QualityDesc { get; set; }

    public ICollection<WfImpegnoRef> Riferimenti { get; set; } = [];
    public ICollection<WfWorkflow> Workflows { get; set; } = [];
}
