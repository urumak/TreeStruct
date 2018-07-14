using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TreeStruct.Infrastructure;
using TreeStruct.Models;

namespace TreeStruct.Controllers
{
    public class NodeController : Controller
    {
        //tutaj przechowuje id węzłów, które należy rozwinąć
        private static List<string> idsToExpand = new List<string>();
        //tu przechowuję informację o tym, czy węzły należy posortować
        private static bool sort = false;

        //wyświetlanie zawartości wezła
        public ActionResult NodeContent(string id)
        {
            //jeśli nie ma id to strona nie istnieje
            if (id == "" || id == null)
            {
                return View("Error", new string[] { "This page does not exist." });
            }
            //wyszukuję odpowiedni węzeł w bazie i przekazuję go do widoku jako model
            using (var db = new DatabaseContext())
            {
                Node node = db.Nodes.FirstOrDefault(x => x.Id.ToString() == id);
                return View(node);
            }
        }

        public ActionResult Menu()
        {
            using (var db = new DatabaseContext())
            {
                //tworzę listę, na której będą wszystkie węzły
                List<Node> nodes = new List<Node>();
                if (sort)
                {
                    //jeśli trzeba posortowac ot sortuję
                    nodes = db.Nodes.Include("Children").Include("Parent").OrderBy(x => x.Name).ToList();
                }
                else
                {
                    //w przeciwnym wypadku nie sortuję
                    nodes = db.Nodes.Include("Children").Include("Parent").ToList();
                }
                //tworzę słownik, który zawiera informacje i węźle i o tym czy ma być rozwinięty
                //wartość true oznacza rozwinięty, a false zwinięty
                Dictionary<Node, bool> model = new Dictionary<Node, bool>();

                //pętla wpisuje odpowiednie wartości do słownika
                //jeśli id węzła znajduje się na liście to wartość true 
                //w przeciwnym wypadku false
                foreach (Node node in nodes)
                {
                    if (idsToExpand.Contains(node.Id.ToString()))
                    {
                        model.Add(node, true);
                    }
                    else
                    {
                        model.Add(node, false);
                    }
                }
                //zwracam widok częściowy
                return PartialView(model);
            }
        }

        //rozwijanie węzła
        public RedirectResult Expand(string id)
        {
            //dodaję id węzła do listy do rozwinięcia
            idsToExpand.Add(id);
            //warcam na stonę
            return Redirect(Request.UrlReferrer.ToString());
        }

        //zwijanie węzła
        public RedirectResult Collapse(string id)
        {
            //usuwam id węzła z listy
            idsToExpand.Remove(id);
            //wracam na stronę
            return Redirect(Request.UrlReferrer.ToString());
        }

        //rozwijanie wszystkich węzłów
        public RedirectResult ExpandAll()
        {
            //usuwam wszystkie id żeby nie było duplikatów
            idsToExpand.Clear();
            using (var db = new DatabaseContext())
            {
                //dodaje id wszystkich węzłów do listy
                idsToExpand = db.Nodes.Select(x => x.Id.ToString()).ToList();
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        //zwijanie węzłów
        public RedirectResult CollapseAll()
        {
            //czyszczę liste id do rozwinięcia
            idsToExpand.Clear();
            return Redirect(Request.UrlReferrer.ToString());
        }

        //sortowanie może wykonaywać tylko zalogowany użytkownik
        [Authorize]
        public RedirectResult Sort()
        {
            //zmieniam wartosc zmiennej sort na przeciwną
            //metoda może teżwyłączać sortowanie
            sort = !sort;
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}