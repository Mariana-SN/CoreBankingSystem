namespace CoreBankingSystem.Shared.Exceptions;

public class DuplicateDocumentException : Exception
{
    public DuplicateDocumentException(string document)
        : base($"Já existe uma conta com o documento '{document}' informado.") { }
}