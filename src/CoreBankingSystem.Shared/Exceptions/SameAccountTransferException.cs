namespace CoreBankingSystem.Shared.Exceptions;

public class SameAccountTransferException : Exception
{
    public SameAccountTransferException()
        : base("Contas de origem e destino devem ser diferentes.") { }
}