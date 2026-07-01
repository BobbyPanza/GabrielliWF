namespace GabrielliWF.Models;

public class LamieraDto
{
    public int IdMlpr { get; set; }
    public int IdCrn { get; set; }
    public string CrCod { get; set; } = "";
    public string PaCod { get; set; } = "";
    public string? PaDsc { get; set; }
    public decimal QtaDisp { get; set; }
    public decimal QtaTot { get; set; }
    public string? MgCod { get; set; }
    public string? LcCod { get; set; }
    public string? JdeQuality { get; set; }
    public string? QualityDesc { get; set; }
}

public class ClienteDto
{
    public int IdCli { get; set; }
    public string CtCod { get; set; } = "";
    public string CtRag { get; set; } = "";
}

public class CommessaDto
{
    public int CoNum { get; set; }
    public string CoCod { get; set; } = "";
    public string? CoDsc { get; set; }
    public string? CoRif { get; set; }
    public string CtCod { get; set; } = "";
}

public class RigaOrdineDto
{
    public int IdRow { get; set; }
    public int CoNum { get; set; }
    public string PaCod { get; set; } = "";
    public string? PaDsc { get; set; }
    public decimal LqNta { get; set; }
    public string PaUdm { get; set; } = "";
}

public class AttributoDto
{
    public int Id { get; set; }
    public string? CodiceFactory { get; set; }
    public string Descrizione { get; set; } = "";
    public string? TipoControllo { get; set; }
}

public class AttributoValoreDto
{
    public int Id { get; set; }
    public string Codice { get; set; } = "";
    public string Descrizione { get; set; } = "";
}

public class StepDefDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string? Descrizione { get; set; }
}

public class CrnDto
{
    public int IdCrn { get; set; }
    public string CrCod { get; set; } = "";
    public DateOnly CrDat { get; set; }
    public string? CrDsc { get; set; }
    public int? IdDmp { get; set; }
}

public class DisponibilitaDto
{
    public int Id { get; set; }
    public int IdCrn { get; set; }
    public string CodiceRint { get; set; } = "";
    public string? DescrMateriale { get; set; }
    public string? DescrizioneArticolo { get; set; }
    public string? Magazzino { get; set; }
    public decimal QtaDisp { get; set; }
    public decimal QtaResidua { get; set; }
    public decimal QtaImpegnata { get; set; }
    public decimal Lunghezza { get; set; }
    public decimal Larghezza { get; set; }
    public decimal Spessore { get; set; }
    public string? Qualita { get; set; }
    public string? Difetto { get; set; }
    public string? A1 { get; set; }
    public string? A2 { get; set; }
    public string? A3 { get; set; }
    public string? A4 { get; set; }
    public string? A5 { get; set; }
    public string? A6 { get; set; }
    public string? A7 { get; set; }
    public string? A8 { get; set; }
    public string? A13 { get; set; }
    public string? A14 { get; set; }
}
