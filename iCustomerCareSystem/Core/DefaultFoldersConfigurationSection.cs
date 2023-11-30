using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCustomerCareSystem.Core
{
    public class DefaultFoldersConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public KeyValueConfigurationCollection DefaultFolders
        {
            get => (KeyValueConfigurationCollection)base[""];
        }
    }
}
