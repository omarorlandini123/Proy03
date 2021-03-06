﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppV2.Models;
using LogicAccess;
using Entidades;
using System.IO;
using AppV2.Reportes;

namespace AppV2.Controllers
{
    public class PresupuestoController : Controller
    {
        // GET: Presupuesto
        public ActionResult Index()
        {
            return View();
        }

        #region "Acciones"

        public ActionResult PorSede(int id) {
            LogicPresupuesto logic = new LogicPresupuesto();
            Usuario user = (Usuario)Session["usuario"];
            
            Session["idSede"] = id;
            if (user == null  )
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");

            }
            else
            {
                if (user.tieneAccesoA(Accesos.MostrarPorSede))
                {
                    if (user.tieneAccesoA(Accesos.MostrarTodasSedes))
                    {

                        return View(logic.getPresupuestosPorSede(id, user.usuario));
                    }
                    else
                    {
                        return View(logic.getPresupuestosPorSede(user.area.sede.codSede, user.usuario));
                    }
                }
                return RedirectToAction("PresupArea", "Presupuesto", new { id = 0 });


            }

        }



        public ActionResult Detalle(int id, int idTipo)
        {
            Usuario user = (Usuario)Session["usuario"];
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");

            }
            else
            {
                if (user.tieneAccesoA(Accesos.MostrarVersionDetalla))
                {
                    LogicPresupuesto logic = new LogicPresupuesto();
                    Entidades.Version version = logic.getVersionDetallada("",id, idTipo, Session["idSede"].ToString());
                    Session["idTipoDetPresup"] = version.presupuestoTipo.tipoPresupuesto.idTipoPresupuesto;
                    Session["idPresupSel"]=version.presupuestoTipo.presupuesto.idPresupuesto;
                    ViewBag.idTipo = idTipo;
                    switch (idTipo)
                    {
                        case 1:
                            ViewBag.TipoDetalles = "Materiales y Suministros";
                            break;
                        case 2:
                            ViewBag.TipoDetalles = "Servicios";
                            break;
                    }



                    return View(version);
                }
                else {
                    return RedirectToAction("Index", "Login");

                }
            }
        }

        public ActionResult DetallesVersion(string cond, int idVersion, int idTipo)
        {
            LogicPresupuesto logicPresup = new LogicPresupuesto();
            Entidades.Version version = logicPresup.getVersionDetallada(cond, idVersion, idTipo, Session["idSede"].ToString());
            return PartialView(version);
        }

        public ActionResult PresupArea(int  id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            LogicArea logArea = new LogicArea();

            Usuario user = (Usuario)Session["usuario"];
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");

            }
            else
            {
                if (user.tieneAccesoA(Accesos.MostrarVersionDetalla))
                {
                    Area area = null;
                    if (id == 0)
                    {
                        area = logArea.getAreaDeCentroCosto(((Usuario)Session["usuario"]).centroCosto.nroCentro);
                        Session["idSede"] = area.sede.codSede;
                        area.presupuestos = logic.getPresupuestosPorSedeLista(area.sede.codSede, int.Parse(area.codArea));
                    }
                    else
                    {
                        area = logArea.getArea(id);
                        area.presupuestos = logic.getPresupuestosPorSedeLista(int.Parse(Session["idSede"].ToString()), id);
                    }

                    return View(area);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
        }


        public ActionResult VerArchivos(int idDetalle)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            return PartialView(logic.getArchivosDetalle(idDetalle));
        }

        public ActionResult EliminarArchivo(int idArchivo, int idDetalle) {

            LogicPresupuesto logic = new LogicPresupuesto();
            ViewBag.idArchivo = idArchivo;
            ViewBag.idDetalle = idDetalle;
            return PartialView(logic.EliminarArchivo(idArchivo));

        }

        public FileResult DescargarArchivo(int idArchivo) {

            LogicPresupuesto logic = new LogicPresupuesto();
            Archivo arc = logic.getArchivo(idArchivo);
            var FileVirtualPath = "~/App_Data/uploads/" + Path.GetFileName(arc.ruta);
            return File(FileVirtualPath, "application/force-download", arc.nombre);

        }

        public ActionResult getDetalleGeneralGastoCapitalPreview(int id) {
            Usuario user = (Usuario)Session["usuario"];
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                LogicPresupuesto logic = new LogicPresupuesto();
                Presupuesto presup = logic.getPresupuestoReporteGeneral(id, Session["idSede"].ToString());
                ViewBag.clasificaciones = logic.getEsquemaGastoCapitalCompleto(id);
                return PartialView(presup);
            }
        }

        public ActionResult getCentrosCosto(string codProducto,int idPresupTipo, int idLista) {
            Usuario user = (Usuario)Session["usuario"];
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                LogicPresupuesto logic = new LogicPresupuesto();
                List<Area> areas = logic.getCentrosCosto(codProducto, idPresupTipo,idLista);
                ViewBag.codProducto = codProducto;
                ViewBag.idPresupTipo = idPresupTipo;
                ViewBag.idLista = idLista;
                return PartialView(areas);
            }
        }

        public ActionResult getDetalleGeneralGastoCapital(int id)

        {
            Usuario user = (Usuario)Session["usuario"];
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var folder = Server.MapPath("~/App_Data/reportes/plantilla_reporte/");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var folderdestino = Server.MapPath("~/App_Data/reportes/" + ((Usuario)Session["usuario"]).usuario + "/");
                if (!Directory.Exists(folderdestino))
                {
                    Directory.CreateDirectory(folderdestino);
                }

                LogicPresupuesto logic = new LogicPresupuesto();
                Presupuesto presup = logic.getPresupuestoReporteGeneral(id, Session["idSede"].ToString());

                string nombreArchivo = "ReporteGeneralGastoCapital.xlsx";
                string nombreDescarga = "Reporte de Gastos de Capital General.xlsx";
                string desde = folder + nombreArchivo;
                string hacia = folderdestino + nombreArchivo;

                ReporteGeneralGastoCapital rp = new ReporteGeneralGastoCapital(desde, hacia, presup, logic.getEsquemaGastoCapital(int.Parse(Session["idSede"].ToString()), id));
                rp.CreateReport();

                return File(hacia, "application/force-download", nombreDescarga);
            }
        }

        public ActionResult getDetalleGeneralGastoFuncionamientoPreview(int id)
        {
            Usuario user = (Usuario)Session["usuario"];
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                LogicPresupuesto logic = new LogicPresupuesto();
                Presupuesto presup = logic.getPresupuestoReporteGeneral(id, Session["idSede"].ToString());
                return PartialView(presup);
            }
        }
        public ActionResult getDetalleGeneralGastoFuncionamiento(int id)
        {
            Usuario user = (Usuario)Session["usuario"];
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var folder = Server.MapPath("~/App_Data/reportes/plantilla_reporte/");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var folderdestino = Server.MapPath("~/App_Data/reportes/" + ((Usuario)Session["usuario"]).usuario + "/");
                if (!Directory.Exists(folderdestino))
                {
                    Directory.CreateDirectory(folderdestino);
                }

                LogicPresupuesto logic = new LogicPresupuesto();
                Presupuesto presup = logic.getPresupuestoReporteGeneral(id, Session["idSede"].ToString());

                string nombreArchivo = "ReporteGeneralGastoFuncionamiento.xlsx";
                string nombreDescarga = "Reporte de Gastos de Funcionamiento General.xlsx";
                string desde = folder + nombreArchivo;
                string hacia = folderdestino + nombreArchivo;

                ReporteGeneralGastoFuncionamiento rp = new ReporteGeneralGastoFuncionamiento(desde, hacia, presup);
                rp.CreateReport();

                return File(hacia, "application/force-download", nombreDescarga);
            }
        }

        public ActionResult getDetalleGeneralGastoCapitalPorAreaPreview(int id, int idArea)
        {
            Usuario user = (Usuario)Session["usuario"];
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                LogicPresupuesto logic = new LogicPresupuesto();
                Presupuesto presup = logic.getPresupuestoReporteGeneralPorArea(id, idArea, Session["idSede"].ToString());
                ViewBag.clasificaciones=logic.getEsquemaGastoCapitalCompleto(id);
                ViewBag.idArea = idArea;
                return PartialView(presup);
            }
        }

        public ActionResult getDetalleGeneralGastoCapitalPorArea(int id,int idArea)
        {
            Usuario user = (Usuario)Session["usuario"];
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var folder = Server.MapPath("~/App_Data/reportes/plantilla_reporte/");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var folderdestino = Server.MapPath("~/App_Data/reportes/" + ((Usuario)Session["usuario"]).usuario + "/");
                if (!Directory.Exists(folderdestino))
                {
                    Directory.CreateDirectory(folderdestino);
                }

                LogicPresupuesto logic = new LogicPresupuesto();
                Presupuesto presup = logic.getPresupuestoReporteGeneralPorArea(id, idArea, Session["idSede"].ToString());

                string nombreArchivo = "ReporteGeneralGastoCapitalPorArea.xlsx";
                string nombreDescarga = "Reporte de Gastos de Capital por Área.xlsx";
                string desde = folder + nombreArchivo;
                string hacia = folderdestino + nombreArchivo;

                ReporteGeneralGastoCapitalPorArea rp = new ReporteGeneralGastoCapitalPorArea(desde, hacia, presup,logic.getEsquemaGastoCapital(int.Parse(Session["idSede"].ToString()),id));
                rp.CreateReport();

                return File(hacia, "application/force-download", nombreDescarga);
            }
        }

        public ActionResult getDetalleGeneralGastoFuncionamientoPorAreaPreview(int id, int idArea)
        {
            Usuario user = (Usuario)Session["usuario"];
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                LogicPresupuesto logic = new LogicPresupuesto();
                Presupuesto presup = logic.getPresupuestoReporteGeneralPorArea(id, idArea, Session["idSede"].ToString());
                ViewBag.idArea = idArea;
                return PartialView(presup);
            }
        }

        public ActionResult getDetalleGeneralGastoFuncionamientoPorArea(int id, int idArea)
        {
            Usuario user = (Usuario)Session["usuario"];
            if (user == null)
            {
                Session.Clear();
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var folder = Server.MapPath("~/App_Data/reportes/plantilla_reporte/");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                var folderdestino = Server.MapPath("~/App_Data/reportes/" + ((Usuario)Session["usuario"]).usuario + "/");
                if (!Directory.Exists(folderdestino))
                {
                    Directory.CreateDirectory(folderdestino);
                }

                LogicPresupuesto logic = new LogicPresupuesto();
                Presupuesto presup = logic.getPresupuestoReporteGeneralPorArea(id, idArea, Session["idSede"].ToString());

                string nombreArchivo = "ReporteGeneralGastoFuncionamientoPorArea.xlsx";
                string nombreDescarga = "Reporte de Gastos de Funcionamiento por Área.xlsx";
                string desde = folder + nombreArchivo;
                string hacia = folderdestino + nombreArchivo;

                ReporteGeneralGastoFuncionamientoPorArea rp = new ReporteGeneralGastoFuncionamientoPorArea(desde, hacia, presup);
                rp.CreateReport();

                return File(hacia, "application/force-download", nombreDescarga);
            }
        }

        public ActionResult SubirArchivo(int idDetalle)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            ViewBag.idDetalle = int.Parse(Request.Form["idDetalle"].ToString());

            string path = "";
            string filename = "";
            string typeFile = "";
            string nombreAleatorio = GenerarCodigo();



            foreach (string upload in Request.Files)
            {
                if (Request.Files[upload].FileName != "")
                {
                    path = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/uploads/";
                    filename = Path.GetFileName(Request.Files[upload].FileName);
                    typeFile = Path.GetExtension(Request.Files[upload].FileName);
                    Request.Files[upload].SaveAs(Path.Combine(path, nombreAleatorio + typeFile));
                }
            }


            return PartialView(logic.subirArchivoaDetalle(idDetalle, filename, typeFile, Path.Combine(path, nombreAleatorio + typeFile), ((Usuario)Session["usuario"]).usuario));
        }

        [HttpPost]
        public ActionResult ActualizarDetalle() {

            DetalleVersion detVersion = new DetalleVersion();
            detVersion.version = new Entidades.Version();
            detVersion.version.idVersion = int.Parse(Request.Form["idVersion"].ToString());
            detVersion.tipo = int.Parse(Request.Form["idTipo"].ToString());
            detVersion.cantidadSoli = double.Parse(Request.Form["cantidad"].ToString());
            detVersion.criticidad = int.Parse(Request.Form["criticidad"].ToString());
            detVersion.prioridad = new Prioridad() { idPrioridad = int.Parse(Request.Form["prioridad"].ToString()) };
            try { 
                detVersion.clasificacion = new Clasificacion() { idLista = int.Parse(Request.Form["clasificacion"].ToString()) };
            }
            catch(Exception s) {
                detVersion.clasificacion = new Clasificacion() { idLista =0 };
            }

            try
            {
                detVersion.Rubro = new Rubro();
                detVersion.Rubro.idRubro = int.Parse(Request.Form["idrubro"].ToString());
            }
            catch (Exception s)
            {
                detVersion.Rubro = new Rubro();
                detVersion.Rubro.idRubro = 0;
            }
            detVersion.largo = double.Parse(Request.Form["largo"].ToString());
            detVersion.ancho = double.Parse(Request.Form["ancho"].ToString());
            detVersion.alto = double.Parse(Request.Form["alto"].ToString());
            detVersion.sustento = Request.Form["sustento"].ToString();
            detVersion.messoli = Request.Form["messoli"].ToString();
            detVersion.messolicant = Request.Form["messolicant"].ToString();
            detVersion.mesent = Request.Form["mesent"].ToString();
            detVersion.precioSoli = double.Parse(Request.Form["preciosoli"].ToString());
            detVersion.uniSoli = Request.Form["uniSoli"].ToString();
            detVersion.mat = new Material();
            detVersion.mat.codProducto = Request.Form["codMaterial"].ToString();
            detVersion.mat.desc = Request.Form["nomMaterial"].ToString();
            detVersion.UsuarioReg = (Usuario)Session["usuario"];
            detVersion.idDetalle = int.Parse(Request.Form["idDetalle"].ToString());
            detVersion.codCentroCosto = ((Usuario)Session["usuario"]).centroCosto.nroCentro;

            LogicPresupuesto logic = new LogicPresupuesto();
            int rpta = logic.actualizarDetalleVersion(detVersion);
            return PartialView(rpta);
        }


        public ActionResult NuevoDetalle()
        {
            string path = "";
            string filename = "";
            string typeFile = "";
            string nombreAleatorio = GenerarCodigo();


            DetalleVersion detVersion = new DetalleVersion();
            foreach (string upload in Request.Files)
            {
                if (Request.Files[upload].FileName != "")
                {
                    path = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/uploads/";
                    filename = Path.GetFileName(Request.Files[upload].FileName);
                    typeFile = Path.GetExtension(Request.Files[upload].FileName);
                    Request.Files[upload].SaveAs(Path.Combine(path, nombreAleatorio + typeFile));
                }
            }


            detVersion.codCentroCosto = ((Usuario)Session["usuario"]).centroCosto.nroCentro;
            detVersion.version = new Entidades.Version();
            detVersion.version.idVersion = int.Parse(Request.Form["idVersion"].ToString());
            detVersion.tipo = int.Parse(Request.Form["idTipo"].ToString());
            detVersion.cantidadSoli = double.Parse(Request.Form["cantidad"].ToString());
            detVersion.criticidad = int.Parse(Request.Form["criticidad"].ToString());
            detVersion.prioridad = new Prioridad() { idPrioridad = int.Parse(Request.Form["prioridad"].ToString()) };
            try
            {
                detVersion.clasificacion = new Clasificacion() { idLista = int.Parse(Request.Form["clasificacion"].ToString()) };
            }
            catch (Exception s)
            {
                detVersion.clasificacion = new Clasificacion() { idLista = 0 };
            }
            detVersion.largo = double.Parse(Request.Form["largo"].ToString());
            detVersion.ancho = double.Parse(Request.Form["ancho"].ToString());
            detVersion.alto = double.Parse(Request.Form["alto"].ToString());
            String sust = Request.Form["sustento"].ToString();
            detVersion.sustento = sust.Length > 500 ? sust.Substring(0, 499) : sust;
            detVersion.messoli = Request.Form["messoli"].ToString();
            detVersion.messolicant= Request.Form["messolicant"].ToString();
            detVersion.mesent = Request.Form["mesent"].ToString();
            detVersion.precioSoli = double.Parse(Request.Form["preciosoli"].ToString());
            detVersion.uniSoli = Request.Form["uniSoli"].ToString();
            try
            {
                detVersion.Rubro = new Rubro();
                detVersion.Rubro.idRubro = int.Parse(Request.Form["idrubro"].ToString());
            }
            catch (Exception s)
            {
                detVersion.Rubro = new Rubro();
                detVersion.Rubro.idRubro = 0;
            }
            

            detVersion.mat = new Material();
            detVersion.mat.codProducto = Request.Form["codMaterial"].ToString();
            String nomMat = Request.Form["nomMaterial"].ToString();
            detVersion.mat.desc = nomMat.Length > 200 ? nomMat.Substring(0, 199) : nomMat;
            detVersion.UsuarioReg = (Usuario)Session["usuario"];
            detVersion.archivosSustento = new List<Archivo>();
            detVersion.archivosSustento.Add(new Archivo() { ruta = Path.Combine(path, nombreAleatorio + typeFile), nombre = filename, tipo = typeFile });

            LogicPresupuesto logic = new LogicPresupuesto();
            int rpta = logic.agregarDetalleVersion(detVersion);
            return PartialView(rpta);
        }


        #endregion

        #region "Vistas parciales"
        public ActionResult AprobacionesPresupuesto(int id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            return PartialView(logic.getAprobacionesPresupuesto(id));
        }

        public ActionResult AprobarDetalle(int idDetalle)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            LogicAcceso logicAcc = new LogicAcceso();
            Usuario userAct = logicAcc.getUsuarioSistema(((Usuario)Session["usuario"]).usuario);

            int rpta = logic.AprobarDetalle(idDetalle, Aprobacion.estados.Aprobado, (userAct.usuario));
            

            DetalleVersion detVer = logic.getDetalleVersion(idDetalle);
            Entidades.Version ver = logic.getVersionDeDetalle(idDetalle);
            Presupuesto presup = logic.getPresupuestoDeVersion(ver.idVersion);
            ver.aprobaciones = logic.getAprobacionesVersion(0,idDetalle).aprobaciones;

            if (rpta == 1) {
                List<Aprobacion> aprobacionesEnv = new List<Aprobacion>();
                foreach (Aprobacion ap in ver.aprobaciones)
                {
                    if (ap.orden == 1 || ap.orden==2)
                    { // solo se envia a los que formulacion la versión
                        aprobacionesEnv.Add(ap);
                    }
                }
                string asunto = "Aprobacion de " + presup.nombrePresupuesto;
                string contenido = "Se ha realizado un cambio en el Presupuesto " +
                    presup.nombrePresupuesto + " en la versión #" + 
                    ver.numeroVersion + "\n El ítem #" + 
                    detVer.idDetalle + " " + detVer.NombreMaterialSoli.Trim() + 
                    "\n ha sido aprobado por " + 
                    userAct.ApellidoPaterno.Trim() + " " + userAct.ApellidoMaterno.Trim() + ", " + userAct.Nombres.Trim();
                logic.EnviarCorreo(userAct.usuario, asunto, contenido, aprobacionesEnv);
            }
            return PartialView(rpta);
        }

        public ActionResult AprobarTodoShow(int id) {
            LogicPresupuesto logic = new LogicPresupuesto();
            Usuario user = (Usuario)Session["usuario"];
            ViewBag.idVersion = id;
            return PartialView();
        }

        public ActionResult AprobarTodo(int id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            LogicAcceso logicAcc = new LogicAcceso();

            Usuario userAct = logicAcc.getUsuarioSistema(((Usuario)Session["usuario"]).usuario);
            //id -> id de la versión 
            RptaServer rpta = logic.AprobarItems(id, userAct.usuario);
            
            Entidades.Version ver = logic.getVersion(id);
            Presupuesto presup = logic.getPresupuestoDeVersion(id);
            ver.aprobaciones = logic.getAprobacionesVersion(id,0).aprobaciones;

            if (rpta.rpta == 1)
            {
                List<Aprobacion> aprobacionesEnv = new List<Aprobacion>();
                foreach (Aprobacion ap in ver.aprobaciones)
                {
                    if (ap.orden == 1 || ap.orden == 2)
                    { // solo se envia a los que formulacion la versión
                        aprobacionesEnv.Add(ap);
                    }
                }
                string asunto = "Aprobacion de " + presup.nombrePresupuesto;
                string contenido = "Se ha realizado un cambio en el Presupuesto " +
                    presup.nombrePresupuesto + " en la versión #" +
                    ver.numeroVersion + "\n Múltiples ítems han sido aprobados por " +
                    userAct.ApellidoPaterno.Trim() + " " + userAct.ApellidoMaterno.Trim() + ", " + userAct.Nombres.Trim();
                logic.EnviarCorreo(userAct.usuario, asunto, contenido, aprobacionesEnv);
            }
            return PartialView(rpta);
        }

        public ActionResult EnviarAprobacion(int id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            LogicAcceso logicAcc = new LogicAcceso();

            Usuario userAct = logicAcc.getUsuarioSistema(((Usuario)Session["usuario"]).usuario);
            int rpta = logic.EnviarAprobacion(id, userAct.usuario);

            Entidades.Version ver = logic.getVersion(id);
            Presupuesto presup = logic.getPresupuestoDeVersion(id);
            ver.aprobaciones = logic.getAprobacionesVersion(id, 0).aprobaciones;

            if (rpta == 1)
            {
                List<Aprobacion> aprobacionesEnv = new List<Aprobacion>();
                foreach (Aprobacion ap in ver.aprobaciones)
                {
                    if (ap.orden == 1 || ap.orden == 2)
                    { // solo se envia a los que formulacion la versión
                        aprobacionesEnv.Add(ap);
                    }
                }
                string asunto = "Aprobacion de " + presup.nombrePresupuesto;
                string contenido = "Se ha realizado un cambio en el Presupuesto " +
                    presup.nombrePresupuesto + " en la versión #" +
                    ver.numeroVersion + "\n Se ha enviado la Versión a Aprobar por " +
                    userAct.ApellidoPaterno.Trim() + " " + userAct.ApellidoMaterno.Trim() + ", " + userAct.Nombres.Trim();
                logic.EnviarCorreo(userAct.usuario, asunto, contenido, aprobacionesEnv);
            }

            return PartialView(rpta);
        }

        public ActionResult AprobarVersion(int id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            LogicAcceso logicAcc = new LogicAcceso();
            Entidades.Version verant = logic.getVersion(id);

            Usuario userAct = logicAcc.getUsuarioSistema(((Usuario)Session["usuario"]).usuario);
            int rpta = logic.AprobarVersion(id, userAct.usuario);

            Entidades.Version ver = logic.getVersion(id);
            Presupuesto presup = logic.getPresupuestoDeVersion(id);
            ver.aprobaciones = logic.getAprobacionesVersion(id, 0).aprobaciones;
            string motivo = "";

            switch(rpta)
            {
                case 1:
                    motivo = "Se aprobó correctamente esta Versión por "+
                    userAct.ApellidoPaterno.Trim() + " " + userAct.ApellidoMaterno.Trim() + ", " + userAct.Nombres.Trim();
                    break;
                case 2:
                    motivo = "Se aprobó correctamente esta Versión por " +
                    userAct.ApellidoPaterno.Trim() + " " + userAct.ApellidoMaterno.Trim() + ", " + userAct.Nombres.Trim()+
                    "\n La versión subió del nivel " +verant.ordenActual +"al nivel "+ver.ordenActual+ " de aprobación";
                    break;                   
            }

            if (rpta == 1 || rpta == 2)
            {
               
                
                string asunto = "Aprobacion de " + presup.nombrePresupuesto;
                string contenido = "Se ha realizado un cambio en el Presupuesto " +
                    presup.nombrePresupuesto + " en la versión #" +
                    ver.numeroVersion + "\n " + motivo;
                logic.EnviarCorreo(userAct.usuario, asunto, contenido, ver.aprobaciones);
            }

            return PartialView(rpta);
        }
        public ActionResult RechazarVersion(int id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            LogicAcceso logicAcc = new LogicAcceso();

            Usuario userAct = logicAcc.getUsuarioSistema(((Usuario)Session["usuario"]).usuario);
            RptaServer rpta = logic.RechazarVersion(id, userAct.usuario);

            Entidades.Version ver = logic.getVersion(id);
            Presupuesto presup = logic.getPresupuestoDeVersion(id);
            ver.aprobaciones = logic.getAprobacionesVersion(id, 0).aprobaciones;

            if ( rpta.rpta == 2)
            {
                
                string asunto = "Aprobacion de " + presup.nombrePresupuesto;
                string contenido = "Se ha realizado un cambio en el Presupuesto " +
                    presup.nombrePresupuesto + " en la versión #" +
                    ver.numeroVersion + "\n La versión ha sido rechazada y se ha regresado al nivel de aprobación anterior por: " +
                    userAct.ApellidoPaterno.Trim() + " " + userAct.ApellidoMaterno.Trim() + ", " + userAct.Nombres.Trim() ;
                logic.EnviarCorreo(userAct.usuario, asunto, contenido, ver.aprobaciones);
            }
            return PartialView(rpta);
        }
        public ActionResult AprobacionesDetallePresupuesto(int id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            return PartialView(logic.getAprobacionesDetallePresupuesto(id));
        }

        public ActionResult AprobacionesVersion(int id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            return PartialView(logic.getAprobacionesVersion(id,0));
        }

        public ActionResult AprobacionesDetalleVersion(int id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            return PartialView(logic.getAprobacionesDetalleVersion(id));
        }

        public ActionResult NuevoAprobadorPresup(int id)
        {

            ViewBag.idPresupuesto = id;
            return PartialView();

        }
        public ActionResult NuevoAprobadorVersion(int id)
        {

            ViewBag.idVersion = id;
            return PartialView();

        }
        public ActionResult AgregarAprobPresup(int id, int idOrden, string idUsuario)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            ViewBag.idPresupuesto = id;
            int rpta = logic.AgregarAprobPresup(id, idOrden, idUsuario, ((Usuario)Session["usuario"]).usuario);
            return PartialView(rpta);
        }
        public ActionResult AgregarAprobVersion(int id, int idOrden, string idUsuario)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            LogicAcceso logicAcc = new LogicAcceso();

            Usuario userAct = logicAcc.getUsuarioSistema(((Usuario)Session["usuario"]).usuario);
            ViewBag.idVersion = id;
            int rpta = logic.AgregarAprobVersion(id, idOrden, idUsuario, userAct.usuario);
            Entidades.Version ver = logic.getVersion(id);
            Presupuesto presup = logic.getPresupuestoDeVersion(id);
            ver.aprobaciones = logic.getAprobacionesVersion(id, 0).aprobaciones;

            if (rpta == 1)
            {

                string asunto = "Aprobacion de " + presup.nombrePresupuesto;
                string contenido = "Se ha realizado un cambio en el Presupuesto " +
                    presup.nombrePresupuesto + " en la versión #" +
                    ver.numeroVersion + "\n El usuario "+ idUsuario + " ha sido agregado como nuevo encargado de nivel "+ idOrden + "por: "+
                    userAct.ApellidoPaterno.Trim() + " " + userAct.ApellidoMaterno.Trim() + ", " + userAct.Nombres.Trim(); 
                logic.EnviarCorreo(userAct.usuario, asunto, contenido, ver.aprobaciones);
            }

            return PartialView(rpta);
        }
        public ActionResult AprobarPresupuesto(int id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            ViewBag.idPresupuesto = id;
            int rpta = logic.AprobarPresupuesto(id, ((Usuario)Session["usuario"]).usuario);
            return PartialView(rpta);
        }
        public ActionResult DesAprobarPresupuesto(int id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            ViewBag.idPresupuesto = id;
            int rpta = logic.DesAprobarPresupuesto(id, ((Usuario)Session["usuario"]).usuario);
            return PartialView(rpta);
        }


        public ActionResult DetalleAprobacion(int id)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            List<DetalleAprobacion> rpta = logic.getDetallesAprobacion(id);
            return PartialView(rpta);
        }

        public ActionResult Observaciones(int idDetalle)
        {
            LogicPresupuesto logicPresup = new LogicPresupuesto();
            return PartialView(logicPresup.getObservacionesDetalle(idDetalle));
        }

        public ActionResult MostrarObservarDetalle(int idDetalle,int idTipo)
        {
            LogicPresupuesto logicPresup = new LogicPresupuesto();

            return PartialView(logicPresup.DetalleDeVersion(idDetalle, idTipo, Session["idSede"].ToString()));

        }

        public ActionResult MostrarCrearPresup(int idSede)
        {
            LogicPresupuesto logicPresup = new LogicPresupuesto();
           
            ViewBag.sedes = logicPresup.getSedes();
            return PartialView();
        }


        public ActionResult CrearPresup(string nombre, int idSede, int mesDesde, int anioDesde, int mesHasta, int anioHasta)
        {
            LogicPresupuesto logicPresup = new LogicPresupuesto();
            return PartialView(logicPresup.CrearPresup(nombre, idSede, ((Usuario)Session["usuario"]).usuario, mesDesde, anioDesde, mesHasta, anioHasta));
        }

        public ActionResult MostrarResolucionObs(int idObservacion)
        {
            LogicPresupuesto logicPresup = new LogicPresupuesto();
            return PartialView(logicPresup.getObservacion(idObservacion));

        }

        public ActionResult MostrarEliminarDetalle(int idDetalle,int idTipo) {

            LogicPresupuesto logicPresup = new LogicPresupuesto();
            return PartialView(logicPresup.DetalleDeVersion(idDetalle, idTipo, Session["idSede"].ToString()));
        }

        public ActionResult EliminarDetalle(int idDetalle) {

            LogicPresupuesto logicPresup = new LogicPresupuesto();
            return PartialView(logicPresup.EliminarDetalle(idDetalle,((Usuario)Session["usuario"]).usuario));
        }


        public ActionResult MostrarResolverObservacion(int idObservacion)
        {
            LogicPresupuesto logicPresup = new LogicPresupuesto();

            return PartialView(logicPresup.getObservacion(idObservacion));

        }

        


        public ActionResult Edit(int id,int idTipo,int idTipoDetPresup)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            ViewBag.prioridades = (List<Prioridad>)logic.getPrioridades();
            ViewBag.rubros = (List<Rubro>)logic.getRubros(idTipo);

            ViewBag.clasificaciones = logic.getEsquemaGastoCapital(id);
            ViewBag.idTipoDetPresup = idTipoDetPresup;
           
            return PartialView(logic.DetalleDeVersion(id, idTipo, Session["idSede"].ToString()));
        }




        public ActionResult Nuevo(int idTipo,int idVersion,int idTipoDetPresup)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            ViewBag.prioridades = (List<Prioridad>)logic.getPrioridades();
            ViewBag.rubros = (List<Rubro>)logic.getRubros(idTipo);
            ViewBag.idTipo = idTipo;
            ViewBag.idTipoDetPresup = idTipoDetPresup;
            ViewBag.clasificaciones = logic.getEsquemaGastoCapitalDeVersion(idVersion);
            return PartialView();
        }

        public ActionResult Versiones(int id, int idArea)
        {

            LogicPresupuesto logic = new LogicPresupuesto();
            DetallePresupuesto presTip = logic.getVersiones(id, idArea);
            
            return PartialView(presTip);

        }

        public ActionResult nuevaVersion(int id, int idArea)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            Usuario user = (Usuario)Session["usuario"];
            int rpta = logic.nuevaVersion(id, idArea, user.usuario);
            return PartialView(rpta);

        }

        public ActionResult TiposPresupuesto(int id, int idArea)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            Presupuesto presup = logic.getPresupuesto(id);
            presup.TiposPresupuestos = logic.getPresupuestoTiposArea(id, idArea);
            ViewBag.idArea = idArea;
            return PartialView(presup);

        }

        public ActionResult PorArea(int idPresupuesto) {
            LogicPresupuesto logicpresup = new LogicPresupuesto();            
            Usuario user = (Usuario)Session["usuario"];
            Presupuesto rpta = logicpresup.getPresupuestosPorArea(idPresupuesto, user.usuario,user.area.sede.codSede);
            return PartialView(rpta);
        }

        public ActionResult getAprobacionesAreas(int idPresupuesto,int idarea) {

            LogicPresupuesto logic = new LogicPresupuesto();
            Usuario user = (Usuario)Session["usuario"];

            List<Aprobacion> rpta = logic.getAprobacionesAreas(idPresupuesto,user.usuario,idarea);
            List<TipoPresupuesto> tipos = null;
            if (rpta != null && rpta.Count > 0) {
                tipos = new List<TipoPresupuesto>();
                foreach (Aprobacion aprob in rpta)
                {
                    if (tipos.Count == 0)
                    {
                        tipos.Add(aprob.tipoPresup.tipoPresupuesto);
                    }
                    else {
                        var contiene = false;
                        foreach (TipoPresupuesto tip in tipos) {
                            if (tip.idTipoPresupuesto == aprob.tipoPresup.tipoPresupuesto.idTipoPresupuesto) {
                                contiene = true;
                            }
                        }
                        if (!contiene) {
                            tipos.Add(aprob.tipoPresup.tipoPresupuesto);
                        }
                    }
                }
            }

            ViewBag.tipos = tipos;        
            return PartialView(rpta);

        }

        public ActionResult PorTipo(int idPresupuesto)
        {
            LogicPresupuesto logicPresup = new LogicPresupuesto();
            Usuario user = (Usuario)Session["usuario"];
            Presupuesto det = logicPresup.getPresupuestosPorTipo(idPresupuesto, user.usuario);
            return PartialView(det);
        }

        public ActionResult getMateriales(string cond)
        {
            LogicMaterial logicMaterial = new LogicMaterial();
            int idTipoPresup = 0;
            int idPresupuestoSel = 0;
            int idSedeSel = 0;
            String clases = "";
            String subclase = "";
            if (Session["idTipoDetPresup"] != null) {
                if (Session["idSede"] != null) {
                    if (Session["idPresupSel"] != null) {
                        try { idTipoPresup = int.Parse(Session["idTipoDetPresup"].ToString()); } catch (Exception f){ idTipoPresup = 0;}
                        try { idSedeSel = int.Parse(Session["idSede"].ToString()); } catch (Exception f){ idSedeSel = 0;}
                        try { idPresupuestoSel = int.Parse(Session["idPresupSel"].ToString()); } catch (Exception f) { idPresupuestoSel = 0; }
                        if (idTipoPresup == 1) {
                            LogicPresupuesto logicPresup = new LogicPresupuesto();
                            List<SubClase> subclases = logicPresup.getClasesGastoCapital(idSedeSel, idPresupuestoSel);
                            if (subclases != null)
                            {
                                if (subclases.Count() > 0)
                                {
                                    foreach (SubClase sub in subclases)
                                    {
                                        subclase += "'" + string.Format("{0:00}",int.Parse(sub.clase.codClase)) +""+ string.Format("{0:00}", int.Parse(sub.codSubClase)) + "',";
                                        if (int.Parse(sub.codSubClase) == 0) {
                                            clases += "'" + string.Format("{0:00}", int.Parse(sub.clase.codClase)) +"',";
                                        }
                                    }
                                    subclase = subclase.Substring(0, subclase.Length - 1);
                                    clases = clases.Substring(0, clases.Length - 1);
                                }
                            }
                        }

                    }
                }
            }
            
            return PartialView(logicMaterial.getMateriales(cond,Session["idSede"].ToString(),  clases, subclase));

        }

        public ActionResult getServicios(string cond)
        {
            LogicMaterial logicMaterial = new LogicMaterial();
            return PartialView(logicMaterial.getServicios(cond));

        }

        #endregion

        #region "Modales"

        public ActionResult AprobarPresup(int id) {
            LogicPresupuesto logic = new LogicPresupuesto();
            return PartialView(logic.AprobacionesPresupuesto(id));
        }

        #endregion

        #region "JSon POST"

        [HttpPost]
        public JsonResult AprobarPresup(int id, string observacion, int estado) {
            LogicPresupuesto logic = new LogicPresupuesto();
            Usuario user = (Usuario)Session["usuario"];
            bool rpta = logic.AprobarPresupuesto(id, observacion, estado, user);
            return Json(rpta, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ObservarDetalle(int idDetalle, string observacion)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            LogicAcceso logicAcc = new LogicAcceso();

            bool rpta = logic.ObservarDetalle(idDetalle, observacion, ((Usuario)Session["usuario"]).usuario);
            
            Usuario userAct = logicAcc.getUsuarioSistema(((Usuario)Session["usuario"]).usuario);
            

            DetalleVersion detVer = logic.getDetalleVersion(idDetalle);
            Entidades.Version ver = logic.getVersionDeDetalle(idDetalle);
            Presupuesto presup = logic.getPresupuestoDeVersion(ver.idVersion);
            ver.aprobaciones = logic.getAprobacionesVersion(0, idDetalle).aprobaciones;

            if (rpta)
            {
                List<Aprobacion> aprobacionesEnv = new List<Aprobacion>();
                foreach (Aprobacion ap in ver.aprobaciones)
                {
                    if (ap.orden == 1 || ap.orden <= ver.ordenActual)
                    { // solo se envia a los que formulacion la versión y a los de orden igual o menor al actual
                        aprobacionesEnv.Add(ap);
                    }
                }
                string asunto = "Observación de " + presup.nombrePresupuesto;
                string contenido = "Se ha realizado un cambio en el Presupuesto " +
                    presup.nombrePresupuesto + " en la versión #" +
                    ver.numeroVersion + "\n El ítem #" +
                    detVer.idDetalle + " " + detVer.NombreMaterialSoli.Trim() +
                    "\n ha sido observado por " +
                    userAct.ApellidoPaterno.Trim() + " " + userAct.ApellidoMaterno.Trim() + ", " + userAct.Nombres.Trim()+"\n"+
                    "===================================================== \n "+observacion+ "\n ===================================================== ";
                logic.EnviarCorreo(userAct.usuario, asunto, contenido, aprobacionesEnv);
            }

            return Json(rpta, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult ResolverObservacion(int idObservacion, string observacion)
        {
            LogicPresupuesto logicPresup = new LogicPresupuesto();
            LogicAcceso logicAcc = new LogicAcceso();
            bool rpta = logicPresup.ResolverObservacion(idObservacion, observacion, ((Usuario)Session["usuario"]).usuario);
            Observacion obs= logicPresup.getObservacion(idObservacion);
            Usuario userAct = logicAcc.getUsuarioSistema(((Usuario)Session["usuario"]).usuario);
            Usuario userObse = logicAcc.getUsuarioSistema(obs.usuarioReg.usuario);

            DetalleVersion detVer = logicPresup.getDetalleVersion(obs.detalle.idDetalle);
            Entidades.Version ver = logicPresup.getVersionDeDetalle(obs.detalle.idDetalle);
            Presupuesto presup = logicPresup.getPresupuestoDeVersion(ver.idVersion);
            ver.aprobaciones = logicPresup.getAprobacionesVersion(0, obs.detalle.idDetalle).aprobaciones;

            if (rpta)
            {
                List<Aprobacion> aprobacionesEnv = new List<Aprobacion>();
                foreach (Aprobacion ap in ver.aprobaciones)
                {
                    if (ap.orden == 1 || ap.orden <= ver.ordenActual)
                    { // solo se envia a los que formulacion la versión y a los de orden igual o menor al actual
                        aprobacionesEnv.Add(ap);
                    }
                }
                string asunto = "Observación de " + presup.nombrePresupuesto;
                string contenido = "Se ha realizado un cambio en el Presupuesto " +
                    presup.nombrePresupuesto + " en la versión #" +
                    ver.numeroVersion + "\n El ítem #" +
                    detVer.idDetalle + " " + detVer.NombreMaterialSoli.Trim() +
                    "\n Observado por " +
                    userObse.ApellidoPaterno.Trim() + " " + userObse.ApellidoMaterno.Trim() + ", " + userObse.Nombres.Trim() + "\n" +
                    "===================================================== \n " + obs.observacion.Trim() + "\n ===================================================== "+
                   "\n Ha sido resuelto por " +
                    userAct.ApellidoPaterno.Trim() + " " + userAct.ApellidoMaterno.Trim() + ", " + userAct.Nombres.Trim() + "\n" +
                    "===================================================== \n " + observacion.Trim() + "\n ===================================================== ";
                logicPresup.EnviarCorreo(userAct.usuario, asunto, contenido, aprobacionesEnv);
            }


            return Json(rpta, JsonRequestBehavior.DenyGet);


        }

        [HttpPost]
        public JsonResult getMaterial(string cond)
        {
            LogicMaterial logicMaterial = new LogicMaterial();
            return Json(logicMaterial.getMaterial(cond, Session["idSede"].ToString()), JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public JsonResult getSubClase(string cod_subclase)
        {
            LogicMaterial logicMaterial = new LogicMaterial();
            return Json(logicMaterial.getSubClase(cod_subclase), JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public JsonResult getServicio(string cond)
        {
            LogicMaterial logicMaterial = new LogicMaterial();
            return Json(logicMaterial.getServicio(cond), JsonRequestBehavior.DenyGet);

        }



        #endregion

        private string GenerarCodigo()
        {
            Random obj = new Random();
            string sCadena = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            int longitud = sCadena.Length;
            char cletra;
            int nlongitud = 20;
            string sNuevacadena = string.Empty;
            for (int i = 0; i < nlongitud; i++)
            {
                cletra = sCadena[obj.Next(nlongitud)];
                sNuevacadena += cletra.ToString();
            }
            return sNuevacadena;
        }

    }

    
}
