using System.ComponentModel.DataAnnotations;

namespace GabrielliWF.Models;

public class ImpegnoCreateModel
{
    [Required(ErrorMessage = "Selezionare una lamiera")]
    public int? IdCrn { get; set; }
    public string? CrCod { get; set; }
    public string? PaCod { get; set; }
    public string? PaDsc { get; set; }
    public string? JdeQuality { get; set; }
    public string? QualityDesc { get; set; }

    [Required(ErrorMessage = "Inserire la quantità")]
    [Range(0.001, double.MaxValue, ErrorMessage = "Quantità deve essere > 0")]
    public decimal? Quantita { get; set; }

    public string? Note { get; set; }

    public List<RiferimentoModel> Riferimenti { get; set; } = [new()];
}

public class RiferimentoModel
{
    public int? IdCli { get; set; }
    public string? CtRag { get; set; }
    public int? CoNum { get; set; }
    public string? CoCod { get; set; }
    public int? IdRow { get; set; }
    public string? PaDscRiga { get; set; }
    public decimal Quantita { get; set; }
}
