using Rocket.API;
using Rocket.Unturned.Player;

namespace ZXInfiniteReloadAmmo
{
    public class ZXCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "ZX";
        public string Help => "无限子弹开关";
        public string Syntax => "/ZX <ON|OFF>";
        public System.Collections.Generic.List<string> Aliases => new System.Collections.Generic.List<string>();
        public System.Collections.Generic.List<string> Permissions => new System.Collections.Generic.List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = (UnturnedPlayer)caller;
            var plugin = ZXInfiniteReloadAmmoPlugin.Instance;

            if (plugin == null || plugin.Configuration?.Instance == null)
                return;

            if (!plugin.Configuration.Instance.Enabled)
            {
                plugin.SendKey(player, "ZX_ServerDisabled");
                return;
            }

            if (!plugin.HasZXPermission(player))
            {
                plugin.SendKey(player, "ZX_NoPermission");
                return;
            }

            if (command.Length < 1)
            {
                plugin.SendKey(player, "ZX_Usage");
                return;
            }

            string arg = command[0].ToUpperInvariant();

            if (arg == "ON")
            {
                if (plugin.IsInfiniteEnabled(player))
                {
                    plugin.SendKey(player, "ZX_AlreadyOn");
                    return;
                }

                plugin.SetInfiniteEnabled(player, true);
                plugin.SendKey(player, "ZX_On");
                return;
            }

            if (arg == "OFF")
            {
                if (!plugin.IsInfiniteEnabled(player))
                {
                    plugin.SendKey(player, "ZX_AlreadyOff");
                    return;
                }

                plugin.SetInfiniteEnabled(player, false);
                plugin.SendKey(player, "ZX_Off");
                return;
            }

            plugin.SendKey(player, "ZX_Usage");
        }
    }
}
