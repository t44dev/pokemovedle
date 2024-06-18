using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PokeMovedle.Models.Moves
{

    public sealed class MoveContext : DbContext
    {
        public DbSet<Move> moves { get; set; }
        public DbSet<TypeMatchup> matchups { get; set; }
        public string dbPath { get; private set; }

        private static Move? move { get; set; }
        public static DateTime lastTimestamp { get; set; } = NewTimestamp();

        public MoveContext()
        {
            dbPath = "./pokedb.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseSqlite($"Data Source={dbPath}");

        private static DateTime NewTimestamp()
        {
            return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        public async Task<Move> GetMove()
        {
            if (move == null || (DateTime.UtcNow - lastTimestamp >= (new TimeSpan(24, 0, 0))))
            {
                // Update Timestamp
                lastTimestamp = NewTimestamp();
                // Update Move
                int moveNumber = Math.Abs(lastTimestamp.GetHashCode()) % await moves.CountAsync<Move>();
                move = await moves.Skip(moveNumber).FirstAsync<Move>();
            }
            return move;
        }

    }

    public class Move
    {
        [Key]
        public required int id { get; init; }
        public required string name { get; init; }
        public required int? power { get; init; }
        public required int? pp { get; init; }
        public required int? accuracy { get; init; }
        public required PokeType type { get; init; }
        public required DamageClass damageClass { get; init; }

        public static string FormatName(string name)
        {
            TextInfo tInfo = new CultureInfo("en-GB", false).TextInfo;
            return tInfo.ToTitleCase(name.Replace("-", " "));
        }

        public static string DeFormatName(string name)
        {
            TextInfo tInfo = new CultureInfo("en-GB", false).TextInfo;
            return name.ToLower().Replace(" ", "-");
        }

    }

    [PrimaryKey(nameof(attacker), nameof(defender))]
    public class TypeMatchup
    {
        public required PokeType attacker { get; init; }
        public required PokeType defender { get; init; }
        public required float multiplier { get; init; }
    }

    public enum DamageClass
    {
        PHYSICAL,
        SPECIAL,
        STATUS
    }

    public enum PokeType
    {
        NORMAL,
        FIRE,
        WATER,
        ELECTRIC,
        GRASS,
        ICE,
        FIGHTING,
        POISON,
        GROUND,
        FLYING,
        PSYCHIC,
        BUG,
        ROCK,
        GHOST,
        DRAGON,
        DARK,
        STEEL,
        FAIRY,
        UNKNOWN,
        SHADOW
    }

}
