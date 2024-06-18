using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeMovedle.Models.Moves;

namespace PokeMovedle.Pages;

public class IndexModel : PageModel
{

    public async Task<IActionResult> OnGet()
    {
        MoveContext ctx = new MoveContext();
        return (await ctx.GetMove()) == null
            ? StatusCode(500)
            : Page();
    }

    private Move? moveFromGuess(string guess, MoveContext ctx) {
        if (guess == null) guess = "";
        guess = Move.DeFormatName(guess);

        try { return ctx.moves.Where(m => m.name == guess).First<Move>(); }
        catch { return null; }
    }

    public IActionResult OnGetRow([FromQuery(Name = "guess")] string guess, [FromQuery(Name = "guesses")] int guesses)
    {
        MoveContext ctx = new MoveContext();

        Move? guessedMove = moveFromGuess(guess, ctx);
        if (guessedMove == null) return StatusCode(404);

        return Partial("_Row", new _GameModel(guessedMove, guesses));
    }

    public IActionResult OnPostGuess([FromForm(Name = "guess")] string guess, [FromForm(Name = "guesses")] int guesses)
    {
        MoveContext ctx = new MoveContext();

        Move? guessedMove = moveFromGuess(guess, ctx);
        if (guessedMove == null) return StatusCode(404);

        return Partial("_Submit", new _GameModel(guessedMove, guesses + 1));
    }

    public IActionResult OnPostOptions([FromForm(Name = "guess")] string pattern)
    {
        MoveContext ctx = new MoveContext();

        if (pattern == null) pattern = "";
        int OPTIONS_LENGTH = 20;
        pattern = Move.DeFormatName(pattern);

        IEnumerable<string> moveNames = ctx.moves
            .Where(m => m.name.Contains(pattern))
            .Take<Move>(OPTIONS_LENGTH)
            .Select(m => Move.FormatName(m.name));

        string result = "";
        foreach (string formattedName in moveNames)
        {
            result = result + $"""<option value="{formattedName}"></option>""";
        }

        return Content(result, "text/html");
    }

}
