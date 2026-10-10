using PetPal.Models.Db;

namespace PetPal.Models
{
    public class BestAdopterCandidate
    {
        public User User { get; set; }
        public AdoptionRequest Request { get; set; }
        public int OwnedPetsCount { get; set; }
        public int RejectedCount { get; set; }
        public bool HasAdoptedSameBreed { get; set; }
    }
}
