namespace loja.models{

    public class Cliente{

        public int Id {get; set;}
        public required String Nome {get; set;}
        public required String Cpf {get; set;}
        public required String Email {get; set;}
    }
}