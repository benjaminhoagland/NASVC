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

namespace NASVC
{
    public partial class NASVC : ServiceBase
    {
        public NASVC()
        {
            InitializeComponent(); 
        }

        protected override void OnStart(string[] args)
        {
            Log.Write("NodeAlive Scheduluer Service started.");
            
            // initialize database
            Data.Initialize(); 

            Timer timer = new Timer();
            timer.Interval = 1000; // 1 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
           Log.Write("NodeAlive Scheduluer Service stopped.");
        }
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // check if the database is alive
            // if not, try to connect
            // if not connected after x attempts, report error to event log and stop service
            // else continue
            // poll nodes
            // update service definitions for nodes
            // check if nodes are due
            // if nodes are due to query, add to queue

        }
    }
}
