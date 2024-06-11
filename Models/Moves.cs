using PokeMovedle.Utils;
using System.Text.Json.Serialization;

namespace PokeMovedle.Models.Moves
{

    public sealed class MoveManager
    {
        private static MoveManager? instance = null;
        private DateTime lastTimestamp { set; get; }
        public Move? move { set; get; }

        private MoveManager(Move? startMove)
        {
            move = startMove;
        }

        public static async Task<MoveManager> Instance()
        {
            if (instance == null)
            {
                instance = new MoveManager(await newMove());
            }
            return instance;
        }

        private static DateTime newTimestamp()
        {
            return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        private static async Task<Move?> newMove()
        {
            int ID_MIN = 1;
            int ID_MAX = 748;
            int id = Globals.random.Next(ID_MIN, ID_MAX);
            return await Move.fetch(id);
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


        public static async Task<Move?> fetch(int id)
        {
            return await fetchWith(id.ToString());
        }

        public static async Task<Move?> fetch(string name)
        {
            return await fetchWith(name);
        }

        private static async Task<Move?> fetchWith(string data)
        {
            HttpResponseMessage res = await Globals.client.GetAsync($"https://pokeapi.co/api/v2/move/{data}");
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
