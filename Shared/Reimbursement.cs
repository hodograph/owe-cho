﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorApp.Shared
{
    public class Reimbursement
    {
        public string Payer { get; set; }

        public string Recipient { get; set; }

        public double Amount { get; set; }

        public bool Confirmed { get; set; }
    }
}
