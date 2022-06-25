namespace Fortune.Models
{
    public class FortuneItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SoundURI { get; set; }
    }
}