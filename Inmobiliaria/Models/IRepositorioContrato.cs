using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        IList<Contrato> BuscarPorInquilino(int id);
        IList<Contrato> BuscarPorInmueble(int id);
        IList<Contrato> BuscarVigentes();
        Contrato ObtenerPorIdRenovar(int id);
    }
}
