using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorApp.Shared
{
    public class Trip
    {
        public Guid id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public List<string> SharedWith { get; set; } = new List<string>();

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public bool Archived { get; set; }
    }
}
