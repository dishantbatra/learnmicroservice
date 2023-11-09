namespace Shopping.Aggregator.Models
{
    public class CatalogModel
    {
        // It is in sync with the Product model in the Catalog API project
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ImageFile { get; set; }
        public decimal Price { get; set; }
    }
}
