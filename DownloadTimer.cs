using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FileCompare_Reforged
{
    public static class DownloadTimer
    {
        public static DispatcherTimer Timer = new DispatcherTimer {Interval = new TimeSpan(0, 1, 0, 0)};
    }
}
