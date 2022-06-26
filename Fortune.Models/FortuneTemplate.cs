using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fortune.Models
{
    public class FortuneTemplate
    {
        [Key]
        [Required]
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<FortuneItem> Items { get; set; }
    }
}