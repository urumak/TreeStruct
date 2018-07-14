using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TreeStruct.Models
{
    //modele zawierają dane potrzebne do przekazywania pomiędzy akcjami i widokami
    //pojawiają się też odpowiednie adnotacje
    //odpowiadające za walidaję danych wejściowych i wyświetlanie informacji
    public class AddViewModel
    {
        public Node Node { get; set; }
        [Required]
        [Display(Name = "Name")]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "Content")]
        public string Content { get; set; }
    }

    public class ChangeNameViewModel
    {
        [Required]
        [Display(Name = "New Name")]
        [StringLength(50)]
        public string Name { get; set; }
        public Node Node { get; set; }
    }

    public class ChangeContentViewModel
    {
        [Display(Name = "New Content")]
        public string Content { get; set; }
        public Node Node { get; set; }
    }

    public class ChangeLocationViewModel
    {
        public Node Node { get; set; }
        public List<Node> Nodes { get; set; }
        public string NewParentId { get; set; }
    }
}