using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;

namespace HardwareMonitorService
{
    public partial class HardwareMonitorService : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);
        UdpClient udpClient;
        byte[] HWName;
        byte[] HWTemp;
        byte[] HWLoad;
        byte[] messageToSend;
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        public HardwareMonitorService()
        {
            InitializeComponent();
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists("HWMonitor"))
            {
                EventLog.CreateEventSource("HWMonitor", "HWMonitorLog");
            }
            eventLog1.Source = "HWMonitor";
            eventLog1.Log = "HWMonitorLog";
        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            eventLog1.WriteEntry("HWMonitor Service started.");
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            StartUDPClient();
            StartOHM();
            StartMulticast();
        }

        protected override void OnStop()
        {
            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            eventLog1.WriteEntry("HWMonitor Service stopped.");
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("HWMonitor Service continued.");
        }

        void StartUDPClient()
        {
            udpClient = new UdpClient(new IPEndPoint(IPAddress.Broadcast, 22588));
        }

        void StartOHM()
        {
            Computer myPC = new Computer();
            myPC.CPUEnabled = true;
            myPC.GPUEnabled = true;
            myPC.RAMEnabled = true;
            myPC.Open();
            foreach (var hwItem in myPC.Hardware)
            {
                if (hwItem.HardwareType == HardwareType.CPU)
                {
                    hwItem.Update();
                    foreach (IHardware subHW in hwItem.SubHardware) subHW.Update();
                    foreach (var sensor in hwItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {
                            Console.WriteLine("{0} Temperature = {1}", sensor.Name, sensor.Value.HasValue ? sensor.Value.Value.ToString() : "no value");
                        }
                        else if (sensor.SensorType == SensorType.Clock)
                        {

                        }
                        else if (sensor.SensorType == SensorType.Load)
                        {

                        }
                    }
                }
            }
        }

        void StartMulticast()
        {
            
        }
    }
}
