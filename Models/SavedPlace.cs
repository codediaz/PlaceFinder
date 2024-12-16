namespace PlaceFinder.Models
{
    public class SavedPlace
    {
        public int Id { get; set; } 

        public int UserId { get; set; }
        public User User { get; set; }

        public string PlaceId { get; set; }
        public Place Place { get; set; }

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;
    }
}
