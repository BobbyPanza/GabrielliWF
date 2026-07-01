namespace GabrielliWF.Data.Entities;

public class WfWorkflow
{
    public int ID { get; set; }
    public int IDImpegno { get; set; }
    public int? IDTemplate { get; set; }
    public string? Nome { get; set; }
    public DateTime DataCreazione { get; set; } = DateTime.Now;
    public string? CreatoDa { get; set; }
    public StatoWorkflow Stato { get; set; } = StatoWorkflow.Aperto;

    public WfImpegno Impegno { get; set; } = null!;
    public WfTemplate? Template { get; set; }
    public ICollection<WfStep> Steps { get; set; } = [];
}
