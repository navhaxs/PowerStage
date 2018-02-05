using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace PowerStageAddin
{
    class WiimoteMapping : ApplicationSettingsBase
    {
        private string displayName;  // TODO button mapping here
        [UserScopedSetting()]
        [SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Xml)]
        public string DisplayName
        {
            get { return displayName; }
        }
    }
}
