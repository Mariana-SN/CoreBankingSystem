namespace CoreBankingSystem.Shared.Exceptions;

public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException()
        : base("Saldo insuficiente.") { }
}