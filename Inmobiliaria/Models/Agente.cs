using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Agente
    {
        public int Id { get; set; }
        public String Mail { get; set; }
        public String Clave { get; set; }
        public String Salt { get; set; }
        public String Rol { get; set; }
    }
}
