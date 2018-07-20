using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppV2.Models;
using LogicAccess;
using Entidades;
namespace AppV2.Controllers
{
    public class LoginController : Controller
    {


        // GET: Login
        public ActionResult Index()
        {
            Session["usuario"] = null;
            if (Session["malInicio"] != null)
            {
                if ((bool)Session["malInicio"])
                {
                    ViewBag.mensaje = "No se han podido validar sus credenciales";
                }
            }

            if (Session["UsuarioNoEnSistema"] != null)
            {
                if ((bool)Session["UsuarioNoEnSistema"])
                {
                    ViewBag.mensaje = "El usuario no se encuentra en el sistema";
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(Usuario cuenta)
        {
            LogicAcceso logic = new LogicAcceso();
            Session.Timeout = 1440;
            Usuario user = logic.Login(cuenta.usuario, cuenta.password);
            if (user != null)
            {
                if (int.Parse(user.idUsuario) > 0)
                {
                    Session["usuario"] = user;
                    if (Session["usuario"] == null)
                    {
                        Session["malInicio"] = true;
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        Usuario userin = logic.getUsuarioSistema(cuenta.usuario);
                        if (userin == null) {
                            Session["UsuarioNoEnSistema"] = true;
                            return RedirectToAction("Index", "Login");
                        }

                        

                        if (((Usuario)Session["usuario"]).tieneAccesoA(Accesos.MostrarSoloArea))
                        {
                            return RedirectToAction("PresupArea", "Presupuesto", new { id = 0 });
                        }
                        else
                        {
                            return RedirectToAction("PorSede", "Presupuesto", new { id = ((Usuario)Session["usuario"]).area.sede.codSede });
                        }

                    }
                }
                else
                {

                    Session["malInicio"] = true;
                    return RedirectToAction("Index", "Login");
                }
            }
            else {
                Session["malInicio"] = true;
                return RedirectToAction("Index", "Login");
            }
        }
        
        
        public ActionResult EsquemaReporte() {
            Session["PestanaConfActiva"] = 3;
            return View();
        }

        public ActionResult ClasesGK()
        {
            Session["PestanaConfActiva"] = 4;
            return View();
        }

        public ActionResult Parametros()
        {
            Session["PestanaConfActiva"] = 5;
            LogicPresupuesto logic = new LogicPresupuesto();

            return View(logic.getParametrosApp());
        }

        public ActionResult GuardarParam(int idParam,string contParam)
        {
            LogicPresupuesto logic = new LogicPresupuesto();

            return PartialView(logic.GuardarParam(idParam,contParam));

        }


        public ActionResult BannerConfiguracion() {
            return PartialView();
        }

        public ActionResult BuscarSedes() {
            LogicPresupuesto logic = new LogicPresupuesto();
            List<Sede> sedes = logic.getSedes();
            return PartialView(sedes);
        }


        public ActionResult BuscarPresupuestos(int idSede) {
            LogicPresupuesto logic = new LogicPresupuesto();
            Usuario user = ((Usuario)Session["usuario"]);
            Sede sede= logic.getPresupuestosPorSede(idSede,user.usuario);
            return PartialView(sede);

        }

        public ActionResult BuscarEsquema(int idSede, int idPresupuesto) {
            LogicPresupuesto logic = new LogicPresupuesto();
           List<Clasificacion> rpta = logic.getEsquemaGastoCapital(idSede, idPresupuesto);
            return PartialView(rpta);
        }

        public ActionResult BuscarClases(int idSede, int idPresupuesto)
        {
            LogicPresupuesto logic = new LogicPresupuesto();
            List<SubClase> rpta = logic.getClasesGastoCapital(idSede, idPresupuesto);
            ViewBag.idSede = idSede;
            ViewBag.idPresupuesto = idPresupuesto;
            return PartialView(rpta);
        }
        public ActionResult AgregarClase(int idSede, int idPresupuesto) {
            ViewBag.idSede = idSede;
            ViewBag.idPresupuesto = idPresupuesto;
            return PartialView();
        }

        public ActionResult InsertClase(int idSede, int idPresupuesto, string cod_subclase) {
            LogicPresupuesto logic = new LogicPresupuesto();
            int rpta = logic.AgregarSubClase(idSede, idPresupuesto, cod_subclase);
            return PartialView(rpta);

        }

        public ActionResult EliminarClase(int idSede, int idPresupuesto, string cod_subclase) {
            LogicPresupuesto logic = new LogicPresupuesto();
            int rpta = logic.EliminarClase(idSede, idPresupuesto, cod_subclase);
            return PartialView(rpta);

        }

        public ActionResult EliminarLista(int idLista) {
            LogicPresupuesto logic = new LogicPresupuesto();
            int rpta = logic.EliminarLista(idLista);
            return PartialView(rpta);
        }

        public ActionResult AgregarItemLista(int idSede,int idPresupuesto, string itemLista) {
            LogicPresupuesto logic = new LogicPresupuesto();
            int rpta = logic.AgregarItemLista(idSede,idPresupuesto, itemLista);
            return PartialView(rpta);
        }

        public ActionResult AgregarItemListaHijo(int idSede, int idPresupuesto, int idPadre, string itemLista) {
            LogicPresupuesto logic = new LogicPresupuesto();
            int rpta = logic.AgregarItemListaHijo(idSede, idPresupuesto,idPadre, itemLista);
            return PartialView(rpta);
        }

        public ActionResult AgregarLista() {
            return PartialView();
        }

        public ActionResult AgregarListaHijo(int idLista) {
            return PartialView(idLista);
        }

        public ActionResult Logout()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Logout(int idUsuario)
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        public ActionResult Configuracion()
        {
            if (Session["usuario"] != null)
            {
                LogicAcceso logic = new LogicAcceso();
                List<Perfil> rpta= logic.getPerfiles();
                if (rpta != null)
                {
                    Session["PestanaConfActiva"] = 1;
                    return View(rpta);
                }else
                {

                    return RedirectToAction("Index", "Login");
                }

            }
            else {
                return RedirectToAction("Index", "Login");
            }

        }

        public ActionResult Areas(int idSede)
        {
            if (Session["usuario"] != null)
            {
                LogicAcceso logic = new LogicAcceso();
                List<Usuario> rpta = logic.getUsuariosSistema(idSede);
                Session["PestanaConfActiva"] = 2;
                    ViewBag.idSede = idSede;
                    return View(rpta);
               

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        public ActionResult EliminarUsuario(string  usuario)
        {
            if (Session["usuario"] != null)
            {
                LogicAcceso logic = new LogicAcceso();
                int rpta = logic.eliminarUsuario(usuario);
                return PartialView(rpta);              

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

       

        public ActionResult MostrarCrearNuevoUsuario() {
            if (Session["usuario"] != null)
            {
                LogicPresupuesto logicPresup = new LogicPresupuesto();

                ViewBag.sedes = logicPresup.getSedes();
                return PartialView();

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        public ActionResult MostrarAreasSinSelec(int idSede)
        {
            if (Session["usuario"] != null)
            {
                LogicArea logArea = new LogicArea();
                List<Area> rptaAreas = logArea.getAreas(idSede);
                return PartialView(rptaAreas);

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }

        [HttpPost]
        public JsonResult ValidarDatosUsuario(string usuario)
        {

            LogicAcceso logic = new LogicAcceso();
            Usuario rpta = logic.ValidarAcceso(usuario);           

            return Json(rpta, JsonRequestBehavior.DenyGet);

        }



        


        public ActionResult insAreasUsuario(int idSede, string areas, string usuario,string apepausuario,string apemausuario,string nombresusuario,string emailusuario)
        {
            if (Session["usuario"] != null)
            {
                int rpta = 0;
                LogicAcceso logic = new LogicAcceso();
                Usuario user = logic.ValidarAcceso(usuario);
                //if (user != null)
                //{
                //    if (int.Parse(user.idUsuario) > 0)
                //    {
                        rpta = logic.insAreasUsuario(idSede, areas, usuario, apepausuario, apemausuario, nombresusuario, emailusuario);
                    //}
                    //else
                    //{
                    //    rpta = 100;
                    //}
                //}
                //else {
                //    rpta = 101;
                //}
               

                return PartialView(rpta);


            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult getAreasUsuario(int idSede,string usuario)
        {
            if (Session["usuario"] != null)
            {
                LogicAcceso logic = new LogicAcceso();
                LogicArea logArea = new LogicArea();
                int idSedeUsuario = (int)logic.getSedeUsuario(usuario);
                List<Area> rptaAreas = logArea.getAreas(idSedeUsuario);
                List<Area> rptaAreasUsuario = logic.getAreasUsuariosSistema(idSedeUsuario, usuario);
                LogicPresupuesto logicPresup = new LogicPresupuesto();
                ViewBag.sedes = logicPresup.getSedes();
                ViewBag.usuarioSel =  logic.getUsuarioSistema(usuario); ;
                ViewBag.idSede = idSedeUsuario;
                ViewBag.rptaAreasUsuario = rptaAreasUsuario;
                
                return PartialView(rptaAreas);               

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

       


        public ActionResult getAccesosDePerfil(int idPerfil) {
           
                LogicAcceso logic = new LogicAcceso();
                List<Acceso> rpta = logic.getAccesos();
                List<Acceso> rpta2 = logic.getAccesosDePerfil(idPerfil);
            if (rpta != null )
            {
                ViewBag.AccesosPerfil = rpta2;
                ViewBag.idPerfil = idPerfil;
                return PartialView(rpta);
            }
            
            return PartialView();


        }

        public ActionResult MostrarCrearPerfil() {
            
            return PartialView();

        }

        public ActionResult CrearPerfil(string nombre, string codPerfil) {
            LogicAcceso login = new LogicAcceso();
           int rpta= login.crearPrefil(nombre,codPerfil);

            return PartialView(rpta);
        }

        public ActionResult EliminarPerfil(string codPerfil) {
            LogicAcceso login = new LogicAcceso();
            int rpta = login.EliminarPerfil(codPerfil);

            return PartialView(rpta);

        }

        public ActionResult GuardarAccesos(string codPerfil,string accesos)
        {
            LogicAcceso login = new LogicAcceso();
            int rpta = login.GuardarAccesos(codPerfil, accesos);
            ViewBag.idPerfil = codPerfil;
            return PartialView(rpta);

        }

    }
}