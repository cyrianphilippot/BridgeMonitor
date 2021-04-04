using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pont_Chaban.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace Pont_Chaban.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<rasModel> ras0 = Getras0();
            ras0.Sort((s1, s2) => DateTimeOffset.Compare(s1.dateTotal, s2.dateTotal));

            foreach (var rasModel in ras0)
            {
                if (DateTimeOffset.Compare(DateTimeOffset.Now, rasModel.dateTotal) < 0)
                {
                    ViewData["ras"] = rasModel;
                    break;
                }
            }

            return View();
        }

        public IActionResult All()
        {
            ViewData["ras0"] = Getras0();
            return View();
        }

        public IActionResult Details(DateTime dateTotal)
                {
                    var fermetures = GetFermeturesFromApi();
                    return View(fermetures.FirstOrDefault(f => f.dateTotal == dateTotal));
                }

        //vcube
        private static List<Fermetures> GetFermeturesFromApi()
        {
            // Création d'un HttpClient (=outil qui va permettre d'interroger une URL via une requête HTTP)
            using (var client = new HttpClient())
            {
                //Interrogation de l'URL censée me retourner les données
                var response = client.GetAsync("https://api.alexandredubois.com/pont-chaban/api.php");
                //Récupération du corps de la réponse HTTP sous forme de chaîne de caractères
                var stringResult = response.Result.Content.ReadAsStringAsync();
                //Conversion de mon flux JSON (string) en une collection d'objects Stations
                //d'un flux de données vers des objets => Deserialisation
                //d'objets  vers une flux de données => serialisation
                var result = JsonConvert.DeserializeObject<List<Fermetures>>(stringResult.Result);
                return result;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}