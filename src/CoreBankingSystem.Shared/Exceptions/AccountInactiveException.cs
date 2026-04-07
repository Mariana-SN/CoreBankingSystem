namespace CoreBankingSystem.Shared.Exceptions;

public class AccountInactiveException : Exception
{
    public AccountInactiveException()
        : base("Conta está inativa.") { }
}