using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using Data;
using System.Data;
using Oracle.DataAccess.Client;

namespace DataAccess
{
    public class DAOAcceso
    {

        public Usuario ValidarAcceso(string usuario) {
            Usuario userRpta = null;

            try
            {
                AutenticarService.Autenticar aut = new AutenticarService.Autenticar();

                AutenticarService.RespuestaBE rpta = aut.LogOn(usuario, "", false);
                if (rpta.IdUsuario > 0)
                {
                    DataTable st = aut.GetOptionsByProfile(1, rpta.IdUsuario);
                }
                else
                {
                    rpta = aut.LogOn(usuario, "", false);

                }
                if (rpta != null)
                {
                    DataTable rptaperfil = rpta.MiPerfil;

                    userRpta = new Usuario();
                    userRpta.perfiles = new List<Perfil>();
                    foreach (DataRow fila in rptaperfil.Rows)
                    {
                        userRpta.idUsuario = fila["IdUsuario"].ToString();
                        userRpta.usuario = fila["Login"].ToString().ToLower();
                        userRpta.Nombres = fila["ApellidosyNombres"].ToString();
                        userRpta.numeroPersonal = fila["NroPersonal"].ToString();
                        userRpta.area = new Area();
                        userRpta.area.codArea = fila["idArea"].ToString();
                        userRpta.area.desArea = fila["NombreArea"].ToString();
                        userRpta.area.sede = new Sede();
                        userRpta.area.sede.codSede = int.Parse(fila["idCentroOperativo"].ToString());
                        userRpta.area.sede.desSede = fila["NombreCentroOperativo"].ToString();
                        userRpta.centroCosto = new CentroCosto();
                        userRpta.centroCosto.idCentro = int.Parse(fila["idCentroCosto"].ToString());
                        userRpta.centroCosto.nroCentro = fila["NroCentroCosto"].ToString();
                        userRpta.centroCosto.nombre = fila["NombreCentroCosto"].ToString();
                        userRpta.grupoCentroCosto = new GrupoCentroCosto();
                        userRpta.grupoCentroCosto.idGrupoCentro = int.Parse(fila["idGrupoCentroCosto"].ToString());
                        userRpta.grupoCentroCosto.nombre = fila["NombreGrupoCentroCosto"].ToString();
                        Perfil perfil = new Perfil();
                        perfil.idPerfil = int.Parse(fila["IdPerfil"].ToString());
                        perfil.nombre = fila["Perfil"].ToString();
                        perfil.accesos = getAccesos(perfil.idPerfil);
                        userRpta.perfiles.Add(perfil);
                        userRpta.existeBD = (getUsuarioSistema(userRpta.usuario) != null) ? 1 : 0;

                    }
                }
            }
            catch (Exception s)
            {

            }
            //userRpta.nivelesAprobacion;

            //*************************************

            return userRpta;
        }



