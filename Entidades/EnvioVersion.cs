using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class EnvioVersion
    {

        public int idEnvioVersion { get; set; }
        public DateTime fechaReg { get; set; }
        public Usuario usuarioReg { get; set; }
        public int estado { get; set; }
        public int NivelDesde { get; set; }
        public int NivelHasta { get; set; }
        public Version version { get; set; }

    }
}
