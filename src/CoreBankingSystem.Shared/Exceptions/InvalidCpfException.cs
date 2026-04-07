namespace CoreBankingSystem.Shared.Exceptions;

public class InvalidCpfException : Exception
{
    public InvalidCpfException(string cpf)
        : base($"'{cpf}' não é um CPF válido.") { }
}