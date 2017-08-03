using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using DataAccess;
namespace LogicAccess
{
    public class LogicAcceso
    {

        public int eliminarUsuario(string usuario)
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.eliminarUsuario(usuario);

        }
        public Usuario ValidarAcceso(string usuario)
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.ValidarAcceso(usuario);

        }
        public Usuario Login(string usuario,string password)
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.Login(usuario,password);
        }

        public List<Perfil> getPerfiles() {
            DAOAcceso dao = new DAOAcceso();
            return dao.getPerfiles();
        }

        public List<Acceso> getAccesosDePerfil(int idPerfil) {
            DAOAcceso dao = new DAOAcceso();
            return dao.getAccesos(idPerfil);
        }

        public List<Acceso> getAccesos()
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.getAccesos();
        }

        public int crearPrefil(string nombre, string codPerfil)
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.crearPrefil(nombre,codPerfil);
        }

        public int EliminarPerfil(string codPerfil)
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.EliminarPerfil( codPerfil);
        }

        public int GuardarAccesos(string codPerfil, string accesos)
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.GuardarAccesos(codPerfil, accesos);
        }

        public List<Usuario> getUsuariosSistema(int idSede)
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.getUsuariosSistema(idSede);
        }

        public Usuario getUsuarioSistema(string usuario)
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.getUsuarioSistema(usuario);
        } 
        public List<Area> getAreasUsuariosSistema(int idSede , string codusuario)
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.getAreasUsuariosSistema(idSede,codusuario);
        }
        public int insAreasUsuario(int idSede,string areas, string usuario, string apepausuario, string apemausuario, string nombresusuario, string emailusuario)
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.insAreasUsuario(idSede,areas, usuario,apepausuario,apemausuario,nombresusuario,emailusuario);
        }

        public int getSedeUsuario(string usuario)
        {
            DAOAcceso dao = new DAOAcceso();
            return dao.getSedeUsuario(usuario);
        }
    }
}
