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
            return View();
        }

        [HttpPost]
        public ActionResult Login(Usuario cuenta)
        {
            LogicAcceso logic = new LogicAcceso();
            Session.Timeout = 1440;
            Usuario user =logic.Login(cuenta.usuario, cuenta.password);
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
               
                    ViewBag.idSede = idSede;
                    return View(rpta);
               

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

        public ActionResult insAreasUsuario(int idSede, string areas, string usuario)
        {
            if (Session["usuario"] != null)
            {
                LogicAcceso logic = new LogicAcceso();

                int rpta = logic.insAreasUsuario(idSede, areas, usuario);

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
                ViewBag.codusuario = usuario;
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