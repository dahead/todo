using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace todo
{
   [Description("Ping multiple hosts at the same time.")]
    public sealed class PingHostsCommand : Command<PingHostsCommand.Settings>
    {

        public class Settings : CommandSettings
        {            
            [CommandArgument(0, "[FILENAME]")]
            public string Filename { get; set; }

            // [CommandOption("-t|--timeout <TIMEOUT>")]
            // public int Timeout { get; set; }
        }

        public List<string> Hosts { get; set; }

        public override int Execute(CommandContext context, Settings settings)
        {
            this.Hosts = GetHosts(settings.Filename);

            AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots4)
            .Start("Pinging hosts...", ctx =>
            {
                var task = executeParallelAsync();
                task.Wait();
            });
            
            return 0;
        }

        private async Task executeParallelAsync()
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;
            var results = await PingHostsParallelAsyncV2(this.Hosts, progress);
            PrintResults(results);
        }
        
        public async Task<List<PingHostDataModel>> PingHostsParallelAsyncV2(List<string> hosts, IProgress<ProgressReportModel> progress)
        {
            List<PingHostDataModel> output = new List<PingHostDataModel>();
            ProgressReportModel report = new ProgressReportModel();

            await Task.Run(() =>
            {
                // Ping each host x times
                //  Parallel.For(1, 5,
                //    index => { 

                Parallel.ForEach<string>(hosts, (host) =>
                {
                    // Ping host, get result, add result to output
                    PingHostDataModel result = PingHost(host);
                    output.Add(result);

                    // report progress
                    report.PingHostResults = output;
                    report.PercentageComplete = (output.Count * 100) / this.Hosts.Count;
                    progress.Report(report);
                });

                //    } );

            });

            return output;
        }

        // private List<ProgressTask> CreateTasks(List<string> hosts, ProgressContext ctx)
        // {
        //     List<ProgressTask> output = new List<ProgressTask>();
        //     foreach (string host in hosts)
        //         output.Add(ctx.AddTask(host));
        //     return output;
        // }

        private static List<string> GetHosts(string filename)
        {
            List<string> output = new List<string>();
            if (System.IO.File.Exists(filename))
            {
                string[] lines = System.IO.File.ReadAllLines(filename);
                foreach (var item in lines)
                    output.Add(item);
                AnsiConsole.MarkupLine($"Loaded [blue]{ output.Count }[/] hosts from file [blue]{ filename }[/]");
            }
            else
                output = GetDefaultHosts();
                // output = GetIPRange(192, 168, 100, 1, 192, 168, 100, 255);

            return output;
        }

        private static  List<string> GetDefaultHosts()
        {
            AnsiConsole.MarkupLine($"[blue]Using example hosts![/]");
            List<string> output = new List<string>();
            output.Add("yahoo.com");
            output.Add("google.com");
            output.Add("microsoft.com");
            output.Add("cnn.com");
            output.Add("amazon.com");
            output.Add("facebook.com");
            output.Add("twitter.com");
            output.Add("codeproject.com");
            output.Add("stackoverflow.com");
            output.Add("wikipedia.org");
            return output;
        }

        private static  List<string> GetIPRange(byte startA, byte startB, byte startC, byte startD, byte endA, byte endB, byte endC, byte endD)
        {
            AnsiConsole.MarkupLine($"[blue]Using IP Range![/]");
            List<string> output = new List<string>();
    
            int start = BitConverter.ToInt32(new byte[] { startD, startC, startB, startA }, 0);
            int end = BitConverter.ToInt32(new byte[] { endD, endC, endB, endA }, 0);
            // List<IPAddress> addresses = new List<IPAddress>();
            for (int i = start; i <= end; i++)
            {
                byte[] bytes = BitConverter.GetBytes(i);
                IPAddress ipa = new IPAddress(new[] { bytes[3], bytes[2], bytes[1], bytes[0]});
                output.Add(ipa.ToString());
                // addresses.Add(ipa);
            }

            return output;
        }


        private void ReportProgress(object sender, ProgressReportModel e)
        {
            // AnsiConsole.WriteLine(e.PercentageComplete);
            // xxx.Value = e.PercentageComplete;
            // PrintResults(e.PingHostResults);
        }

        private void PrintResults(List<PingHostDataModel> results)
        {          
            // AnsiConsole.MarkupLine("[bold]Done. Final results:[/]");
            // AnsiConsole.Clear();
            try
            {
                foreach (var item in results)
                {                           
                    if (item.HostData != null && item.HostData.RoundtripTime > 0)
                        AnsiConsole.MarkupLine($"[green]{ item.HostName }[/] ({ item.HostData.Address.ToString() }) pinged in { item.HostData.RoundtripTime } ms.");
                    else
                        AnsiConsole.MarkupLine($"[red]{ item.HostName }[/] no response.");
                }
                
            }
            catch (System.Exception)
            {
            }
        }

        public static PingHostDataModel PingHost(string hostname) 
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions(64, true);  
            byte[] buffer = Encoding.ASCII.GetBytes(new String('A', 32));
            int timeout = 10 * 1000; // 10 seconds
            
            // AnsiConsole.MarkupLine($"Pinging host [green]{ hostname }[/] ...");
            PingHostDataModel result = new PingHostDataModel() {HostName = hostname };

            try
            {
                PingReply reply = pingSender.Send(hostname, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    result.HostData = reply;
                    return result;                   
                }                  
            }
            catch (PingException)
            {                
            }
            catch (System.Exception)
            {
            }
           
            return result;           
        }

    }

    public class ProgressReportModel
    {
        public int PercentageComplete { get; set; } = 0;
        public List<PingHostDataModel> PingHostResults { get; set; } = new List<PingHostDataModel>();
    }

    
    public class PingHostDataModel
    {
        public string HostName { get; set; } = "";
        public PingReply HostData { get; set; } = null;
    }
}
