namespace GabrielliWF.Data.Entities;

public class WfTemplateStep
{
    public int ID { get; set; }
    public int IDTemplate { get; set; }
    public int Ordine { get; set; }
    public int? IDStepDef { get; set; }
    public string? NomeCustom { get; set; }
    public int? IDAttributo { get; set; }
    public string? NoteTemplate { get; set; }

    public WfTemplate Template { get; set; } = null!;
    public WfStepDef? StepDef { get; set; }
}
