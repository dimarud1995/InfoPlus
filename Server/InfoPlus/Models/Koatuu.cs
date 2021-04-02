using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoPlus.Models
{
    public class Koatuu
    {
        [Key]
        public int Id { get; set; }
        public string TE { get; set; }
        public string NP { get; set; }
        public string NU { get; set; }
    }
}
