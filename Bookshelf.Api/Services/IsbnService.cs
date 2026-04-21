using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Bookshelf.Api.Helper;

namespace Bookshelf.Api.Services;

public interface IIsbnService
{
    string? FormatIsbn(string isbn);
}

public class IsbnService : IIsbnService
{
    public string? FormatIsbn(string isbn)
    {
        if (string.IsNullOrEmpty(isbn)) return null;

        var withoutHyphens = Regex.Replace(isbn, @"[\s-]", "");

        if (!Regex.IsMatch(withoutHyphens, "^[0-9]+X?$"))
            return null;

        try
        {
            return Hyphenate(withoutHyphens.ToUpper());
        }
        catch (Exception)
        {
            return withoutHyphens;
        }
    }

    private string Hyphenate(string isbn)
    {
        var (gs1Prefix, remainder) = GetGs1PrefixAndRemainder(isbn);

        var eanPrefix = IsbnRangesHelper.IsbnData.EanUccPrefixes?.FirstOrDefault(p => p.Prefix == gs1Prefix)
                       ?? throw new Exception($"GS1 prefix {gs1Prefix} not recognized");

        var groupPrefixLength = FindRuleLength(remainder, eanPrefix.Rules)
                                ?? throw new Exception("Not in any recognized group range or group range is unused");

        var groupIdentifier = remainder[..groupPrefixLength];
        remainder = remainder[groupPrefixLength..];

        var withHyphens = isbn.Length == 13 ? $"{gs1Prefix}-{groupIdentifier}-" : $"{groupIdentifier}-";

        var fullGroupPrefix = $"{gs1Prefix}-{groupIdentifier}";
        var registrationGroup = IsbnRangesHelper.IsbnData.RegistrationGroups?.FirstOrDefault(g => g.Prefix == fullGroupPrefix)
                               ?? throw new Exception($"Prefix {fullGroupPrefix} not recognized");

        var publisherLength = FindRuleLength(remainder, registrationGroup.Rules, padTo7: true)
                             ?? throw new Exception("Not in any recognized publisher range or publisher range is unused");

        var publisherIdentifier = remainder[..publisherLength];
        remainder = remainder[publisherLength..];

        var titleIdentifier = remainder[..^1];
        var checkDigit = remainder[^1..];

        withHyphens += $"{publisherIdentifier}-{titleIdentifier}-{checkDigit}";

        return withHyphens;
    }

    private (string Gs1Prefix, string Remainder) GetGs1PrefixAndRemainder(string isbn)
    {
        return isbn.Length switch
        {
            13 => (isbn[..3], isbn[3..]),
            10 => ("978", isbn),
            _ => throw new ArgumentException("Length must be 10 or 13")
        };
    }

    private int? FindRuleLength(string input, List<Rule>? rules, bool padTo7 = false)
    {
        if (rules == null || input.Length < 7) return null;

        var comparisonValueStr = input[..7];
        if (padTo7)
        {
            comparisonValueStr = comparisonValueStr.PadRight(7, '0');
        }

        var comparisonValue = int.Parse(comparisonValueStr);

        foreach (var rule in rules)
        {
            if (string.IsNullOrEmpty(rule.Range)) continue;
            var parts = rule.Range.Split('-');
            if (parts.Length != 2) continue;

            if (comparisonValue >= int.Parse(parts[0]) && comparisonValue <= int.Parse(parts[1]))
            {
                return rule.Length > 0 ? rule.Length : null;
            }
        }

        return null;
    }
}