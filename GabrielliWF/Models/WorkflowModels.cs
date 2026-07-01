using System.ComponentModel.DataAnnotations;
using GabrielliWF.Data.Entities;

namespace GabrielliWF.Models;

public class WorkflowCreateModel
{
    public string? Nome { get; set; }
    public int? IDTemplateCaricato { get; set; }
    public List<WorkflowStepModel> Steps { get; set; } = [];

    public bool SalvaTemplate { get; set; }
    public string? NomeTemplate { get; set; }
}

public class WorkflowStepModel
{
    public int Ordine { get; set; }
    public int? IDStepDef { get; set; }
    public string NomeStep { get; set; } = string.Empty;
    public string? Descrizione { get; set; }
    public int? IDAttributo { get; set; }
    public string? NomeAttributo { get; set; }
}

public class StepProgressModel
{
    public int IDStep { get; set; }
    public string NomeStep { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selezionare uno stato")]
    public StatoStep? Stato { get; set; }

    public int? IDAttributoValore { get; set; }
    public string? ValoreRegistrato { get; set; }
    public string? Nota { get; set; }
    public DateOnly? DataProssimoStep { get; set; }
}
