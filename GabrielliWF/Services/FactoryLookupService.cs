using Dapper;
using GabrielliWF.Models;
using Microsoft.Data.SqlClient;

namespace GabrielliWF.Services;

public class FactoryLookupService(IConfiguration config)
{
    private SqlConnection Open() => new(config.GetConnectionString("IntesiFactory"));

    public async Task<List<LamieraDto>> SearchLamiereAsync(string? filter, int top = 50)
    {
        using var conn = Open();
        var sql = """
            SELECT TOP (@top)
                m.IDMLPR       AS IdMlpr,
                m.IDCRN        AS IdCrn,
                c.CRCOD        AS CrCod,
                m.PACOD        AS PaCod,
                p.PADSC        AS PaDsc,
                (m.CRQTA - m.CRQTR) AS QtaDisp,
                m.CRQTA        AS QtaTot,
                m.MGCOD        AS MgCod,
                m.LCCOD        AS LcCod,
                p.x_JDEQuality AS JdeQuality,
                q.Description  AS QualityDesc
            FROM L_MLPR m
            JOIN S_CRN c ON c.IDCRN = m.IDCRN
            JOIN A_PAR p ON p.PACOD = m.PACOD
            LEFT JOIN JDEQuality q ON q.Code = p.x_JDEQuality
            WHERE (m.CRQTA - m.CRQTR) > 0
              AND (
                    @filter IS NULL
                 OR c.CRCOD LIKE '%' + @filter + '%'
                 OR m.PACOD LIKE '%' + @filter + '%'
                 OR p.PADSC LIKE '%' + @filter + '%'
              )
            ORDER BY c.CRDAT DESC, c.CRCOD
            """;
        var rows = await conn.QueryAsync<LamieraDto>(sql, new { top, filter = string.IsNullOrWhiteSpace(filter) ? null : filter });
        return rows.ToList();
    }

    public async Task<LamieraDto?> GetLamieraByIdCrnAsync(int idCrn)
    {
        using var conn = Open();
        var sql = """
            SELECT TOP 1
                m.IDMLPR AS IdMlpr, m.IDCRN AS IdCrn, c.CRCOD AS CrCod,
                m.PACOD AS PaCod, p.PADSC AS PaDsc,
                (m.CRQTA - m.CRQTR) AS QtaDisp, m.CRQTA AS QtaTot,
                m.MGCOD AS MgCod, m.LCCOD AS LcCod,
                p.x_JDEQuality AS JdeQuality, q.Description AS QualityDesc
            FROM L_MLPR m
            JOIN S_CRN c ON c.IDCRN = m.IDCRN
            JOIN A_PAR p ON p.PACOD = m.PACOD
            LEFT JOIN JDEQuality q ON q.Code = p.x_JDEQuality
            WHERE m.IDCRN = @idCrn
            """;
        return await conn.QueryFirstOrDefaultAsync<LamieraDto>(sql, new { idCrn });
    }

    public async Task<List<ClienteDto>> GetClientiAsync(string? filter = null)
    {
        using var conn = Open();
        var sql = """
            SELECT TOP 100
                IDCLI        AS IdCli,
                CTCOD        AS CtCod,
                ISNULL(CTRAG,'') AS CtRag
            FROM A_CLI
            WHERE (IsObsolete IS NULL OR IsObsolete = 0)
              AND (
                    @filter IS NULL
                 OR CTCOD LIKE '%' + @filter + '%'
                 OR CTRAG LIKE '%' + @filter + '%'
              )
            ORDER BY CTRAG
            """;
        var rows = await conn.QueryAsync<ClienteDto>(sql, new { filter = string.IsNullOrWhiteSpace(filter) ? null : filter });
        return rows.ToList();
    }

    public async Task<List<CommessaDto>> GetCommesseByClienteAsync(int idCli)
    {
        using var conn = Open();
        var sql = """
            SELECT o.CONUM AS CoNum, o.COCOD AS CoCod,
                   ISNULL(o.CODSC,'') AS CoDsc,
                   ISNULL(o.CORIF,'')  AS CoRif,
                   o.CTCOD AS CtCod
            FROM A_COM o
            JOIN A_CLI c ON c.CTCOD = o.CTCOD
            WHERE c.IDCLI = @idCli
              AND o.COSTO NOT IN (9, 99)
            ORDER BY o.CONUM DESC
            """;
        var rows = await conn.QueryAsync<CommessaDto>(sql, new { idCli });
        return rows.ToList();
    }

    public async Task<List<RigaOrdineDto>> GetRigheByCommessaAsync(int coNum)
    {
        using var conn = Open();
        var sql = """
            SELECT IDROW AS IdRow, CONUM AS CoNum, PACOD AS PaCod,
                   ISNULL(PADSC,'') AS PaDsc, LQNTA AS LqNta, PAUDM AS PaUdm
            FROM L_CMPA
            WHERE CONUM = @coNum
            ORDER BY FINUM
            """;
        var rows = await conn.QueryAsync<RigaOrdineDto>(sql, new { coNum });
        return rows.ToList();
    }

    public async Task<List<AttributoDto>> GetAttributiAsync()
    {
        using var conn = Open();
        var sql = """
            SELECT ID AS Id, CodiceFactory, Descrizione, TipoControllo
            FROM X_JDE_AttributiAnagrafica
            WHERE ControlloAbilitato = 1 OR ControlloAbilitato IS NULL
            ORDER BY Descrizione
            """;
        var rows = await conn.QueryAsync<AttributoDto>(sql);
        return rows.ToList();
    }

    public async Task<List<AttributoValoreDto>> GetValoriAttributoAsync(int idAttributo)
    {
        using var conn = Open();
        var sql = """
            SELECT ID AS Id,
                   RTRIM(Codice)      AS Codice,
                   RTRIM(Descrizione) AS Descrizione
            FROM X_JDE_AttributiValori
            WHERE ID_X_JDE_AttributiAnagrafica = @idAttributo
              AND LEN(RTRIM(Codice)) > 0
              AND RTRIM(Codice) <> '.'
            ORDER BY Codice
            """;
        var rows = await conn.QueryAsync<AttributoValoreDto>(sql, new { idAttributo });
        return rows.ToList();
    }
}
