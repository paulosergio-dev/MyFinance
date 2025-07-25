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
    public class PlanoContaController : Controller
    {
        private readonly DAL _dal;

        public PlanoContaController(DAL dal)
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

            ViewBag.ListaPlanoContas = PlanoContaModel.ListaPlanoContas(_dal, usuarioId.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CriarPlanoConta(PlanoContaModel formulario)
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
                    TempData["MensagemSucesso"] = "Plano de conta criado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Já existe um plano de conta com essa descrição ou erro na criação!";
                    return View(formulario);
                }
            }
            return View(formulario);
        }

        [HttpGet]
        public IActionResult CriarPlanoConta(int? id)
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (id != null)
            {
                PlanoContaModel planoConta = PlanoContaModel.CarregarRegistro(_dal, id.Value, usuarioId.Value);
                if (planoConta == null)
                {
                    TempData["MensagemErro"] = "Plano de conta não encontrado!";
                    return RedirectToAction("Index");
                }
                return View(planoConta);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPlanoConta(PlanoContaModel formulario)
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
                    TempData["MensagemSucesso"] = "Plano de conta atualizado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao atualizar o plano de conta!";
                    return View(formulario);
                }
            }
            return View(formulario);
        }

        [HttpGet]
        public IActionResult ExcluirPlanoConta(int id)
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            bool sucesso = PlanoContaModel.Excluir(_dal, id, usuarioId.Value);
            
            if (sucesso)
            {
                TempData["MensagemSucesso"] = "Plano de conta excluído com sucesso!";
            }
            else
            {
                TempData["MensagemErro"] = "Não foi possível excluir o plano de conta. Verifique se não existem transações associadas.";
            }
            
            return RedirectToAction("Index");
        }
    }
}