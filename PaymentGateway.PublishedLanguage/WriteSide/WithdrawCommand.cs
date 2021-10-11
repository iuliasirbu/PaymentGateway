﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.PublishedLanguage.WriteSide
{
    public class WithdrawCommand
    {
        public int AccountId { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
    }
}
