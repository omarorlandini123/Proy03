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
        public List<Material> getMateriales(string cond,string idSede)
        {
            DAOMaterial dao = new DAOMaterial();
            return dao.getMateriales(cond,idSede);
        }

        public Material getMaterial(string cond,string idSede)
        {
            DAOMaterial dao = new DAOMaterial();
            return dao.getMaterial(cond,idSede);
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
