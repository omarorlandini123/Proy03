using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using DataAccess;
namespace LogicAccess
{
   public class LogicMaterial
    {
        public List<Material> getMateriales(string cond,string idSede,string clases,string subclases)
        {
            DAOMaterial dao = new DAOMaterial();
            return dao.getMateriales(cond,idSede, clases, subclases);
        }

        public Material getMaterial(string cond,string idSede)
        {
            DAOMaterial dao = new DAOMaterial();
            return dao.getMaterial(cond,idSede);
        }

        public SubClase getSubClase(string cod_subclase) {
            DAOMaterial dao = new DAOMaterial();
            return dao.getSubClase(cod_subclase);
        }

        public List<Material> getServicios(string cond)
        {
            DAOMaterial dao = new DAOMaterial();
            return dao.getServicios(cond);
        }

        public Material getServicio(string cond)
        {
            DAOMaterial dao = new DAOMaterial();
            return dao.getServicio(cond);
        }
    }
}
