using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeMovedle.Models.Moves;

namespace PokeMovedle.Pages;

public class _PersistModel: PageModel
{
    public List<Move> guessedMoves { get; private set; }
    public bool reset { get; private set; }

    public _PersistModel(bool reset, List<Move> moves)
    {
        this.reset = reset;
        this.guessedMoves = moves;
    }
}
