using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using TreeStruct.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace TreeStruct.Infrastructure
{
    //stosuję podejści code first
    //klasa kontekstu dla węzłów
    public class DatabaseContext : DbContext
    {
        //wywołuję konstruktor bazowy z nazwą bazy, jaka ma zostać utworzona
        public DatabaseContext() : base("TreeDb") { }
        //w tym kontekście jest tylko dbset zawierający węzły
        public DbSet<Node> Nodes { get; set; }

        //konstruktor statyczny
        //incjalizacja bazy danych
        static DatabaseContext()
        {
            //potrzebne jest stworzenie klasy inicjalizacyjnej
            Database.SetInitializer<DatabaseContext>(new DatabaseInit());
        }
    }

    //klasa inicjalizabyjna dla DatabaseContext
    public class DatabaseInit : DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        //przeciążona metoda seed
        protected override void Seed(DatabaseContext context)
        {
            //metoda tworząca odpowiednie obiekty "na start"
            PerformInitialSetup(context);
            //wywołanie bazowej metosy seed
            base.Seed(context);
        }

        //tworzenie obiektów "na start"
        public void PerformInitialSetup(DatabaseContext context)
        {
            //tworzę kilkanaście obiektów, które od początku będą w bazie
            //dodaję je i zapisuję zmiany
            Node node1 = new Node
            {
                Children = new List<Node>(),
                Parent = null,
                Name = "Node 1",
                Content = "Content"
            };
            context.Nodes.Add(node1);
            context.SaveChanges();

            Node node2 = new Node
            {
                Children = new List<Node>(),
                Parent = null,
                Name = "Node 2",
                Content = "Content"
            };
            context.Nodes.Add(node2);
            context.SaveChanges();

            Node node3 = new Node
            {
                Children = new List<Node>(),
                Parent = null,
                Name = "Node 3",
                Content = "Content"
            };
            context.Nodes.Add(node3);
            context.SaveChanges();

            Node node11 = new Node
            {
                Children = new List<Node>(),
                Parent = node1,
                Name = "Node 1.1",
                Content = "Content"
            };
            node1.Children.Add(node11);
            context.Nodes.Add(node11);
            context.SaveChanges();

            Node node12 = new Node
            {
                Children = null,
                Parent = node1,
                Name = "Node 1.2",
                Content = "Content"
            };
            node1.Children.Add(node12);
            context.Nodes.Add(node12);
            context.SaveChanges();

            Node node111 = new Node
            {
                Children = null,
                Parent = node11,
                Name = "Node 1.2",
                Content = "Content"
            };
            node11.Children.Add(node111);
            context.Nodes.Add(node111);
            context.SaveChanges();

            Node node21 = new Node
            {
                Children = null,
                Parent = node2,
                Name = "Node 2.1",
                Content = "Content"
            };
            node2.Children.Add(node21);
            context.Nodes.Add(node21);
            context.SaveChanges();

            Node node22 = new Node
            {
                Children = null,
                Parent = node2,
                Name = "Node 2.2",
                Content = "Content"
            };
            node2.Children.Add(node22);
            context.Nodes.Add(node22);
            context.SaveChanges();

            Node node31 = new Node
            {
                Children = new List<Node>(),
                Parent = node3,
                Name = "Node 3.1",
                Content = "Content"
            };
            node3.Children.Add(node31);
            context.Nodes.Add(node31);
            context.SaveChanges();

            Node node32 = new Node
            {
                Children = null,
                Parent = node3,
                Name = "Node 3.2",
                Content = "Content"
            };
            node3.Children.Add(node32);
            context.Nodes.Add(node32);
            context.SaveChanges();

            Node node311 = new Node
            {
                Children = null,
                Parent = node31,
                Name = "Node 3.1.1",
                Content = "Content"
            };
            node31.Children.Add(node311);
            context.Nodes.Add(node311);
            context.SaveChanges();

            Node node312 = new Node
            {
                Children = null,
                Parent = node31,
                Name = "Node 3.1.2",
                Content = "Content"
            };
            node31.Children.Add(node312);
            context.Nodes.Add(node312);
            context.SaveChanges();
        }
    }

    //klasa kontekstu dla użytkownika
    public class IdentityDatabaseContext : IdentityDbContext<AppUser>
    {
        //w konstruktorze wywołuję bazowy z nazwą nowej bazy
        public IdentityDatabaseContext() : base("TreeDb") { }

        //statyczny konstruktor odpowiedzialny za incjalizację bazy danych
        static IdentityDatabaseContext()
        {
            Database.SetInitializer<IdentityDatabaseContext>(new IdentityDbInit());
        }

        //metoda statyczna odpowiedzialna za utworzenie instancji klasy
        public static IdentityDatabaseContext Create()
        {
            return new IdentityDatabaseContext();
        }
    }

    //klasa incjalizacyjna dla IdentityDatabaseContext
    public class IdentityDbInit
        : DropCreateDatabaseIfModelChanges<IdentityDatabaseContext>
    {
        //przeciążona metoda seed
        protected override void Seed(IdentityDatabaseContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        //stworzenie jednego użytkownika, który będzie w bazie "na start"
        public void PerformInitialSetup(IdentityDatabaseContext context)
        {
            AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));
            string userName = "Admin";
            string password = "Admin123";
            string email = "admin@test.com";
            AppUser user = userMgr.FindByName(userName);
            if (user == null)
            {
                userMgr.Create(new AppUser { UserName = userName, Email = email }, password);
                user = userMgr.FindByName(userName);
            }
        }
    }
}