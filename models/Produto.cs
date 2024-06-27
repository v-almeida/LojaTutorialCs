namespace loja.models{

    public class Produto{

        public int Id {get; set;}
        public required String Nome {get; set;}

        public Double Preco {get; set;}

        public required String Fornecedor {get; set;}
    }
}