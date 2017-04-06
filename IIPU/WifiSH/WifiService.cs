using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WifiSH
{
    public class WifiService
    {
        private string output;

        public WifiService()
        {

        }

        public List<WifiModel> GetWifiList()
        {
            return this.FillWifies();
        }

        private List<WifiModel> FillWifies()
        {
            List<WifiModel> models = new List<WifiModel>();
            var str = this.ExecuteShellCommand();
            var x = str.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Skip(3).ToList();
            for (var i = 0; i < x.Count/10; i++)
            {
                models.Add(new WifiModel()
                {
                    Name = x[i * 10 + 0].Split(':').Skip(1).ToArray()[0].Trim(),
                    ConnectPercent = x[i * 10 + 5].Split(':').Skip(1).ToArray()[0].Trim(),
                    CypherType = x[i * 10 + 2].Split(':').Skip(1).ToArray()[0].Trim(),
                    MacAddress = string.Join("", x[i * 10 + 4].Trim().Skip(8).ToArray()).Trim()
                });
            }
            return models;
        }

        private string ExecuteShellCommand()
        {
            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "cmd";
            proc.StartInfo.Arguments = @"/C ""netsh wlan show networks mode=bssid""";

            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return output;
        }
    }
}
