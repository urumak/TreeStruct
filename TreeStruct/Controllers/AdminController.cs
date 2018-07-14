using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TreeStruct.Models;
using TreeStruct.Infrastructure;

namespace TreeStruct.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //dodawanie węzła
        public ActionResult AddNode(string id)
        {
            AddViewModel model = new AddViewModel();

            //jeśli nie dodaję korzenia
            if (id != "" && id != null)
            {
                //wyszukuję dane rodzica
                using (var db = new DatabaseContext())
                {
                    model.Node = db.Nodes.FirstOrDefault(x => x.Id.ToString() == id);
                }
                //przekazuję dane rodzica do widoku
                return View(model);
            }
            //jeśli tworzę korzeń, to nie ma rodzica, nie przekazuję nic do widoku
            return View();
        }

        [HttpPost]
        public ActionResult AddNode(string id, AddViewModel model)
        {
            //jeśli dane zostały wprowadzone prawidłowo
            if(ModelState.IsValid)
            {
                using (var db = new DatabaseContext())
                {
                    //tworzę nowy węzeł
                    Node node = new Node();
                    node.Name = model.Name;
                    node.Content = model.Content;
                    //jeśli ma rodzica to go zapisuję w Parent
                    if (id != "" && id != null)
                    {
                        node.Parent = db.Nodes.FirstOrDefault(x => x.Id.ToString() == id);
                    }
                    //dodaję węzeł do bazy i zapisuję zmiany
                    db.Nodes.Add(node);
                    db.SaveChanges();

                    //w zależności od tego czy węzeł ma rodzica czy nie przekierowuję do konkretnej akcji
                    if(node.Parent != null)
                    {
                        return RedirectToAction("NodeContent", "Node", new { id = node.Parent.Id });
                    }
                    else
                    {
                        return RedirectToAction("NodeContent", "Node", new { id = node.Id });
                    }
                }
            }
            //jeśli dane zostały wprowadzone nieprawidłowo, to należy je wpisać jeszcze raz
            else
            {
                return View(model);
            }
        }

        //zmiana nazwy węzła
        public ActionResult ChangeName(string id)
        {
            //jeśli w adresie url nie ma żadnego id, to błąd
            if (id == "" || id == null)
            {
                return View("Error", new string[] { "This page does not exist." });
            }

            //tworzę nowy model do którego zapisuję dane węzła, przydadzą się do wyświetlenia informacji
            ChangeNameViewModel model = new ChangeNameViewModel();
            using (var db = new DatabaseContext())
            {
                model.Node = db.Nodes.FirstOrDefault(x => x.Id.ToString() == id);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeName(string id, ChangeNameViewModel model)
        {
            //jeśli wprowadzono poprawne dane
            if (ModelState.IsValid)
            {
                //wyszukuję dany węzeł w bazie danych, zmieniam jego nazwę i zapisuję zmieny
                using (var db = new DatabaseContext())
                {
                    Node node = db.Nodes.FirstOrDefault(x => x.Id.ToString() == id);
                    node.Name = model.Name;
                    db.SaveChanges();
                    //przekierowuję na główną stronę węzła
                    return RedirectToAction("NodeContent", "Node", new { id = node.Id });
                }
            }
            //jeśli dane wprowadzono niepoprawnie, to trzeba je wprowadzić jeszcze raz
            else
            {
                return View(model);
            }
        }

        //przy zmienianiu zawrtości węzła wszystko odbywa się
        //analogicznie jak przy zmianie nazwy
        public ActionResult ChangeContent(string id)
        {
            if (id == "" || id == null)
            {
                return View("Error", new string[] { "This page does not exist." });
            }

            ChangeContentViewModel model = new ChangeContentViewModel();
            using (var db = new DatabaseContext())
            {
                model.Node = db.Nodes.FirstOrDefault(x => x.Id.ToString() == id);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeContent(string id, ChangeContentViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new DatabaseContext())
                {
                    Node node = db.Nodes.FirstOrDefault(x => x.Id.ToString() == id);
                    node.Content = model.Content;
                    db.SaveChanges();
                    return RedirectToAction("NodeContent", "Node", new { id = node.Id });
                }
            }
            else
            {
                return View(model);
            }
        }

        //usuwanie węzła
        public RedirectToRouteResult DeleteNode(string id)
        {
            using (var db = new DatabaseContext())
            {
                //w tej zmiennej zapiszę id rodzica jeśli istnieje
                string tempId = "";
                //wyszukuję węzeł do usunięcia w bazie
                Node node = db.Nodes.Include("Children").Include("Parent").FirstOrDefault(x => x.Id.ToString() == id);
                //jeśli ma rodzica to zapisuję jego id
                if (node.Parent != null)
                {
                     tempId = node.Parent.Id.ToString();
                }
                //usuwam rodzica wraz ze wszystkimi potomkami
                //trzeba to zrobic rekurencyjnie, definicja metody znajduję się na samym dole
                RecursiveDelete(node, db);
                db.SaveChanges();

                //w zależności od tego czy węzeł miał rodzica przekierowuję na konkretną stronę
                if(tempId != "")
                {
                    return RedirectToAction("NodeContent", "Node", new { id = tempId });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        //zmiana położenia węzła
        public ActionResult ChangeNodeLocation(string id)
        {
            //jeśli nie ma id to strona nie istnieje
            if (id == "" || id == null)
            {
                return View("Error", new string[] { "This page does not exist." });
            }
            //zapisuję do modelu odpowienie informacje, które będą potrzebne w widoku
            ChangeLocationViewModel model = new ChangeLocationViewModel();
            using (var db = new DatabaseContext())
            {
                model.Nodes = db.Nodes.Include("Children").Include("Parent").ToList();
                model.Node = db.Nodes.FirstOrDefault(x => x.Id.ToString() == id);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeNodeLocation(string id, ChangeLocationViewModel model)
        {
            using (var db = new DatabaseContext())
            {
                //wyszukuję odpowieni węzeł, jego starego i nowego rodzica
                Node node = db.Nodes.Include("Parent").FirstOrDefault(x => x.Id.ToString() == id);
                Node parent = db.Nodes.Include("Children").FirstOrDefault(x => x.Id == node.Parent.Id);
                Node newParent = db.Nodes.FirstOrDefault(x => x.Id.ToString() == model.NewParentId);
                //usuwam węzeł z listy dzieci starego rodzica
                parent.Children.Remove(node);
                //w zależności od tego czy węzeł ma być teraz korzeniem drzewa, czy ma mieć rodzica
                //odpowienio przypisuję do Parent
                if (model.NewParentId != "" && model.NewParentId != null)
                {
                    node.Parent = newParent;
                }
                else
                {
                    node.Parent = null;
                }
                db.SaveChanges();
                //przkierowuję do strony głównej przenoszonego węzła
                return RedirectToAction("NodeContent", "Node", new { id = node.Id });
            }
        }

        //rekurencyjna metoda pozwalająca na usunięcie węzła waraz z jego potomkami
        private void RecursiveDelete(Node node, DatabaseContext db)
        {
            //jeśli węzeł ma dzieci
            if (node.Children != null)
            {
                //to należy je wyszukać i tak samo rekurencyjnie usunąć ich dzieci
                List<Node> nodes = db.Nodes.Include("Children").Where(x => x.Parent.Id == node.Id).ToList();
                foreach (Node child in nodes)
                {
                    RecursiveDelete(child, db);
                }
            }
            //jeśli węzeł nie ma już dzieci to go usuwam
            db.Nodes.Remove(node);
        }
    }
}