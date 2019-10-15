using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Pago
    {
        public int Id { get; set; }
        [Required]
        public String NumeroPago { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        public decimal Importe { get; set; }
        [Required]
        public Contrato Contrato { get; set; }
    }
}
