namespace CoreBankingSystem.Shared.Exceptions;

public class InvalidAmountException : Exception
{
    public InvalidAmountException()
        : base("Valor de transferência deve ser maior do que zero.") { }
}
