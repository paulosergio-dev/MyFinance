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
    public class TransacaoController : Controller
    {
        private readonly DAL _dal;

        public TransacaoController(DAL dal)
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

            ViewBag.ListaTransacao = TransacaoModel.ListaTransacao(_dal, usuarioId.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(TransacaoModel formulario)
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
                    TempData["MensagemSucesso"] = "Transação registrada com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao registrar transação! Verifique se a conta e plano de conta são válidos.";
                }
            }

            ViewBag.ListaContas = ContaModel.ListaConta(_dal, usuarioId.Value);
            ViewBag.ListaPlanoContas = PlanoContaModel.ListaPlanoContas(_dal, usuarioId.Value);
            return View(formulario);
        }

        [HttpGet]
        public IActionResult Registrar(int? id)
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            if (id != null)
            {
                TransacaoModel transacao = TransacaoModel.CarregarRegistro(_dal, id.Value, usuarioId.Value);
                if (transacao == null)
                {
                    TempData["MensagemErro"] = "Transação não encontrada!";
                    return RedirectToAction("Index");
                }
                ViewBag.Registro = transacao;
            }

            ViewBag.ListaContas = ContaModel.ListaConta(_dal, usuarioId.Value);
            ViewBag.ListaPlanoContas = PlanoContaModel.ListaPlanoContas(_dal, usuarioId.Value);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarTransacao(TransacaoModel formulario)
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
                    TempData["MensagemSucesso"] = "Transação atualizada com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao atualizar transação!";
                }
            }

            ViewBag.ListaContas = ContaModel.ListaConta(_dal, usuarioId.Value);
            ViewBag.ListaPlanoContas = PlanoContaModel.ListaPlanoContas(_dal, usuarioId.Value);
            return View(formulario);
        }

        [HttpGet]
        public IActionResult ExcluirTransacao(int id)
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            TransacaoModel transacao = TransacaoModel.CarregarRegistro(_dal, id, usuarioId.Value);
            if (transacao == null)
            {
                TempData["MensagemErro"] = "Transação não encontrada!";
                return RedirectToAction("Index");
            }

            ViewBag.Registro = transacao;
            return View();
        }

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            bool sucesso = TransacaoModel.Excluir(_dal, id, usuarioId.Value);
            
            if (sucesso)
            {
                TempData["MensagemSucesso"] = "Transação excluída com sucesso!";
            }
            else
            {
                TempData["MensagemErro"] = "Erro ao excluir transação!";
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Extrato(TransacaoModel formulario)
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            ViewBag.ListaTransacao = TransacaoModel.ListaTransacao(_dal, usuarioId.Value, 
                formulario.Data, formulario.DataFinal, formulario.Tipo, formulario.Conta_Id, 50);
            ViewBag.ListaContas = ContaModel.ListaConta(_dal, usuarioId.Value);
            return View();
        }

        public IActionResult Dashboard()
        {
            int? usuarioId = ObterUsuarioLogado();
            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            List<DashBoard> lista = DashBoard.RetornarDadosGraficoPie(_dal, usuarioId.Value);
            string valores = "";
            string labels = "";
            string cores = "";
            var random = new Random();

            for (int i = 0; i < lista.Count; i++)
            {
                valores += lista[i].Total.ToString() + ",";
                labels += "'" + lista[i].PlanoConta.ToString() + "',";
                cores += "'" + String.Format("#{0:X6}", random.Next(0x1000000)) + "',";
            }

            ViewBag.Cores = cores;
            ViewBag.Labels = labels;
            ViewBag.Valores = valores;

            return View();
        }
    }
}