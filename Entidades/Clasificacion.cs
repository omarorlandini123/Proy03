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
        public int orden { get; set; }
        public string secuencia { get; set; }

        public String getStringHTML(int x,string secue)
        {
            
            String rpta = "<div class=\"col-sm-2\">"+ secue + "</div><div class=\"col-sm-8\" style=\"padding-left:" + 30 * x + "px;\"><strong>" + desLista + "</strong></div><div class=\"col-sm-2\"> ";
            rpta+= "<a href=\"javascript:eliminarClas(" + idLista+ ")\"> Eliminar </a> / <a href=\"javascript:addSubItem(" + idLista + ")\"> SubItem </a></div>";
            if (hijos != null)
            {
                if (hijos.Count > 0)
                {
                    //rpta = rpta + "<div class=\"row\" >";
                    secue += ".";
                    foreach (var item in hijos)
                    {
                        
                        rpta = rpta + item.getStringHTML(x+1, secue + item.secuencia);
                    }
                    //rpta = rpta + "</div>";
                }
            }
            return rpta;
        }

        public String getStringHTMLCombo(int x, string secue,int idLista)
        {
            String rpta = "";
            if (hijos != null) { //si tiene hijos no se puede elegir, por lo tanto se deshabilita
                rpta = "<option disabled = \"disabled\" value = \"0\" style=\"padding-left:" + 30 * x + "px;\" > " +secue +".- "+ this.desLista + "</option>";

                if (hijos.Count > 0)
                {
                    secue += ".";
                    foreach (var item in hijos)
                    {

                        rpta = rpta + item.getStringHTMLCombo(x + 1, secue + item.secuencia,idLista);
                    }
                }
            } else { // si no tiene hijos se habilita porque es un item elegible
                rpta = "<option " + (this.idLista == idLista ? "selected" : "") + " value = " + this.idLista + "  style=\"padding-left:" + 30 * x + "px;\" > " + secue +".- " + this.desLista + " </option>";
            }
            

            return rpta;
        }

        public bool existe { get; set; }
        public double totalCantidad { get; set; }
        public double totalPrecio { get; set; }

        public String getStringHTMLTable(int x, string secue, List<DetalleVersion> detalles) {
             
            String rpta = "<tr style = \"background-color: white;\"><td colspan = \"18\"><strong> "+ secue+ ".- " + this.desLista + " </strong></td></tr>";
            if (hijos != null)
            {
                //rpta = "<option disabled = \"disabled\" value = \"0\" style=\"padding-left:" + 30 * x + "px;\" > " + secue + ".- " + this.desLista + "</option>";

                if (hijos.Count > 0)
                {
                    secue += ".";
                    foreach (var item in hijos)
                    {
                       
                        rpta = rpta + item.getStringHTMLTable(x + 1, secue + item.secuencia, detalles);
                        totalCantidad += item.totalCantidad;
                        totalPrecio += item.totalPrecio;
                    }
                }
            }
            else {
                foreach (DetalleVersion detVer in detalles)
                {

                    if (detVer.tipo == 1 && detVer.clasificacion.idLista == this.idLista)
                    {
                        existe = true;
                        totalCantidad += detVer.cantidadSoli;
                        totalPrecio += detVer.totalSoli;
                        rpta += "<tr><td> " + detVer.mat.codProducto + " - " + @detVer.NombreMaterialSoli + "</td>";
                        rpta += "<td><center> " + detVer.cantidadSoli + " </center></td>";
                        rpta += "<td><center> " + detVer.precioSoli + "</center></td>";
                        rpta += "<td><center> " + detVer.codCentroCosto + "</center></td>";
                        for (int y = 1; y <= 12; y++)
                        {
                            rpta += "<td><center>" + (detVer.contieneMesSoli(y) ? detVer.getCantidadMesSoli(y).ToString() : "") + "</center></td>";
                        }
                        rpta += "<td><center> " + detVer.totalSoli + "</center></td>";
                        rpta += "<td><center> " + detVer.sustento + "</center></td></tr>";
                    }
                }
            }
           


            return rpta;
        }
        public String getStringHTMLTable2(int x, string secue, List<DetalleVersion> detalles, int idPresupuestoTipo)
        {

            String rpta = "<tr style = \"background-color: white;\"><td colspan = \"18\"><strong> " + secue + ".- " + this.desLista + " </strong></td></tr>";
            if (hijos != null)
            {
                //rpta = "<option disabled = \"disabled\" value = \"0\" style=\"padding-left:" + 30 * x + "px;\" > " + secue + ".- " + this.desLista + "</option>";

                if (hijos.Count > 0)
                {
                    secue += ".";
                    foreach (var item in hijos)
                    {

                        rpta = rpta + item.getStringHTMLTable2(x + 1, secue + item.secuencia, detalles,idPresupuestoTipo);
                        totalCantidad += item.totalCantidad;
                        totalPrecio += item.totalPrecio;
                    }
                }
            }
            else
            {
                foreach (DetalleVersion detVer in detalles)
                {

                    if (detVer.tipo == 1 && detVer.clasificacion.idLista == this.idLista)
                    {
                        existe = true;
                        totalCantidad += detVer.cantidadSoli;
                        totalPrecio += detVer.totalSoli;
                        rpta += "<tr><td> " + detVer.mat.codProducto + " - " + @detVer.NombreMaterialSoli + "</td>";
                        rpta += "<td><center> " + detVer.cantidadSoli + " </center></td>";
                        rpta += "<td><center><a href=\"javascript:getCentrosCosto('" + detVer.mat.codProducto + "','"+idPresupuestoTipo+"','"+this.idLista+"')\">"+detVer.codCentroCosto+"</a></center></td>";
                        rpta += "<td><center> "+detVer.totalSoli+"</center></td></tr>";
                        rpta += "<tr style=\"display: none; \" id=\"DETALLE_" + detVer.mat.codProducto + "_" + idPresupuestoTipo + "_" + this.idLista + "\">";
                        rpta += "<td colspan = \"4\" ><div class=\"col-sm-12\" id=\"DETALLEDIV_"+detVer.mat.codProducto+"_"+idPresupuestoTipo+"_"+this.idLista+"\"></div></td></tr>";

                    }
                }
            }



            return rpta;
        }

    }
}
