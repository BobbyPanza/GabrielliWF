namespace GabrielliWF.Data.Entities;

public class WfImpegnoRef
{
    public int ID { get; set; }
    public int IDImpegno { get; set; }
    public int IDCLI { get; set; }
    public int CONUM { get; set; }
    public int IDROW { get; set; }
    public decimal Quantita { get; set; }
    public string? CtRag { get; set; }
    public string? CoCod { get; set; }

    public WfImpegno Impegno { get; set; } = null!;
}
