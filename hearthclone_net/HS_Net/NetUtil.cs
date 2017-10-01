using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HS_Net
{
    public class NetUtil
    {

        public static string PostSynchro(string url, Dictionary<string, string> args)
        {
            StringBuilder param = new StringBuilder();
            int i = 0;
            foreach(string key in args.Keys)
            {
                param.Append(key);
                param.Append('=');
                param.Append(args[key]);
                if (i < args.Keys.Count - 1) param.Append('&');
                i++;
            }

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string result = wc.UploadString(url, param.ToString());
                return result;
            }
        }
    }
}
