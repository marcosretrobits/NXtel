﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace MySql.Data.MySqlClient
{
    public static class MySqlDataReaderExtensions
    {
        public static string GetStringNullable(this MySqlDataReader rdr, string column)
        {
            if (rdr == null)
                return "";
            if (rdr.IsDBNull(rdr.GetOrdinal(column)))
                return "";
            return rdr.GetString(column);
        }
    }
}
