using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeMovedle.Models.Moves;

namespace PokeMovedle.Pages;

public class GameModel : PageModel
{

    public static int guesses { get; private set; } = 0;
    public static readonly int MAX_GUESSES = 6;

    public void OnGet()
    {
        guesses = 0;
    }

    public IActionResult OnPostGuess()
    {
        guesses = Math.Min(guesses + 1, MAX_GUESSES);
        return Partial("_Submit", this);
    }

    public IActionResult OnPostOptions([FromForm(Name = "guess")] string? pattern)
    {
        if (pattern == null) pattern = "";
        int OPTIONS_LENGTH = 20;
        TextInfo tInfo = new CultureInfo("en-GB", false).TextInfo;

        IEnumerable<string> moveNames = MoveManager.moves
            .ConvertAll<string>(m => tInfo.ToTitleCase(m.name.Replace("-", " ")))
            .FindAll(n => n.StartsWith(pattern, true, null))
            .Take(OPTIONS_LENGTH);

        string result = "";
        foreach (string formattedName in moveNames)
        {
            result = result + $"""<option value="{formattedName}"></option>""";
        }

        return Content(result, "text/html");
    }

}
