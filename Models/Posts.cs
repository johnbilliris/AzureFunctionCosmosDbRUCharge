using System;
using Newtonsoft.Json;

namespace Company.Function.Models
{
    public class PostsModel
    {
        [JsonProperty("id")]
        public string Id {get; set;}

        [JsonProperty("CreatedOn")]
        public DateTime CreatedOn {get; set;}

        [JsonProperty("DeletedOn")]
        public DateTime? DeletedOn {get; set;}

        [JsonProperty("CreatorId")]
        public string CreatorId { get; set;}
    }
}