using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Inquilino
    {
        public int Id { get; set; }
        [Required]
        public string Dni { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        [Required, EmailAddress]
        public string Mail { get; set; }
        public string Trabajo { get; set; }
        public DateTime FechaAlta { get; set; }
    }
}
