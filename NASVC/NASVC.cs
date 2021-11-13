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

namespace NASVC
{
    public partial class NASVC : ServiceBase
    {
        public NASVC()
        {
            InitializeComponent(); 
        }
        Timer timer = new Timer();
        public static List<Data.Schema.Table.Node> NodeExecutionQueue = new List<Data.Schema.Table.Node>();
        public static List<Data.Schema.Table.Node> NodesToBeRemoved = new List<Data.Schema.Table.Node>();
        public static List<string> ExecutingGUIDs = new List<string>();
        bool dead = false;
        bool alive = true;
        bool locked = false;
        protected override void OnStart(string[] args)
        {
            Log.Write("NodeAlive Scheduluer Service started.");
            
            // initialize database
            Data.Initialize(); 

            timer.Interval = 1000; // 1 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
           Log.Write("NodeAlive Scheduluer Service stopped.");
        }
        public async void OnTimer(object sender, ElapsedEventArgs args)
        {

            if(locked)
            {
                return;
            }
            else
            {
                locked = true; 
            }
            var nodes = Data.Select.Node();
            // Log.Write("got here 1");
            // Log.Write("selected node count is " + nodes.Count.ToString());
            foreach(var node in nodes)
			{
                if(node == null) continue;
                var seconds = (DateTime.Now - node.LastResponse).TotalSeconds;
                // Log.Write("node.Timeout is " + node.Timeout.ToString());
                // Log.Write("seconds are " + seconds);
                // Log.Write("node.Timeout / 3f is " + node.Timeout / 3f);
                // Log.Write("node.Timeout / 3f < seconds is " + (node.Timeout / 3f < seconds));
                if(node.Timeout / 3f < seconds)
				{
					// Log.Write("node is due, add script to execution queue");
					NodeExecutionQueue.Add(node);
				}
                else
				{
                    if(NodeExecutionQueue.Contains(node))
					{
                        NodesToBeRemoved.Add(node);
					}
				}
			}
            foreach(var node in NodesToBeRemoved)
			{
                if(node == null) continue;
                if(NodeExecutionQueue.Contains(node))
			    {
                    NodeExecutionQueue.Remove(node);
			    }
			}
            // Log.Write("selected script count is " + scripts.Count.ToString());
            // Log.Write("NodesToExecute count is: " + NodesToExecute.Count);
            foreach(var node in NodeExecutionQueue)
			{
                if(node == null) continue;
                var script = (from s in Data.Select.Script() 
                                where s.NodeGUID == node.GUID
                                select s).FirstOrDefault();
                if(script == null) continue;
                // Log.Write("selected script contents are:" + Environment.NewLine + script.Contents);
                var result = new Data.Schema.Table.Result();
                result.NodeGUID = node.GUID;
                result.ScriptGUID = script.GUID;
                result.DateCreated = DateTime.Now;
                var status = dead;
                var message = "No response.";
                try
				{
                    Log.Write("attempting to run script " + script.GUID + " for node " + node.GUID);
                    System.Management.Automation.PowerShell ps = PowerShell.Create();
                    //ps.AddScript(script.Contents);
                    ps.AddScript(script.Contents);
                    Collection<PSObject> results = ps.Invoke();
                    if(ps.Streams.Error.Count > 0) 
                    { 
                        status = dead; 
                        // foreach(var s in ps.Streams.Error)
						//{
                        //   Log.Write(s.ErrorDetails.Message);
						//}
                    }
                    else
					{
                        status = alive;
					}
                    message = "";
                    foreach(PSObject r in results)
                    { 
                        message += r.ToString();
                    }
                    // Log.Write("node is done, remove script from execution queue");
				}
                catch (RuntimeException ex)
                {
                    status = dead;
                    Log.Write(ex.Message);
                }
                result.DateFinishedExecution = DateTime.Now;
                result.Status = status;
                result.Contents = message;
                result.GUID = Guid.NewGuid().ToString();
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
                NodesToBeRemoved.Add(node);

                RunTask(script);
                
			}
            
            // Log.Write("got here 2");
            locked = false;
        }
        public async void RunTask(Data.Schema.Table.Script script)
		{
            Log.Write("Executed RunTask at " + DateTime.Now);
            // if(ExecutingGUIDs.Contains(script.GUID)) return;
            // ExecutingGUIDs.Add(script.GUID);
            var result = await ExecutePowerShellTask(script);
            // ExecutingGUIDs.Remove(script.GUID);
            foreach(var pSObject in result)
			{
                // Log.Write(pSObject.ToString());
			}
            Log.Write("Completed RunTAsk at " + DateTime.Now);
		}
        public async Task<PSDataCollection<PSObject>> ExecutePowerShellTask(Data.Schema.Table.Script script)
        {
            Log.Write("ExecutePowerShellTask executed at " + DateTime.Now);
            // var ps = PowerShell.Create();
            // ps.AddScript(script.Contents);
            var result = new PSDataCollection<PSObject>();
            //var result = await Task.Factory.FromAsync(ps.BeginInvoke(), psResult => ps.EndInvoke(psResult));
            return result;
        }

    }
}
