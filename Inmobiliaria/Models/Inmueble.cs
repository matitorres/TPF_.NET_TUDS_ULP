using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Inmueble
    {
        public int Id { get; set; }
        [Required]
        public string Latitud { get; set; }
        [Required]
        public string Longitud { get; set; }
        [Required]
        public string Tipo { get; set; }
        [Required]
        public string Uso { get; set; }
        [Required]
        public int Ambientes { get; set; }
        [Required]
        public string Direccion { get; set; }
        [Required]
        public Decimal Precio { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaAlta { get; set; }
        public Propietario Propietario { get; set; }
    }
}
