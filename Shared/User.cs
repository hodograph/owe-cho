using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorApp.Shared
{
    public class User
    {
        public string email { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string VenmoId { get; set; } = string.Empty;

        /// <summary>
        /// Unique ID in DB. Don't change or mess with.
        /// </summary>
        public string id { get; set; }

        public override bool Equals(object obj)
        {
            bool equal = false;
            if (obj is User user)
            {
                equal = user.email == email;
            }

            return equal;
        }

        public override int GetHashCode()
        {
            return email.GetHashCode();
        }
    }
}
