using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeMovedle.Models.Moves;

namespace PokeMovedle.Pages;

public class _GameModel : PageModel
{
    public Move? guessedMove { get; private set; }
    public int guesses { get; private set; }
    public const int MAX_GUESSES = 6;

    public _GameModel(Move move, int guesses)
    {
        guessedMove = move;
        this.guesses = Math.Min(guesses, MAX_GUESSES);
    }
}
