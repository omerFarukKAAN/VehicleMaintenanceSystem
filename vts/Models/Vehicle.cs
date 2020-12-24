using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace vts.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string LicensePlate { get; set; }
        public string Transmission { get; set; }
        public string Color { get; set; }
        public string FuelType { get; set; }
        public int Year { get; set; }
        public int Horsepower { get; set; }
        public int EngineCapacity { get; set; }
        public int Kilometer { get; set; }
        public List<Maintenance> Maintenances { get; set; }
    }
}
