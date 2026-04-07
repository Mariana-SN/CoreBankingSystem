using CoreBankingSystem.Shared.Exceptions;
using System.Text.RegularExpressions;

namespace CoreBankingSystem.Shared.ValueObjects;

public record Cpf
{
    private static readonly Regex _digitsOnly = new(@"\D", RegexOptions.Compiled);

    public string Value { get; }

    private Cpf(string value) => Value = value;

    public static Cpf Parse(string input)
    {
        var digits = _digitsOnly.Replace(input, string.Empty);

        if (digits.Length != 11 || digits.Distinct().Count() == 1)
            throw new InvalidCpfException(input);

        if (!CheckVerifierDigit(digits, 9) || !CheckVerifierDigit(digits, 10))
            throw new InvalidCpfException(input);

        return new Cpf(digits);
    }

    public static Cpf FromStored(string digits) => new(digits);

    public string Formatted =>
        $"{Value[..3]}.{Value[3..6]}.{Value[6..9]}-{Value[9..11]}";

    public override string ToString() => Value;

    private static bool CheckVerifierDigit(string cpf, int position)
    {
        var sum = 0;
        for (var i = 0; i < position; i++)
            sum += (cpf[i] - '0') * (position + 1 - i);

        var expected = sum % 11 < 2 ? 0 : 11 - sum % 11;
        return expected == cpf[position] - '0';
    }
}