        public Usuario Login(string usuario, string password)
        {
            Usuario userRpta = null;

            try
            {
                //AutenticarService.Autenticar aut = new AutenticarService.Autenticar();

                AutenticarService.RespuestaBE rpta = null; // = aut.LogOn(usuario, password, false);
                //if (rpta.IdUsuario > 0)
                //{
                //    DataTable st = aut.GetOptionsByProfile(1, rpta.IdUsuario);
                //}
                //else {
                //   rpta = aut.LogOn(usuario, password, false);

                //}
                if (rpta == null)
                {
                    //DataTable rptaperfil = rpta.MiPerfil;

                    userRpta = new Usuario();
                    userRpta.perfiles = new List<Perfil>();
                    //foreach (DataRow fila in rptaperfil.Rows)
                    //{
                    userRpta.idUsuario = "490";
                    userRpta.usuario = "consultor3";
                    userRpta.Nombres = "BOGGIO RODRIGUEZ LUIS GUILLERMO";
                    userRpta.numeroPersonal = "5883";
                    userRpta.area = new Area();
                    userRpta.area.codArea = "2124";
                    userRpta.area.desArea = "DIVISION DE DESARROLLO";
                    userRpta.area.sede = new Sede();
                    userRpta.area.sede.codSede = 1;
                    userRpta.area.sede.desSede = "PERU";
                    userRpta.centroCosto = new CentroCosto();
                    userRpta.centroCosto.idCentro = 2286;
                    userRpta.centroCosto.nroCentro = "400716";
                    userRpta.centroCosto.nombre = "DIVISION DE DESARROLLO";
                    userRpta.grupoCentroCosto = new GrupoCentroCosto();
                    userRpta.grupoCentroCosto.idGrupoCentro = 678;
                    userRpta.grupoCentroCosto.nombre = "OFICINA TECNOLOGIAS INFORMACION Y COMUNICACIONES";
                    Perfil perfil = new Perfil();
                    perfil.idPerfil = 103;
                    perfil.nombre = "z";
                    perfil.accesos = getAccesos(perfil.idPerfil);
                    userRpta.perfiles.Add(perfil);

                    //userRpta.idUsuario = fila["IdUsuario"].ToString();
                    //userRpta.usuario = fila["Login"].ToString().ToLower();
                    //userRpta.Nombres = fila["ApellidosyNombres"].ToString();
                    //userRpta.numeroPersonal = fila["NroPersonal"].ToString();
                    //userRpta.area = new Area();
                    //userRpta.area.codArea = fila["idArea"].ToString();
                    //userRpta.area.desArea = fila["NombreArea"].ToString();
                    //userRpta.area.sede = new Sede();
                    //userRpta.area.sede.codSede = int.Parse(fila["idCentroOperativo"].ToString());
                    //userRpta.area.sede.desSede = fila["NombreCentroOperativo"].ToString();
                    //userRpta.centroCosto = new CentroCosto();
                    //userRpta.centroCosto.idCentro = int.Parse(fila["idCentroCosto"].ToString());
                    //userRpta.centroCosto.nroCentro = fila["NroCentroCosto"].ToString();
                    //userRpta.centroCosto.nombre = fila["NombreCentroCosto"].ToString();
                    //userRpta.grupoCentroCosto = new GrupoCentroCosto();
                    //userRpta.grupoCentroCosto.idGrupoCentro = int.Parse(fila["idGrupoCentroCosto"].ToString());
                    //userRpta.grupoCentroCosto.nombre = fila["NombreGrupoCentroCosto"].ToString();
                    //Perfil perfil = new Perfil();
                    //perfil.idPerfil = int.Parse(fila["IdPerfil"].ToString());
                    //perfil.nombre = fila["Perfil"].ToString();
                    //perfil.accesos = getAccesos(perfil.idPerfil);
                    //userRpta.perfiles.Add(perfil);


                    // }
                }
            }
            catch (Exception s)
            {

            }
            //userRpta.nivelesAprobacion;

            //*************************************

            return userRpta;
        }

        public int eliminarUsuario(string usuario)
        {
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "DEL_USUARIO" };
            proc.parametros.Add(new Parametro("VAR_USUARIO", usuario, OracleDbType.Varchar2, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);
            int rpta = 0;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    
                    foreach (DataRow fila in dt.Rows)
                    {

                        return int.Parse(fila["RPTA"].ToString());
                    }
                }
            }


