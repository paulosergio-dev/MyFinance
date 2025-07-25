using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Models;
using MyFinance.Util;
using MyFinance.Services;

namespace MyFinance.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly DAL _dal;
        private readonly IPasswordHashService _passwordHashService;

        public UsuarioController(DAL dal, IPasswordHashService passwordHashService)
        {
            _dal = dal;
            _passwordHashService = passwordHashService;
        }

        [HttpGet]
        public IActionResult Login(int? id)
        {
            if (id != null)
            {
                if (id == 0)
                {
                    HttpContext.Session.SetString("IdUsuarioLogado", string.Empty);
                    HttpContext.Session.SetString("NomeUsuarioLogado", string.Empty);
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ValidarLogin(UsuarioModel usuario)
        {            

            bool login = usuario.ValidarLogin(_dal, _passwordHashService);

            if (login)
            {
                HttpContext.Session.SetString("NomeUsuarioLogado", usuario.Nome);
                HttpContext.Session.SetString("IdUsuarioLogado", usuario.Id.ToString());
                return RedirectToAction("Menu", "Home");
            }
            else
            {
                TempData["MensagemLoginInvalido"] = "Email ou senha incorretos!";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(UsuarioModel usuario)
        {
            if (ModelState.IsValid)
            {
                bool sucesso = usuario.RegistrarUsuario(_dal, _passwordHashService);
                
                if (sucesso)
                {
                    TempData["MensagemSucesso"] = "Usuário registrado com sucesso!";
                    return RedirectToAction("Sucesso");
                }
                else
                {
                    TempData["MensagemErro"] = "Email já existe ou erro no registro!";
                    return View(usuario);
                }
            }
            return View(usuario);
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        public IActionResult Sucesso()
        {
            return View();
        }
    }
}