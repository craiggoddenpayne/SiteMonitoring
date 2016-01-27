using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace SiteMonitoringApi
{
    public class PhantomJsBrowser : IDisposable
    {
        private Process _phantomProcess;
        private readonly List<string> _errorLines;
        private Stream _inputStream;
        private Stream _outputStream;
        private TimeSpan _executionTimeout;


        public PhantomJsBrowser(Stream inputStream, Stream outputStream)
        {
            _inputStream = inputStream;
            _outputStream = outputStream;
            _errorLines = new List<string>();
            this.ToolPath = this.ResolveAppBinPath();
            this.PhantomJsExeName = "phantomjs.exe";
            this.ProcessPriority = ProcessPriorityClass.Normal;
        }

        public void Dispose()
        {
            Try(() => _inputStream.Dispose());
            Try(() => _outputStream.Dispose());
            Try(() => _phantomProcess.Kill());
            Try(() => _phantomProcess.Dispose());
        }
        
        public event EventHandler<DataReceivedEventArgs> OutputReceived;
        public event EventHandler<DataReceivedEventArgs> ErrorReceived;

        public string ToolPath { get; set; }
        public string TempFilesPath { get; set; }
        public string PhantomJsExeName { get; set; }
        public string CustomArgs { get; set; }
        public ProcessPriorityClass ProcessPriority { get; set; }


        private string ResolveAppBinPath()
        {
            string result = AppDomain.CurrentDomain.BaseDirectory;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                Type type = assembly.GetType("System.Web.HttpRuntime", false);
                if (type != null)
                {
                    PropertyInfo property = type.GetProperty("AppDomainId", BindingFlags.Static | BindingFlags.Public);
                    if (!(property == null) && property.GetValue(null, null) != null)
                    {
                        PropertyInfo property2 = type.GetProperty("BinDirectory", BindingFlags.Static | BindingFlags.Public);
                        if (!(property2 != null))
                        {
                            break;
                        }
                        object value = property2.GetValue(null, null);
                        if (value is string)
                        {
                            result = (string)value;
                            break;
                        }
                        break;
                    }
                }
            }
            return result;
        }

        public void Run(string jsFile, TimeSpan timeout, params string[] jsArgs)
        {
            _executionTimeout = timeout;

            if (jsFile == null)
            {
                throw new ArgumentNullException("jsFile");
            }

            RunInternal(jsFile, jsArgs);
            try
            {
                WaitProcessForExit();
            }
            finally
            {
                _phantomProcess.Close();
                _phantomProcess = null;
            }
        }

        public void RunScript(string javascriptCode, TimeSpan timeout, params string[] jsArgs)
        {
            _executionTimeout = timeout;
            string tempPath = this.GetTempPath();
            string text = Path.Combine(tempPath, "phantomjs-" + Path.GetRandomFileName() + ".js");
            try
            {
                File.WriteAllBytes(text, Encoding.UTF8.GetBytes(javascriptCode));
                Run(text, timeout, jsArgs);
            }
            finally
            {
                this.DeleteFileIfExists(text);
            }
        }

        private string GetTempPath()
        {
            if (!string.IsNullOrEmpty(this.TempFilesPath) && !Directory.Exists(this.TempFilesPath))
            {
                Directory.CreateDirectory(this.TempFilesPath);
            }
            return this.TempFilesPath ?? Path.GetTempPath();
        }

        private void DeleteFileIfExists(string filePath)
        {
            if (filePath != null && File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                }
            }
        }

        private string PrepareCmdArg(string arg)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('"');
            stringBuilder.Append(arg.Replace("\"", "\\\""));
            stringBuilder.Append('"');
            return stringBuilder.ToString();
        }

        private void RunInternal(string jsFile, params string[] jsArgs)
        {
            this._errorLines.Clear();
            try
            {
                string text = Path.Combine(this.ToolPath, this.PhantomJsExeName);
                if (!File.Exists(text))
                {
                    throw new FileNotFoundException("Cannot find PhantomJS: " + text);
                }
                StringBuilder stringBuilder = new StringBuilder();
                if (!string.IsNullOrEmpty(this.CustomArgs))
                {
                    stringBuilder.AppendFormat(" {0} ", this.CustomArgs);
                }
                stringBuilder.AppendFormat(" {0}", this.PrepareCmdArg(jsFile));
                if (jsArgs != null)
                {
                    for (int i = 0; i < jsArgs.Length; i++)
                    {
                        string arg = jsArgs[i];
                        stringBuilder.AppendFormat(" {0}", this.PrepareCmdArg(arg));
                    }
                }
                ProcessStartInfo processStartInfo = new ProcessStartInfo(text, stringBuilder.ToString());
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.WorkingDirectory = Path.GetDirectoryName(this.ToolPath);
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                _phantomProcess = new Process();
                _phantomProcess.StartInfo = processStartInfo;
                _phantomProcess.EnableRaisingEvents = true;
                _phantomProcess.Start();
                _phantomProcess.ErrorDataReceived += delegate (object o, DataReceivedEventArgs args)
                {
                    if (args.Data == null)
                    {
                        return;
                    }
                    this._errorLines.Add(args.Data);
                    if (this.ErrorReceived != null)
                    {
                        this.ErrorReceived(this, args);
                    }
                };
                _phantomProcess.BeginErrorReadLine();
                CopyToStdIn(_inputStream);
                ReadStdOutToStream(this._phantomProcess, _outputStream);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot execute PhantomJS: " + ex.Message, ex);
            }
        }

        protected void CopyToStdIn(Stream inputStream)
        {
            byte[] array = new byte[8192];
            while (true)
            {
                int num = inputStream.Read(array, 0, array.Length);
                if (num <= 0)
                {
                    break;
                }
                this._phantomProcess.StandardInput.BaseStream.Write(array, 0, num);
                this._phantomProcess.StandardInput.BaseStream.Flush();
            }
            this._phantomProcess.StandardInput.Close();
        }

        public void WriteLine(string s)
        {
            if (this._phantomProcess == null)
            {
                throw new InvalidOperationException("PhantomJS is not running");
            }
            this._phantomProcess.StandardInput.WriteLine(s);
            this._phantomProcess.StandardInput.Flush();
        }

        public void WriteEnd()
        {
            _phantomProcess.StandardInput.Close();
        }

        private void WaitProcessForExit()
        {
            _phantomProcess.WaitForExit((int)this._executionTimeout.TotalMilliseconds);
        }

        private void ReadStdOutToStream(Process proc, Stream outputStream)
        {
            byte[] array = new byte[32768];
            int count;
            while ((count = proc.StandardOutput.BaseStream.Read(array, 0, array.Length)) > 0)
            {
                outputStream.Write(array, 0, count);
            }
        }

        public void Try(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}