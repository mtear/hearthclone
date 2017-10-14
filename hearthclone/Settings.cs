using System;
using System.Collections.Generic;
using System.IO;

namespace hearthclone
{
    class Settings
    {
        private Dictionary<string, string> settings = new Dictionary<string, string>();

        public string ServerUrl
        {
            get { return settings["server_url"]; }
        }

        public int GameServerPort
        {
            get { return Convert.ToInt32(settings["game_server_port"]); }
        }

        public int LoginServerPort
        {
            get { return Convert.ToInt32(settings["login_server_port"]); }
        }

        public string InternalApiAccessCode
        {
            get { return settings["internal_api_access_code"]; }
        }

        public string InternalLoginUrl
        {
            get { return settings["internal_login_url"]; }
        }

        public string InternalTokenLookupUrl
        {
            get { return settings["internal_token_lookup_url"]; }
        }

        public Settings()
        {
            string[] lines = File.ReadAllLines("settings.ini");
            foreach(string l in lines)
            {
                string line = l.Trim();
                if(line != "" && !line.StartsWith("#"))
                {
                    string[] c = line.Split(' ');
                    settings.Add(c[0], c[1]);
                }
            }
        }

        private static Settings instance = null;
        public static Settings Current
        {
            get
            {
                if (instance == null) instance = new Settings();
                return instance;
            }
        }

    }
}
