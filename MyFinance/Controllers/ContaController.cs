using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Models;
using MyFinance.Util;

namespace MyFinance.Controllers
{
    public class ContaController : Controller
    {
        private readonly DAL _dal;
       
        public ContaController(DAL dal)
        {
            _dal = dal;
        }

        private int? ObterUsuarioLogado()
        {
            string usuarioId = HttpContext.Session.GetString("IdUsuarioLogado");
            if (int.TryParse(usuarioId, out int id))
            {
                return id;
            }
            return null;
        }

        public IActionResult Index()
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            ViewBag.ListaConta = ContaModel.ListaConta(_dal, usuarioId.Value);
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarConta(ContaModel formulario)
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (ModelState.IsValid)
            {
                bool sucesso = formulario.Insert(_dal, usuarioId.Value);
                
                if (sucesso)
                {
                    TempData["MensagemSucesso"] = "Conta criada com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Já existe uma conta com esse nome ou erro na criação!";
                    return View(formulario);
                }
            }
            return View(formulario);
        }

        [HttpGet]
        public IActionResult CriarConta()
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            return View();
        }

        [HttpGet]
        public IActionResult ExcluirConta(int id)
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            bool sucesso = ContaModel.Excluir(_dal, id, usuarioId.Value);
            
            if (sucesso)
            {
                TempData["MensagemSucesso"] = "Conta excluída com sucesso!";
            }
            else
            {
                TempData["MensagemErro"] = "Não foi possível excluir a conta. Verifique se não existem transações associadas.";
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditarConta(int id)
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            ContaModel conta = ContaModel.CarregarRegistro(_dal, id, usuarioId.Value);
            if (conta == null)
            {
                TempData["MensagemErro"] = "Conta não encontrada!";
                return RedirectToAction("Index");
            }

            return View(conta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarConta(ContaModel formulario)
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (ModelState.IsValid)
            {
                bool sucesso = formulario.Atualizar(_dal, usuarioId.Value);
                
                if (sucesso)
                {
                    TempData["MensagemSucesso"] = "Conta atualizada com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao atualizar a conta!";
                    return View(formulario);
                }
            }
            return View(formulario);
        }
    }
}