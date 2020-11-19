using Olive;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSharp.Build.UpdateNuget
{

    public sealed class MicroserviceItem
    {
        public List<NugetReference> References = new List<NugetReference>();

        static IEnumerable<SolutionProject> StandardProjects
           => Enum.GetValues(typeof(SolutionProject)).OfType<SolutionProject>();

        public void RefreshPackages()
        {
            References = new List<NugetReference>();

            foreach (var project in StandardProjects)
            {
                var packages = project.GetNugetPackages(SolutionFolder)
                    .Where(x => !x.name.StartsWith("Microsoft.AspNetCore."));

                foreach (var p in packages)
                    References.Add(new NugetReference(p.name, p.ver, this, project));
            }
        }
        public void UpdatePackages()
        {
            References.Do(x => x.ShouldUpdate = true);
            UpdateSelectedPackages();
        }

        //    int _nugetFetchTasks, _procId;
        //    readonly double runImageOpacity = .2;

        //    #region INotifyPropertyChanged Implementations

        //    public event PropertyChangedEventHandler PropertyChanged;

        //    //[NotifyPropertyChangedInvocator]
        //    //void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //    //{
        //    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    //}

        //    #endregion

        //    #region Overrides of ToString Object

        //    public override string ToString() => $"\'{Service}\' port : {Port} Status : {Status}";

        //    #endregion

        //    //////readonly LogWindow Logwindow;
        //    //////public MainWindow MainWindow { get; set; }

        //    ////public void UpdateProgress(int progress, string msg = null)
        //    ////{
        //    ////    MainWindow?.Dispatcher?.BeginInvoke(DispatcherPriority.Normal, new MainWindow.MyDelegate(() =>
        //    ////    {
        //    ////        MainWindow.statusProgress.Value = progress;
        //    ////        MainWindow.txtStatusMessage.Text = msg;
        //    ////    }));
        //    ////}

        //    ////public void RollProgress(string msg = null)
        //    ////{
        //    ////    MainWindow?.Dispatcher?.BeginInvoke(DispatcherPriority.Normal, new MainWindow.MyDelegate(() =>
        //    ////    {
        //    ////        MainWindow.statusProgress.IsIndeterminate = true;
        //    ////        MainWindow.txtStatusMessage.Text = msg;
        //    ////    }));
        //    ////}

        //    ////public void StopProgress(string msg = null)
        //    ////{
        //    ////    MainWindow?.Dispatcher?.BeginInvoke(DispatcherPriority.Normal, new MainWindow.MyDelegate(() =>
        //    ////    {
        //    ////        MainWindow.statusProgress.Value = 0;
        //    ////        MainWindow.statusProgress.IsIndeterminate = false;
        //    ////        MainWindow.txtStatusMessage.Text = msg;
        //    ////    }));
        //    ////}

        //    public enum EnumStatus
        //    {
        //        NoSourcerLocally = 1,
        //        Stop = 2,
        //        Run = 3,
        //        Pending = 4
        //    }

        //    bool _nugetUpdateIsInProgress, _gitUpdateIsInProgress, FirstStatus = true;
        //    EnumStatus _status;
        //    public EnumStatus Status
        //    {
        //        get => _status;
        //        set
        //        {
        //            if (value != _status)
        //                switch (value)
        //                {
        //                    case EnumStatus.Run:
        //                        Logwindow.LogMessage("Service Started.");
        //                        break;
        //                    case EnumStatus.Stop:
        //                        if (FirstStatus) FirstStatus = false;
        //                        else Logwindow.LogMessage("Service Stoped.");
        //                        break;
        //                }

        //            _status = value;

        //            OnPropertyChanged(nameof(Status));
        //            OnPropertyChanged(nameof(RunImage));
        //            OnPropertyChanged(nameof(RunImageOpacity));
        //            OnPropertyChanged(nameof(ServiceColor));
        //            OnPropertyChanged(nameof(ServiceFontWeight));
        //            OnPropertyChanged(nameof(ServiceTooltip));
        //            OnPropertyChanged(nameof(VisibleDebug));
        //            OnPropertyChanged(nameof(VsCodeIcon));
        //        }
        //    }

        //    public string Service
        //    {
        //        get => _service;
        //        set
        //        {
        //            _service = value;
        //            Logwindow.Title = $"{Service} Microservice Log Window";
        //        }
        //    }

        //    public FontWeight ServiceFontWeight => Status == EnumStatus.Run ? FontWeights.Bold : FontWeights.Regular;

        //    public Brush ServiceColor
        //    {
        //        get
        //        {
        //            switch (Status)
        //            {
        //                case EnumStatus.NoSourcerLocally:
        //                    return Brushes.DimGray;
        //                case EnumStatus.Stop:
        //                    return Brushes.DarkRed;
        //                case EnumStatus.Run:
        //                    return Brushes.Green;
        //                default:
        //                    return Brushes.Black;
        //            }
        //        }
        //    }

        //    public string ServiceTooltip
        //    {
        //        get
        //        {
        //            switch (Status)
        //            {
        //                case EnumStatus.NoSourcerLocally:
        //                    return "Source not available locally";
        //                case EnumStatus.Stop:
        //                    return "Service Stopped locally";
        //                case EnumStatus.Run:
        //                    return $"Service is Running locally ( '{ProcessName}' process Id : {ProcId})";
        //                default:
        //                    return "";
        //            }
        //        }
        //    }

        //    public string BuildTooltip
        //    {
        //        get
        //        {
        //            switch (BuildStatus)
        //            {
        //                case "off":
        //                    return "Building Microservice project has been stopped.";
        //                case "Running":
        //                    return "Building Microservice project...";
        //                case "Failed":
        //                    return "Building Microservice project has been failed.";
        //                default:
        //                    return "Build Microservice Project";
        //            }
        //        }
        //    }

        //    public string Port { get; set; }

        //    public string LiveUrl { get; set; }
        //    public string UatUrl { get; set; }

        //    public object RunImage
        //    {
        //        get
        //        {
        //            switch (Status)
        //            {
        //                case EnumStatus.Stop:
        //                    return "Resources/run2.png";
        //                case EnumStatus.Run:
        //                    return "Resources/pause.png";
        //                case EnumStatus.Pending:
        //                    return "Resources/gears.gif";
        //                default:
        //                    return null;
        //            }
        //        }
        //    }

        //    public object BuildImage
        //    {
        //        get
        //        {
        //            switch (BuildStatus)
        //            {
        //                case "off":
        //                    return "Resources/run2.png";
        //                case "failed":
        //                    return "Resources/pause.png";
        //                case "running":
        //                    return "Resources/gears.gif";
        //                default:
        //                    return null;
        //            }
        //        }
        //    }

        //    public double RunImageOpacity => Status == EnumStatus.Run ? 1 : runImageOpacity;
        //    public int ProcId
        //    {
        //        get => _procId;
        //        set
        //        {
        //            _procId = value;
        //            if (_procId > 0)
        //            {
        //                ProcessName = Process.GetProcessById(_procId).ProcessName;
        //                Status = EnumStatus.Run;
        //            }
        //            else
        //            {
        //                ProcessName = null;
        //                Status = EnumStatus.Stop;
        //            }

        //            OnPropertyChanged(nameof(Status));
        //            OnPropertyChanged(nameof(ProcId));
        //            OnPropertyChanged(nameof(ProcessName));
        //            OnPropertyChanged(nameof(VisibleKestrel));
        //        }
        //    }

        //    public bool VsIsOpen { get; set; }

        //    public string ProcessName { get; private set; }

        //    string _websiteFolder;
        //    public string WebsiteFolder
        //    {
        //        get => _websiteFolder;
        //        set
        //        {
        //            _websiteFolder = value;
        //            OnPropertyChanged(nameof(WebsiteFolder));
        //            OnPropertyChanged(nameof(VisibleCode));
        //        }
        //    }

        public string SolutionFolder { get; set; }
        public IUpdateNugetBuilder Builder { get; set; }

        //    public object PortIcon => Port.TryParseAs<int>().HasValue ? null : "Resources/Warning.png";

        //    public string PortTooltip => PortIcon != null ? $"launchsettings.json File Not Found in this location :\n{WebsiteFolder}\\Properties\\launchSettings.json" : null;

        //    public Visibility VisibleCode => PortTooltip.IsEmpty() ? Visibility.Visible : Visibility.Hidden;

        //    public object VsCodeIcon => VsDTE == null ? "Resources/VS.png" : "Resources/VS2.png";

        //    public object Tag { get; set; }

        //    DTE2 _vsDTE;
        //    string _gitUpdates;

        //    public DTE2 VsDTE
        //    {
        //        get => _vsDTE;
        //        set
        //        {
        //            _vsDTE = value;
        //            OnPropertyChanged(nameof(VsDTE));
        //            OnPropertyChanged(nameof(VsCodeIcon));
        //            OnPropertyChanged(nameof(VisibleDebug));
        //            OnPropertyChanged(nameof(DebuggerIcon));
        //        }
        //    }

        //    public Visibility VisibleDebug => VsDTE == null || ProcId <= 0 ? Visibility.Collapsed : Visibility.Visible;

        //    public object DebuggerIcon
        //    {
        //        get
        //        {
        //            string icon = null;
        //            if (VsDTE != null)
        //                try
        //                {
        //                    icon = VsDTE.Mode == vsIDEMode.vsIDEModeDebug
        //                        ? "Resources/debug_stop.png"
        //                        : "Resources/debug.png";
        //                }
        //                catch (Exception e)
        //                {
        //                    icon = "Resources/debug.png";
        //                }

        //            OnPropertyChanged(nameof(VisibleDebug));
        //            return icon;
        //        }
        //    }

        void UpdateSelectedPackages()
        {
            var toUpdate = References.Where(x => x.ShouldUpdate && !x.IsUpToDate);

            foreach (var item in toUpdate)
                item.Update();
        }

        public void LogMessage(string message, string desc = null)
        {
            Builder.Log(message);
        }
    }
}