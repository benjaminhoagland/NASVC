using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Collections;

namespace NASVC
{
    public partial class NASVC : ServiceBase
    {
        public NASVC()
        {
            InitializeComponent(); 
        }
        Timer timer = new Timer();
        public static Queue<Data.Schema.Table.Node> NodesToExecute = new Queue<Data.Schema.Table.Node>();
        public static List<string> ExecutingScriptGUIDs = new List<string>();
        protected override void OnStart(string[] args)
        {
            Log.Write("NodeAlive Scheduluer Service started.");
            Data.Initialize(); 

            timer.Interval = 1000 * 1; // 1 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
           Log.Write("NodeAlive Scheduluer Service stopped.");
        }
        public async void OnTimer(object sender,ElapsedEventArgs args)
        {
            try
			{

            foreach(var node in Data.Select.Node())
            {
                try
                {
                    if(node.Timeout / 3f < (DateTime.Now - node.LastResponse).TotalSeconds &&
                       !NodesToExecute.Contains(node) &&
                       !ExecutingScriptGUIDs.Contains((from s in Data.Select.Script()
                                                       where s.NodeGUID == node.GUID
                                                       select s.GUID).FirstOrDefault()))
                    {
                        NodesToExecute.Enqueue(node);
                    }
                }
                catch
                { }
            }
			}
            catch
			{

			}
            while(NodesToExecute.Count > 0)
            {
                var node = NodesToExecute.Dequeue();
                if(node == null) continue;
                try
				{

                ExecutingScriptGUIDs.Add((from s in Data.Select.Script()
                                          where s.NodeGUID == node.GUID
                                          select s.GUID).FirstOrDefault());
				}
                catch
				{

				}
                try
				{
                    if(node != null) ExecuteNodeScript(node).Start();
				}
                catch
				{
                    Log.WriteError("problem executing ExecuteNodeScript(node).Start();");
				}
            }
        }
        private async Task ExecuteNodeScript(Data.Schema.Table.Node node)
		{
            try
			{
                var script = (from s in Data.Select.Script()
                                      where s.NodeGUID == node.GUID
                                      select s).FirstOrDefault();
                if(script.Contents == null) return;
                PowerShell ps = PowerShell.Create().AddScript(script.Contents);
                var result = new Data.Schema.Table.Result();
                result.DateCreated = DateTime.Now;
                try
			    {
                    var powerShellResults = await Task.Factory.FromAsync(ps.BeginInvoke(), psResult => ps.EndInvoke(psResult));
                    result.Status = ps.Streams.Error.Count > 0 ? false : true;
                    foreach(PSObject r in powerShellResults)
                    { 
                        result.Contents += r.ToString();
                    }
			    } 
                catch (RuntimeException ex)
                {
                    result.Status = false;
                    result.Contents = "No response from node: " + node.Name;
                    Log.Write(ex.Message);
                }
                result.DateFinishedExecution = DateTime.Now;
                result.GUID = Guid.NewGuid().ToString();
                result.NodeGUID = node.GUID;
                result.ScriptGUID = script.GUID;
                Data.Insert("result", new List<Data.RecordStructure.Attribute>()
			    {
                    new Data.RecordStructure.Attribute("guid", result.GUID),
                    new Data.RecordStructure.Attribute("node_guid", result.NodeGUID),
                    new Data.RecordStructure.Attribute("script_guid", result.ScriptGUID),
                    new Data.RecordStructure.Attribute("date_created", result.DateCreated.ToString(Data.timeformat)),
                    new Data.RecordStructure.Attribute("date_finished_execution", result.DateFinishedExecution.ToString(Data.timeformat)),
                    new Data.RecordStructure.Attribute("status", result.Status.ToString()),
                    new Data.RecordStructure.Attribute("contents", result.Contents),
			    });
                Data.Update.Node(node.GUID, result.DateFinishedExecution, result.Status);
                if(ExecutingScriptGUIDs.Contains(script.GUID)) ExecutingScriptGUIDs.Remove(script.GUID);
			}
            catch
			{

			}
		}
    }
}
