using Microsoft.AspNetCore.Mvc.RazorPages;
using PokeMovedle.Models.Moves;

namespace PokeMovedle.Pages;

public class _BadgeModel : PageModel
{

    private PokeType attacker { get; set; }
    private PokeType defender { get; set; }
    public float multiplier { get; private set; }

    public _BadgeModel(PokeType attacker, PokeType defender)
    {
        MoveContext ctx = new MoveContext();
        this.attacker = attacker;
        this.defender = defender;
        multiplier = ctx.matchups.Find(attacker, defender).multiplier;
    }
}
