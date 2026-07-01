namespace GabrielliWF.Data.Entities;

public enum StatoImpegno : short { Aperto = 0, Completato = 1, Annullato = 2 }
public enum StatoWorkflow : short { Aperto = 0, Completato = 1, Annullato = 2 }
public enum StatoStep : short { Aperto = 0, Eseguito = 1, NonOk = 2, Annullato = 3 }
