using Rocket.API;
using System.Collections.Generic;

namespace ZXInfiniteReloadAmmo
{
    public class ZXIRAConfiguration : IRocketPluginConfiguration
    {
        public bool Enabled { get; set; }
        public string Permission { get; set; }
        public List<ushort> WeaponBlacklist { get; set; }
        public bool Debug { get; set; }

        public bool AutoEnableOnJoin { get; set; }

        public string MessageIconUrl { get; set; }

        public string MessageColor { get; set; }

        public void LoadDefaults()
        {
            Enabled = true;
            Permission = "ZX";
            WeaponBlacklist = new List<ushort> { 58961 };
            Debug = false;

            AutoEnableOnJoin = false;
            MessageIconUrl = "https://z3.ax1x.com/2021/02/27/6pIG4g.png";
            MessageColor = "rocket";
        }
    }
}
