using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Clasificacion
    {

        public int idLista { get; set; }
        public string desLista { get; set; }
        public Clasificacion padre { get; set; }
        public List<Clasificacion> hijos { get; set; }
        public Sede sede { get; set; }
        public Presupuesto presupuesto { get; set; }
        public String getStringHTML(int x)
        {
            String rpta = "<div class=\"col-sm-10\" style=\"padding-left:"+30*x+"px;\">" + desLista +"</div><div class=\"col-sm-2\"> <a href=\"javascript:eliminarClas("+idLista+")\"> Eliminar </a></div>";
            if (hijos != null)
            {
                if (hijos.Count > 0)
                {
                    //rpta = rpta + "<div class=\"row\" >";
                    foreach (var item in hijos)
                    {
                        rpta = rpta + item.getStringHTML(x+1);
                    }
                    //rpta = rpta + "</div>";
                }
            }
            return rpta;
        }

    }
}
