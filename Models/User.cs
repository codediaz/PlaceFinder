namespace PlaceFinder.Models
{
    public class User
    {
        public int Id { get; set; } 
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public ICollection<SavedPlace> SavedPlaces { get; set; } = new List<SavedPlace>();

    }
}
