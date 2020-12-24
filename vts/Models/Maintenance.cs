using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace vts.Models
{
    public class Maintenance
    {
        [Key]
        public int MaintenanceId { get; set; }
        public int VehicleId { get; set; }
        public int MaintenanceKilometer { get; set; }
        public int Cost { get; set; }
        public int NextMaintenanceKilometer { get; set; }
        public string Date { get; set; }
        public string Done { get; set; }
        public string NextMaintenanceDate { get; set; }
        public string Note { get; set; }
    }
}
