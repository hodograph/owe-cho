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

        public TripState TripState { get; set; }
        public bool Indefinite { get; set; }
    }
}
