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
    public class DAOArea
    {

        public Area getAreaDeCentroCosto(string centrocosto)
        {
            
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "GET_AREA_COSTO" };
            proc.parametros.Add(new Parametro("VAR_CC", centrocosto, OracleDbType.Varchar2, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);

            Area listaRpta = null;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    listaRpta = new Area();

                    foreach (DataRow fila in dt.Rows)
                    {
                        try
                        {
                            listaRpta.codArea = fila["ID_AREA"].ToString();
                            listaRpta.desArea = fila["NOMB_AREA"].ToString();
                            listaRpta.centrocosto = fila["CC"].ToString();
                            listaRpta.sede = new Sede();
                            listaRpta.sede.codSede = int.Parse(fila["ID_SEDE"].ToString());
                            listaRpta.sede.desSede = fila["NOMB_SEDE"].ToString();
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

        public Area getArea(int idArea)
        {
            
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "GET_AREA" };
            proc.parametros.Add(new Parametro("VAR_ID_AREA", idArea, OracleDbType.Int32, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);

            Area listaRpta = null;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    listaRpta = new Area();

                    foreach (DataRow fila in dt.Rows)
                    {
                        try
                        {
                            listaRpta.codArea = fila["ID_AREA"].ToString();
                            listaRpta.desArea = fila["NOMB_AREA"].ToString();
                            listaRpta.centrocosto = fila["CC"].ToString();
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

        public List<Area> getAreas(int idSede) {
            List<Area> listaRpta = null;
            Conexion con = new Conexion();
            Procedimiento proc = new Procedimiento() { nombre = "GET_AREAS" };
            proc.parametros.Add(new Parametro("VAR_ID_SEDE", idSede, OracleDbType.Int32, Parametro.tipoIN));

            DataTable dt = con.EjecutarProcedimiento(proc);

            

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    listaRpta = new List<Area>();

                    foreach (DataRow fila in dt.Rows)
                    {
                        try
                        {
                            Area area = new Area();
                            area.codArea = fila["ID_AREA"].ToString();
                            area.desArea = fila["NOMB_AREA"].ToString();
                            area.centrocosto = fila["CC"].ToString();
                            listaRpta.Add(area);
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


    }
}
