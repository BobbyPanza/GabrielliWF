namespace GabrielliWF.Data.Entities;

public class WfStep
{
    public int ID { get; set; }
    public int IDWorkflow { get; set; }
    public int Ordine { get; set; }
    public int? IDStepDef { get; set; }
    public string? NomeCustom { get; set; }
    public int? IDAttributo { get; set; }
    public int? IDAttributoValore { get; set; }
    public StatoStep Stato { get; set; } = StatoStep.Aperto;
    public string? ValoreRegistrato { get; set; }
    public string? Nota { get; set; }
    public DateTime? DataEsecuzione { get; set; }
    public DateOnly? DataProssimoStep { get; set; }
    public string? RegistratoDa { get; set; }
    public DateTime? DataRegistrazione { get; set; }

    public WfWorkflow Workflow { get; set; } = null!;
    public WfStepDef? StepDef { get; set; }
}
