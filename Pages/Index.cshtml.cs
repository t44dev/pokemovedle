using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeMovedle.Models.Moves;

namespace PokeMovedle.Pages;

public class IndexModel : PageModel
{

    public void OnGet()
    {
    }

    private async Task<Move?> formattedNameToMove(string? guess)
    {
        guess = guess.ToLower().Replace(" ", "-");
        return await MoveManager.moveFetcher.fetch(guess);
    }

    public async Task<IActionResult> OnGetRow([FromQuery(Name = "guess")] string guess, [FromQuery(Name = "guesses")] int guesses)
    {
        if (guess == null) guess = "";
        Move? guessedMove = await formattedNameToMove(guess);
        if (guessedMove == null) return StatusCode(404);

        return Partial("_Row", new _GameModel(guessedMove, guesses));
    }

    public async Task<IActionResult> OnPostGuess([FromForm(Name = "guess")] string guess, [FromForm(Name = "guesses")] int guesses)
    {
        Move? guessedMove = await formattedNameToMove(guess);
        if (guessedMove == null) return StatusCode(404);

        return Partial("_Submit", new _GameModel(guessedMove, guesses + 1));
    }

    public IActionResult OnPostOptions([FromForm(Name = "guess")] string pattern)
    {
        Console.WriteLine(pattern);
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
