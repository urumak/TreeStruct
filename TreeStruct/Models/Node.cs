using System.Collections.Generic;

namespace TreeStruct.Models
{
    //klasa węzła
    //na podstwie jej właściwośi, tworzona jest tabela w bazie danych
    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public Node Parent { get; set; }
        public List<Node> Children { get; set; }
    }
}