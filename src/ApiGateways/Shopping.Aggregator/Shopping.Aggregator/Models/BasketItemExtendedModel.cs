﻿namespace Shopping.Aggregator.Models
{
    public class BasketItemExtendedModel
    {

        public int Quantity { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public string ProductId { get; set; } // With the help of this info we will receive extra information from the mongo database
        public string ProductName { get; set; }

        //Product Related Additional Fields
        public string Category { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ImageFile { get; set; }
    }
}
