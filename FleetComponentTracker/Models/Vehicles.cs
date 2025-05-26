using System.ComponentModel.DataAnnotations;

namespace FleetComponentTracker.Models
{
    public class Vehicles
    {
        [Key]
        [StringLength(10)]
        [Required]
        public string VehicleNumber { get; set; }

        [StringLength(50)]
        [Required]
        public string FleetName { get; set; }

        [Required]
        public DateOnly DateIntoService { get; set; }

        [Required]
        public DateOnly DateEndsService { get; set; }


    }
}