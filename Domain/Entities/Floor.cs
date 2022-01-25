namespace Domain.Entities
{
    public class Floor
    {
        public int Id { get; set; }
        public ICollection<WorkPlace> WorkPlaces { get; set; }
        public int FloorNumber { get; set; }
    }
}