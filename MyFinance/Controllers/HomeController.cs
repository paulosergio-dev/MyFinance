using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Models;
using Microsoft.AspNetCore.Http;

namespace MyFinance.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            HomeModel objHomeModel = new HomeModel();
            string nome = objHomeModel.LerNomeUsuario();
            ViewData["Nome"] = nome;

            // ou fazer somente esse//
            //ViewData["Nome"] = new HomeModel().LerNomeUsuario();
            // ou fazer somente esse//

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Menu()
        {
            return View();
        }

        public IActionResult Ajuda()
        {
            return View();
        }
    }
}
