using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeMovedle.Models.Moves;

namespace PokeMovedle.Pages;

public class IndexModel : PageModel
{

    public async Task<IActionResult> OnGet()
    {
        MoveContext ctx = new MoveContext();

        return (await ctx.GetMove() == null) ? StatusCode(500) : Page();
    }

    public IActionResult OnGetPersist([FromQuery(Name = "day")] int? day, [FromQuery(Name = "moves")] string? moves)
    {
        MoveContext ctx = new MoveContext();
        if (moves == null) moves = "";

        bool reset = true;
        List<Move> moveList = new List<Move>();
        if (day == MoveContext.GetDay())
        {
            reset = false;
            moveList = movesFromGuesses(moves, ctx);
        }

        return Partial("_Persist", new _PersistModel(reset, moveList));
    }

    private List<Move> movesFromGuesses(string guesses, MoveContext ctx)
    {
        List<Move> moveList = new List<Move>();
        string[] moveStrings = guesses.Split(",");
        foreach (string moveString in moveStrings)
        {
            Move? move = moveFromGuess(moveString, ctx);
            if (move != null) moveList.Insert(0, move);
        }
        return moveList;
    }

    private Move? moveFromGuess(string guess, MoveContext ctx)
    {
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


    public async Task<IActionResult> OnGetModal([FromQuery(Name = "guess")] string guess, [FromQuery(Name = "guesses")] int guesses)
    {
        MoveContext ctx = new MoveContext();

        Move? guessedMove = moveFromGuess(guess, ctx);
        bool gameOver = (guessedMove != null) && ((guessedMove.id == (await ctx.GetMove()).id) || (guesses + 1 >= _GameModel.MAX_GUESSES));
        if ((guessedMove == null) || !gameOver) return StatusCode(404);

        return Partial("_Modal", new _GameModel(guessedMove, guesses + 1));
    }

    public async Task<IActionResult> OnGetResults([FromQuery(Name = "moves")] string? moves)
    {
        MoveContext ctx = new MoveContext();
        Move correctMove = await ctx.GetMove();

        if (moves == null) moves = "";
        List<Move> moveList = movesFromGuesses(moves, ctx);

        // Build result
        // Header
        string result = $"PokeMovedle {MoveContext.GetDay()}\n";
        // Moves
        foreach (Move move in moveList)
        {
            result = result + processMoveResultString(move, correctMove);
        }
        // Attempts
        result = result + $"{moveList.Count()}/{_GameModel.MAX_GUESSES} Attempts";

        return Content(result);
    }

    private string processMoveResultString(Move g, Move c)
    {
        string s<T>(T a, T b) { 
            if (a == null && b == null) return "🟩";
            if (a == null) return "🟥";
            if (b == null) return "🟥";
            return a.Equals(b) ? "🟩" : "🟥";
        }

        return $"{s(g.name,c.name)}{s(g.type,c.type)}{s(g.power,c.power)}{s(g.pp,c.pp)}{s(g.accuracy,c.accuracy)}{s(g.damageClass,c.damageClass)}\n";
    }

}
