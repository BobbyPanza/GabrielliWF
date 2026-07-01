namespace GabrielliWF.Data.Entities;

public class WfStepDef
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descrizione { get; set; }
    public bool IsObsolete { get; set; }
    public DateTime DataCreazione { get; set; } = DateTime.Now;

    public ICollection<WfTemplateStep> TemplateSteps { get; set; } = [];
    public ICollection<WfStep> WorkflowSteps { get; set; } = [];
}
