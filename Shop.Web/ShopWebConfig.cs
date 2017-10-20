namespace Shop.Web {
    public class ShopWebConfig
    {
        public ConnectionStrings ConnectionStrings { get; set; }
        public JwtIssuerOptions JwtIssuerOptions { get; set; }
        public NodeOptions NodeOptions { get; set; }
    }
}