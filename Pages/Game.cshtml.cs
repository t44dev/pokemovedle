using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeMovedle.Models.Moves;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace PokeMovedle.Pages;

public class GameModel : PageModel
{

    public static int guesses { get; private set; } = 0;
    public static readonly int MAX_GUESSES = 6;
    public static Move? lastMove = null;

    public void OnGet()
    {
        guesses = 0;
        lastMove = null;
    }

    public async Task<IActionResult> OnGetRow([FromQuery(Name = "guess")] string? guess)
    {
        guess = guess.ToLower().Replace(" ", "-");
        Move? guessedMove = await MoveManager.moveFetcher.fetch(guess);
        if (guessedMove == null) return StatusCode(404);

        lastMove = guessedMove;
        return Partial("_Row", this);
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

        IEnumerable<string> moveNames = MoveManager.moves
            .ConvertAll<string>(m => m.FormatName())
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
