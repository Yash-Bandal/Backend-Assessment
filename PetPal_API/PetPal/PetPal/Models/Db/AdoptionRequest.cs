
using System;

namespace PetPal.Models.Db
{
    public class AdoptionRequest
    {
        public int Id { get; set; }
        public int ApplicantId { get; set; }
        public int PetId { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
