namespace CoreBankingSystem.Shared.Exceptions;

public class AccountNotFoundException : Exception
{
    public AccountNotFoundException(string document)
        : base($"Conta com o documento '{document}' não encontrada.") { }
}
