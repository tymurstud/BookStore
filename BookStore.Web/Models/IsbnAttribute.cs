using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BookStore.Web.Models;

public class IsbnAttribute : ValidationAttribute
{
    public bool RequireChecksum { get; set; } = false;

    public IsbnAttribute() => ErrorMessage = "Nieprawidłowy format ISBN";

    public override bool IsValid(object? value)
    {
        if (value is not string raw) return true;

        var digits = Regex.Replace(raw, @"[\s\-]", "");

        if (!Regex.IsMatch(digits, @"^\d{10}$|^\d{13}$"))
            return false;

        if (!RequireChecksum)
            return true;

        if (digits.Length == 13)
        {
            int sum = 0;
            for (int i = 0; i < 12; i++)
                sum += (digits[i] - '0') * (i % 2 == 0 ? 1 : 3);
            int check = (10 - sum % 10) % 10;
            return check == (digits[12] - '0');
        }

        return true;
    }
}
