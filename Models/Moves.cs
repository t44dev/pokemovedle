using PokeMovedle.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PokeMovedle.Models.Moves
{

    public sealed class MoveManager
    {
        private static MoveManager? instance { get; set; } = null;
        public static List<MinimalMove>? moves { get; private set; } = new List<MinimalMove>();
        private static string MOVES_FILE_NAME = "./data/moveNames.json";
        public static MoveFetcher moveFetcher { private get; set; } = new PokeAPIMoveFetcher();

        public Move? move { get; private set; }
        private DateTime lastTimestamp { get; set; }

        private MoveManager(Move? startMove)
        {
            move = startMove;
        }

        public static async Task<MoveManager> Instance()
        {
            if (instance == null)
            {
                instance = new MoveManager(await moveFetcher.fetchNewMove());
                using FileStream stream = File.OpenRead(MOVES_FILE_NAME);
                moves = await JsonSerializer.DeserializeAsync<List<MinimalMove>>(stream);
            }
            return instance;
        }

        private static DateTime newTimestamp()
        {
            return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, DateTimeKind.Utc);
        }

    }

    public class Move
    {
        public required string name { get; init; }
        public required int? power { get; init; }
        public required int pp { get; init; }
        public required int? accuracy { get; init; }

        public required NamedEnumField<PokeType> type { get; set; }

        [JsonPropertyName("damage_class")]
        public required NamedEnumField<DamageClass> damageClass { get; set; }

    }

    public class MinimalMove
    {
        public required string name { get; init; }
        public required int id { get; init; }
    }

    public interface MoveFetcher
    {
        Task<Move?> fetchNewMove();
    }

    public class DummyMoveFetcher : MoveFetcher
    {
        private Move? move { get; set; } = null;
        private static string FILE_NAME = "./data/dummyMove.json";

        public async Task<Move?> fetchNewMove()
        {
            if (move != null) return move;

            using FileStream stream = File.OpenRead(FILE_NAME);
            Move? deserializedMove = await JsonSerializer.DeserializeAsync<Move>(stream);
            move = deserializedMove;
            return move;
        }
    }

    public class PokeAPIMoveFetcher : MoveFetcher
    {
        public async Task<Move?> fetchNewMove()
        {
            List<MinimalMove>? moves = MoveManager.moves;
            if (moves == null) throw new InvalidOperationException("Null moves list.");

            MinimalMove moveData = moves[Globals.random.Next(moves.Count)];

            HttpResponseMessage res = await Globals.client.GetAsync($"https://pokeapi.co/api/v2/move/{moveData.id}");
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<Move>();
            }
            return null;
        }
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
        FAIRY
    }

}
