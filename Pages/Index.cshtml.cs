using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeMovedle.Models.Moves;

namespace PokeMovedle.Pages;

public class IndexModel : PageModel
{

    public async Task<IActionResult> OnGet()
    {
        return (await MoveManager.Instance()).move == null
            ? StatusCode(500)
            : Page();
    }

    public async Task<IActionResult> OnGetRow([FromQuery(Name = "guess")] string guess, [FromQuery(Name = "guesses")] int guesses)
    {
        if (guess == null) guess = "";
        Move? guessedMove = await MoveManager.moveFetcher.Fetch(Move.DeFormatName(guess));
        if (guessedMove == null) return StatusCode(404);

        return Partial("_Row", new _GameModel(guessedMove, guesses));
    }

    public async Task<IActionResult> OnPostGuess([FromForm(Name = "guess")] string guess, [FromForm(Name = "guesses")] int guesses)
    {
        Move? guessedMove = await MoveManager.moveFetcher.Fetch(Move.DeFormatName(guess));
        if (guessedMove == null) return StatusCode(404);

        return Partial("_Submit", new _GameModel(guessedMove, guesses + 1));
    }

    public IActionResult OnPostOptions([FromForm(Name = "guess")] string pattern)
    {
        if (pattern == null) pattern = "";
        int OPTIONS_LENGTH = 20;

        IEnumerable<string> moveNames = MoveManager.moves
            .ConvertAll<string>(m => Move.FormatName(m.name))
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
