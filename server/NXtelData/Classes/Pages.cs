﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace NXtelData
{
    public class Pages : List<Page>
    {
        public int ZoneFilter { get; set; }
        public bool PrimaryFilter { get; set; }

        public Pages()
        {
            ZoneFilter = -1;
            PrimaryFilter = false;
        }

        public static Pages Load()
        {
            var list = new Pages();
            using (var con = new MySqlConnection(DBOps.ConnectionString))
            {
                con.Open();
                string sql = "SELECT * FROM page ORDER BY PageNo,FrameNo;";
                var cmd = new MySqlCommand(sql, con);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var item = new Page();
                        item.Read(rdr);
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public static Pages LoadStubs(int ZoneID = -1)
        {
            var list = new Pages();
            var pids = new List<int>();
            using (var con = new MySqlConnection(DBOps.ConnectionString))
            {
                con.Open();
                string filter = "";
                if (ZoneID > 0)
                    filter = "WHERE PageID IN (SELECT PageID FROM pagezone pz WHERE pz.ZoneID=" + ZoneID + ") ";
                else if (ZoneID == -2)
                    filter = "WHERE PageID NOT IN (SELECT PageID FROM pagezone pz) ";
                string sql = @"SELECT PageID,PageNo,FrameNo,Title,ToPageFrameNo 
                    FROM page " + filter + @"
                    ORDER BY PageNo,FrameNo;";
                var cmd = new MySqlCommand(sql, con);
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var item = new Page();
                        item.Read(rdr, true);
                        list.Add(item);
                        pids.Add(item.PageID);
                    }
                }
                if (pids.Count > 0)
                {
                    sql = @"SELECT PageID,ZoneID
                    FROM pagezone
                    WHERE PageID IN(" +  string.Join(",", pids) + @")
                    ORDER BY PageID,ZoneID;";
                    cmd.CommandText = sql;
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            int pid = rdr.GetInt32("PageID");
                            int zid = rdr.GetInt32("ZoneID");
                            var page = list.FirstOrDefault(p => p.PageID == pid);
                            if (page != null)
                                page.ZoneIDs += (string.IsNullOrWhiteSpace(page.ZoneIDs) ? "" : ",") + zid;
                        }
                    }
                }
            }
            return list;
        }
    }
}
