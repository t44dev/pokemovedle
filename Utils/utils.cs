using System.Text.Json;
using System.Text.Json.Serialization;

namespace PokeMovedle.Utils
{

    public sealed class Globals
    {
        public static readonly HttpClient client = new HttpClient();
        public static readonly Random random = new Random();
    }

    public class NamedEnumField<T> where T : Enum
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required T name { get; init; }

    }

}
