using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Petalos.Models.ViewModels
{
    public class FlorViewModel
    {
        public Datosflores Datos { get; set; }
        public IEnumerable<Imagenesflores> Imagenes { get; set; }
        public IEnumerable<Datosflores> Flores { get; set; }
    }
}
