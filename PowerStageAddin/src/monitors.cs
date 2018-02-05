using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerStageAddin.src
{
    class monitors
    {
        // Computer\HKEY_CURRENT_USER\Software\Microsoft\Office\16.0\PowerPoint\Options
        // DisplayMonitor
        // REG_SZ

        // (only if UseMonMgr == 0)

        // algorithm:
        // for each display [filter: not primary (has taskbar)]
        // remove any entry which matches DisplayMonitor

    }
}
