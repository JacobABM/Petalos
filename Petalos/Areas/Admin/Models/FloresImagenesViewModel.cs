using Petalos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Petalos.Areas.Admin.Models
{
    public class FloresImagenesViewModel
    {
        public Datosflores Datos { get; set; }
        public IEnumerable<Imagenesflores> Imagenes { get; set; }
    }
}
