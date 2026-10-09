
using System;

namespace PetPal.Models.Db
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public int BreedId { get; set; }
        public int OwnerId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
