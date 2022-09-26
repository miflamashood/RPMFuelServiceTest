using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPMFuelService.Data
{
    public class FuelData
    {
        [Key]
        public int Id { get; set; }
        public string RecordDate { get; set; }
        public float  Price { get; set; }
    }
}
