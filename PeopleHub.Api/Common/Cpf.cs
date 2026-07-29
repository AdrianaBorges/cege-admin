using System.Text.RegularExpressions;

namespace PeopleHub.Api.Common;

public static class Cpf
{
    public static string OnlyDigits(string input) =>
        Regex.Replace(input ?? "", "[^0-9]", "");

    public static bool IsValid(string cpfInput)
    {
        var cpf = OnlyDigits(cpfInput);
        if (cpf.Length != 11) return false;
        if (new string(cpf[0], 11) == cpf) return false;

        int[] n = cpf.Select(c => c - '0').ToArray();

        int sum1 = 0;
        for (int i = 0; i < 9; i++) sum1 += n[i] * (10 - i);
        int d1 = (sum1 * 10) % 11; if (d1 == 10) d1 = 0;
        if (n[9] != d1) return false;

        int sum2 = 0;
        for (int i = 0; i < 10; i++) sum2 += n[i] * (11 - i);
        int d2 = (sum2 * 10) % 11; if (d2 == 10) d2 = 0;

        return n[10] == d2;
    }
}