namespace GabrielliWF.Data.Entities;

public class WfTemplate
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descrizione { get; set; }
    public DateTime DataCreazione { get; set; } = DateTime.Now;
    public string? CreatoDa { get; set; }
    public bool IsObsolete { get; set; }

    public ICollection<WfTemplateStep> Steps { get; set; } = [];
    public ICollection<WfWorkflow> Workflows { get; set; } = [];
}
