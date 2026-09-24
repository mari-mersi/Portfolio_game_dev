using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Portfolio_game_dev.Data;
using Portfolio_game_dev.Services.Abstractions;

namespace Portfolio_game_dev.Services.Implementations;

public class SlugService : ISlugService {
    private readonly AppDbContext _db;

    // Простая транслитерация русских букв
    private static readonly Dictionary<char, string> Translit = new() {
        ['а'] = "a", ['б'] = "b", ['в'] = "v", ['г'] = "g", ['д'] = "d",
        ['е'] = "e", ['ё'] = "e", ['ж'] = "zh", ['з'] = "z", ['и'] = "i",
        ['й'] = "y", ['к'] = "k", ['л'] = "l", ['м'] = "m", ['н'] = "n",
        ['о'] = "o", ['п'] = "p", ['р'] = "r", ['с'] = "s", ['т'] = "t",
        ['у'] = "u", ['ф'] = "f", ['х'] = "h", ['ц'] = "ts", ['ч'] = "ch",
        ['ш'] = "sh", ['щ'] = "sch", ['ъ'] = "", ['ы'] = "y", ['ь'] = "",
        ['э'] = "e", ['ю'] = "yu", ['я'] = "ya"
    };

    public SlugService(AppDbContext db) {
        _db = db;
    }

    public string Slugify(string input) {
        if (string.IsNullOrWhiteSpace(input))
            return "untitled";

        var sb = new StringBuilder();
        foreach (var ch in input.ToLowerInvariant()) {
            if (Translit.TryGetValue(ch, out var t))
                sb.Append(t);
            else if (char.IsLetterOrDigit(ch))
                sb.Append(ch);
            else if (ch == ' ' || ch == '-' || ch == '_')
                sb.Append('-');
        }

        var slug = sb.ToString();
        slug = Regex.Replace(slug, "-+", "-");
        slug = slug.Trim('-');

        return string.IsNullOrEmpty(slug) ? "untitled" : slug;
    }

    public async Task<string> GenerateUniqueProjectSlugAsync(
        string title,
        int? excludeId = null,
        CancellationToken ct = default) {
        var baseSlug = Slugify(title);
        var slug = baseSlug;
        var counter = 2;

        while (await _db.Projects.AnyAsync(p =>
                p.Slug == slug && (excludeId == null || p.Id != excludeId), ct)) {
            slug = $"{baseSlug}-{counter}";
            counter++;

            if (counter > 1000)
                throw new InvalidOperationException($"Не удалось создать уникальный slug для '{title}'");
        }

        return slug;
    }
}