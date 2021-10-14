﻿namespace PaymentGateway.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string Currency { get; set; }
        public double Limit { get; set; }


    }
}
