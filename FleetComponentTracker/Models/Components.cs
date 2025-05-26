using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetComponentTracker.Models
{
    public class Components
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        [Required]
        public  string SerialNumber { get; set; }

        [StringLength(100)]
        [Required]
        public string Description { get; set; }

        [StringLength(10)]
        [Required]
        public string VehicleNumber { get; set; }

        [Required]
        public DateOnly InstallDate {  get; set; }

        //Creates relationship with the Vehicle table, so VehicleNumber in Components should match VehicleNumber in Vehicles and allows us to see matched fields
        [ForeignKey(nameof(VehicleNumber))]
        public Vehicles? Vehicle { get; set; }

        //Makes it easier to display FleetName in grid without doing another query 
        public string? FleetName => Vehicle?.FleetName;

    }
}
