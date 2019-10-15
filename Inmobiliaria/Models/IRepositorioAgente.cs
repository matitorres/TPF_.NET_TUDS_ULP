using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public interface IRepositorioAgente : IRepositorio<Agente>
    {
        Agente GetPass(string email);
        Agente GetPassPorId(int id);
        int ModificarClave(Agente agente);
    }
}
