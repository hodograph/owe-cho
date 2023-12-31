﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorApp.Shared
{
    public static class Constants
    {
        public const string DATABASE_NAME = "syncowe";

        public const string TRIPS_TABLE_NAME = "Trips";

        public const string TRANSACTIONS_TABLE_NAME = "Transactions";

        public const string REIMBURSEMENTS_TABLE_NAME = "Reimbursements";

        public const string USERS_TABLE_NAME = "Users";

        public const string SUBSCRIPTIONS_TABLE_NAME = "NotificationSubscriptions";

        public const string CONNECTION_SETTINGS_KEY = "CosmosDBConnection";

        public const string EMAIL_ClAIM_NAME = "preferred_username";

        public const string SEND_PAYMENT_LINK = "https://venmo.com/{0}?txn=pay&audience=public&amount={1}&note={2}";

        public const string REQUEST_PAYMENT_LINK = "https://venmo.com/{0}?txn=charge&audience=public&amount={1}&note={2}";
    }
}
