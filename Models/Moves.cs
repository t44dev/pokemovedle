using PokeMovedle.Utils;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace PokeMovedle.Models.Moves
{

    public sealed class MoveManager
    {
        const string MOVES_FILE_NAME = "./data/moveNames.json";

        private static MoveManager? instance { get; set; } = null;
        public static List<MinimalMove> moves { get; private set; } = new List<MinimalMove>();
        public static MoveFetcher moveFetcher { get; set; } = new DummyMoveFetcher();

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
                // Move List
                using FileStream stream = File.OpenRead(MOVES_FILE_NAME);
                List<MinimalMove>? tempMoveList = await JsonSerializer.DeserializeAsync<List<MinimalMove>>(stream);
                if (tempMoveList != null) moves = tempMoveList;

                // Move
                instance = new MoveManager(await NewMove());
            }
            return instance;
        }

        private static DateTime NewTimestamp()
        {
            return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        private static async Task<Move?> NewMove()
        {
            return await moveFetcher.FetchNewMove();
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

    public class MinimalMove
    {
        public required string name { get; init; }
        public required int id { get; init; }
    }

    public interface MoveFetcher
    {
        Task<Move?> FetchNewMove();
        Task<Move?> Fetch(int id);
        Task<Move?> Fetch(string name);
    }

    public class DummyMoveFetcher : MoveFetcher
    {
        const string FILE_NAME = "./data/dummyMove.json";

        private Move? move { get; set; } = null;

        public async Task<Move?> FetchNewMove()
        {
            if (move != null) return move;

            using FileStream stream = File.OpenRead(FILE_NAME);
            Move? deserializedMove = await JsonSerializer.DeserializeAsync<Move>(stream);
            move = deserializedMove;
            return move;
        }

        public Task<Move?> Fetch(int id)
        {
            return Task.FromResult(move);
        }

        public Task<Move?> Fetch(string name)
        {
            return Task.FromResult(move);
        }

    }

    public class PokeAPIMoveFetcher : MoveFetcher
    {

        private IMemoryCache cache { get; init; }

        public PokeAPIMoveFetcher(IMemoryCache cache) { this.cache = cache; }

        public async Task<Move?> FetchNewMove()
        {
            List<MinimalMove> moves = MoveManager.moves;
            if (moves.Count == 0) throw new InvalidOperationException("Empty moves list.");

            MinimalMove moveData = moves[Globals.random.Next(moves.Count)];

            return await Request(moveData.id.ToString());
        }

        public async Task<Move?> Fetch(int id)
        {
            return await Request(id.ToString());
        }

        public async Task<Move?> Fetch(string name)
        {
            return await Request(name);
        }

        private async Task<Move?> Request(string query)
        {
            if (cache.TryGetValue(query, out object? value) && value is Move cachedMove) {
                Console.WriteLine($"Returning cached Move from {query}");
                return cachedMove;
            }

            Console.WriteLine($"API call for Move from {query}");
            HttpResponseMessage res = await Globals.client.GetAsync($"https://pokeapi.co/api/v2/move/{query}");
            if (res.IsSuccessStatusCode) {
                Move apiMove = await res.Content.ReadFromJsonAsync<Move>();
                cache.Set(query, apiMove);
                return apiMove;
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

    public class DamageMap {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("name")]
        public required PokeType type { get; init; }
        [JsonPropertyName("damage_relations")]
        public required Dictionary<string, List<NamedEnumField<PokeType>>> mapping { get; init; }
//        [JsonPropertyName("double_damage_from")]
//        public required List<NamedEnumField<PokeType>> doubleDamageFrom { get; init; }
//        [JsonPropertyName("double_damage_to")]
//        public required List<NamedEnumField<PokeType>> doubleDamageTo { get; init; }
//        [JsonPropertyName("half_damage_from")]
//        public required List<NamedEnumField<PokeType>> halfDamageFrom { get; init; }
//        [JsonPropertyName("half_damage_to")]
//        public required List<NamedEnumField<PokeType>> halfDamageTo { get; init; }
//        [JsonPropertyName("no_damage_from")]
//        public required List<NamedEnumField<PokeType>> noDamageFrom { get; init; }
//        [JsonPropertyName("no_damage_to")]
//        public required List<NamedEnumField<PokeType>> noDamageTo { get; init; }

        public static IMemoryCache cache { private get; set; }

        private static async Task<DamageMap?> fetch(PokeType type) {
            if (cache != null && cache.TryGetValue(type, out object? value) && value is DamageMap cachedDamageMap) {
                Console.WriteLine($"Returning cached DamageMap from {type.ToString()}");
                return cachedDamageMap;
            }

            Console.WriteLine($"API call for DamageMap from {type.ToString()}");
            HttpResponseMessage res = await Globals.client.GetAsync($"https://pokeapi.co/api/v2/type/{type.ToString().ToLower()}");
            if (res.IsSuccessStatusCode) {
                DamageMap? apiDamageMap = await res.Content.ReadFromJsonAsync<DamageMap>();
                if (cache != null) cache.Set(type, apiDamageMap);
                return apiDamageMap;
            }

            return null;
        }

        public static async Task<float> effectiveTo(PokeType attacker, PokeType defender) {
            DamageMap? damageMap = await fetch(attacker);
            if (damageMap == null) return 1.0f;

            Dictionary<string, List<NamedEnumField<PokeType>>> mapping = damageMap.mapping;
            foreach (NamedEnumField<PokeType> type in mapping["double_damage_to"]) if (type.name == defender) return 2.0f;
            foreach (NamedEnumField<PokeType> type in mapping["half_damage_to"]) if (type.name == defender) return 0.5f;
            foreach (NamedEnumField<PokeType> type in mapping["no_damage_to"]) if (type.name == defender) return 0.0f;
            return 1.0f;
        }
    }

}
