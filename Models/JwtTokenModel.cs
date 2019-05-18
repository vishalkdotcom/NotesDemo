using Newtonsoft.Json;

namespace NotesDemo.Entities
{
    public class JwtTokenModel
    {
        [JsonProperty(PropertyName = "jwt")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
    }
}