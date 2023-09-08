using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorApp.Shared
{
    public static class Constants
    {
        public const string DATABASE_NAME = "syncowe";

        public const string TRIPS_TABLE_NAME = "Trips";

        public const string USERS_TABLE_NAME = "Users";

        public const string CONNECTION_SETTINGS_KEY = "CosmosDBConnection";

        public const string EMAIL_ClAIM_NAME = "preferred_username";

        public const string SEND_PAYMENT_LINK = "https://venmo.com/?txn=pay&audience=public&recipients={0}&amount={1}&note={2}";

        public const string REQUEST_PAYMENT_LINK = "https://venmo.com/?txn=charge&audience=public&recipients={0}&amount={1}&note={2}";
    }
}