            return rpta;
        }

        public int getSedeUsuario(string usuario)
        {
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "GET_USUARIO_SEDE" };
            proc.parametros.Add(new Parametro("VAR_USUARIO", usuario, OracleDbType.Varchar2, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);
            List<Usuario> rpta = null;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    rpta = new List<Usuario>();
                    foreach (DataRow fila in dt.Rows)
                    {

                        return int.Parse(fila["ID_SEDE"].ToString());
                    }
                }
            }


            return 0;
        }

        public List<Usuario> getUsuariosSistema(int idSede)
        {
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "GET_USUARIOS_SIST" };
            proc.parametros.Add(new Parametro("VAR_ID_SEDE", idSede, OracleDbType.Int32, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);
            List<Usuario> rpta=null;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    rpta = new List<Usuario>();
                    foreach (DataRow fila in dt.Rows)
                    {

                        Usuario user = new Usuario();
                        user.usuario = fila["USUARIO"].ToString();
                        rpta.Add(user);
                    }
                }
            }
            

            return rpta;
        }
        public Usuario getUsuarioSistema(string usuario)
        {
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "GET_USUARIO_SIST" };
            proc.parametros.Add(new Parametro("VAR_USUARIO", usuario, OracleDbType.Varchar2, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);
            Usuario rpta  = null;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    rpta = new  Usuario();
                    foreach (DataRow fila in dt.Rows)
                    {

                        Usuario user = new Usuario();
                        user.usuario = fila["USUARIO"].ToString();
                        user.ApellidoPaterno = fila["APELLIDOPA"].ToString();
                        user.ApellidoMaterno = fila["APELLIDOMA"].ToString();
                        user.Nombres = fila["NOMBRE"].ToString();
                        user.email = fila["EMAIL"].ToString();
                        user.Sede = new Sede() { codSede= int.Parse(  fila["ID_SEDE"].ToString()) };
                        rpta = user;
                    }
                }
            }


            return rpta;
        }

        public List<Area> getAreasUsuariosSistema(int idSede, string codusuario)
        {
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "GET_USUARIOS_SIST_AREAS" };
            proc.parametros.Add(new Parametro("VAR_USER", codusuario, OracleDbType.Varchar2, Parametro.tipoIN));
            proc.parametros.Add(new Parametro("VAR_ID_SEDE", idSede, OracleDbType.Int32, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);
            List<Area> rpta = null;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    rpta = new List<Area>();
                    foreach (DataRow fila in dt.Rows)
                    {

                        Area area = new Area();
                        area.codArea = fila["ID_AREA"].ToString();
                        area.desArea = fila["NOMB_AREA"].ToString();
                        rpta.Add(area);
                    }
                }
            }


            return rpta;
        }

        public int insAreasUsuario(int idSede,string areas,string usuario, string apepausuario, string apemausuario, string nombresusuario, string emailusuario)
        {
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "INS_USUARIOS_SIST_AREAS" };
            proc.parametros.Add(new Parametro("VAR_USER", usuario, OracleDbType.Varchar2, Parametro.tipoIN));
            proc.parametros.Add(new Parametro("VAR_AREAS", areas, OracleDbType.Varchar2, Parametro.tipoIN));
            proc.parametros.Add(new Parametro("VAR_ID_SEDE", idSede, OracleDbType.Int32, Parametro.tipoIN));
            proc.parametros.Add(new Parametro("VAR_APEPA", apepausuario, OracleDbType.Varchar2, Parametro.tipoIN));
            proc.parametros.Add(new Parametro("VAR_APEMA", apemausuario, OracleDbType.Varchar2, Parametro.tipoIN));
            proc.parametros.Add(new Parametro("VAR_NOMS", nombresusuario, OracleDbType.Varchar2, Parametro.tipoIN));
            proc.parametros.Add(new Parametro("VAR_MAIL", emailusuario, OracleDbType.Varchar2, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow fila in dt.Rows)
                    {

                        if (int.Parse(fila["RPTA"].ToString()) > 0)
                        {

                            return 1;
                        }
                        else
                        {

                            return 2;
                        }
                    }
                }
            }
            else
            {
                return 3;
            }

            return 4;
        }

        public int GuardarAccesos(string codPerfil, string accesos)
        {
            try
            {
                int t = int.Parse(codPerfil);
            }
            catch (Exception s)
            {
                return 18;
            }

            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "INS_ACCESO_PERFIL" };
            proc.parametros.Add(new Parametro("VAR_ID_PERFIL", codPerfil, OracleDbType.Int32, Parametro.tipoIN));
            proc.parametros.Add(new Parametro("VAR_ACCESOS", accesos, OracleDbType.Varchar2, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);


            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow fila in dt.Rows)
                    {

                        if (int.Parse(fila["RPTA"].ToString()) > 0)
                        {

                            return 1;
                        }
                        else
                        {

                            return 2;
                        }
                    }
                }
            }
            else
            {
                return 3;
            }

            return 4;
        }

        public int EliminarPerfil(string codPerfil)
        {
            try
            {
                int t = int.Parse(codPerfil);
            }
            catch (Exception s)
            {
                return 18;
            }

            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "DEL_PERFIL" };
            proc.parametros.Add(new Parametro("VAR_ID_PERFIL", codPerfil, OracleDbType.Int32, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);


            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow fila in dt.Rows)
                    {

                        if (int.Parse(fila["RPTA"].ToString()) > 0)
                        {

                            return 1;
                        }
                        else
                        {

                            return 2;
                        }
                    }
                }
            }
            else
            {
                return 3;
            }

            return 4;
        }

        public int crearPrefil(string nombre, string codPerfil)
        {

            try
            {
                int t = int.Parse(codPerfil);
            }
            catch (Exception s) {
                return 18;
            }

            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "NEW_PERFIL" };
            proc.parametros.Add(new Parametro("VAR_NOMBRE", nombre, OracleDbType.Varchar2, Parametro.tipoIN));
            proc.parametros.Add(new Parametro("VAR_ID_PERFIL", codPerfil, OracleDbType.Int32, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);


            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    foreach (DataRow fila in dt.Rows)
                    {

                        if (int.Parse(fila["RPTA"].ToString()) > 0)
                        {

                            return 1;
                        }
                        else
                        {

                            return 2;
                        }
                    }
                }
            }
            else
            {
                return 3;
            }

            return 4;
        }

        public List<Acceso> getAccesos()
        {
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "GET_ACCESOS" };

            DataTable dt = con.EjecutarProcedimiento(proc);

            List<Acceso> listaRpta = null;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    listaRpta = new List<Acceso>();

                    foreach (DataRow fila in dt.Rows)
                    {
                        try
                        {
                            Acceso acc = new Acceso();
                            acc.codigo = (Accesos)int.Parse(fila["ACCESO"].ToString());
                            acc.descripcion = fila["DESC"].ToString();
                            listaRpta.Add(acc);
                        }
                        catch (Exception s)
                        {
                            Console.WriteLine("Error En getAccesos ==> " + s.Message);
                        }
                    }
                }
            }

            return listaRpta;
        }

        public List<Perfil> getPerfiles()
        {
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "GET_PERFILES" };

            DataTable dt = con.EjecutarProcedimiento(proc);

            List<Perfil> listaRpta = null;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    listaRpta = new List<Perfil>();

                    foreach (DataRow fila in dt.Rows)
                    {
                        try
                        {
                            Perfil acc = new Perfil();
                            acc.idPerfil = int.Parse(fila["ID_PERFIL"].ToString());
                            acc.nombre = fila["NOMBRE"].ToString();
                            listaRpta.Add(acc);
                        }
                        catch (Exception s)
                        {
                            Console.WriteLine("Error En getAccesos ==> " + s.Message);
                        }
                    }
                }
            }

            return listaRpta;
        }

        public List<Acceso> getAccesos(int idPerfil) {
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "GET_ACCESO_PERFIL" };
            proc.parametros.Add(new Parametro("VAR_ID_PERFIL", idPerfil, OracleDbType.Int32, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);

            List<Acceso> listaRpta = null;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    listaRpta = new List<Acceso>();

                    foreach (DataRow fila in dt.Rows)
                    {
                        try
                        {
                            Acceso acc = new Acceso();
                            acc.codigo = (Accesos)int.Parse(fila["acceso"].ToString());
                            acc.descripcion = Enum.GetName(typeof(Accesos),acc.codigo);
                            listaRpta.Add(acc);
                        }
                        catch (Exception s)
                        {
                            Console.WriteLine("Error En getAccesos ==> " + s.Message);
                        }
                    }
                }
            }

            return listaRpta;
        }

        public Usuario getUsuario(string usuario)
        {
            Usuario userRpta = new Usuario();
            userRpta.usuario = usuario;           

            return userRpta;
        }
    }
}
