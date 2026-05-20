using System;
using System.IO;
using System.Text;

namespace ZXInfiniteReloadAmmo
{
    internal static class ZXUsageDocumentWriter
    {
        private const string ConfigFileName = "换弹式无限子弹.configuration.xml";
        private const string UsageFileName = "无限子弹(换弹匣式)插件使用说明.TXT";
        private const string DefaultIconUrl = "https://z3.ax1x.com/2021/02/27/6pIG4g.png";

        private const string DeveloperName = "欧小茶";
        private const string DeveloperContact = "56896686@QQ.COM";

        public static void EnsureUsageFile(Rocket.Core.Plugins.RocketPlugin plugin, ZXIRAConfiguration config)
        {
            try
            {
                if (plugin == null || string.IsNullOrWhiteSpace(plugin.Directory))
                    return;

                string usageFilePath = Path.Combine(plugin.Directory, UsageFileName);
                string content = BuildUsageText(config);

                File.WriteAllText(usageFilePath, content, new UTF8Encoding(true));
            }
            catch (Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex);
            }
        }

        private static string BuildUsageText(ZXIRAConfiguration config)
        {
            config = config ?? new ZXIRAConfiguration();

            var sb = new StringBuilder();
            sb.AppendLine("==============================");
            sb.AppendLine("无限子弹(换弹匣式)插件使用说明");
            sb.AppendLine("==============================");
            sb.AppendLine();
            sb.AppendLine("一、插件简要说明");
            sb.AppendLine("1. 本插件用于实现换弹匣式武器的无限子弹效果。核心功能不改变玩家原有换弹动作，仅在满足条件时自动补满弹匣。");
            sb.AppendLine("2. 插件支持玩家自行使用命令开启或关闭无限子弹功能。");
            sb.AppendLine("3. 插件支持服务器配置玩家进入服务器时是否默认开启无限子弹。即使默认开启，玩家仍然可以手动关闭。");
            sb.AppendLine("4. 插件支持武器黑名单，加入黑名单的武器不会获得无限子弹效果。");
            sb.AppendLine();
            sb.AppendLine("二、插件使用方法");
            sb.AppendLine("1. 开启功能：/ZX ON");
            sb.AppendLine("2. 关闭功能：/ZX OFF");
            sb.AppendLine("3. 只有拥有对应权限的玩家才可以使用该功能。");
            sb.AppendLine();
            sb.AppendLine("三、配置文件说明");
            sb.AppendLine($"配置文件名：{ConfigFileName}");
            sb.AppendLine("常用配置项如下：");
            sb.AppendLine("1. <Enabled>true</Enabled>");
            sb.AppendLine("   是否启用整个插件。true = 启用，false = 禁用。");
            sb.AppendLine();
            sb.AppendLine("2. <Permission>ZX</Permission>");
            sb.AppendLine("   玩家使用本插件功能所需的权限节点。");
            sb.AppendLine();
            sb.AppendLine("3. <WeaponBlacklist>...</WeaponBlacklist>");
            sb.AppendLine("   武器黑名单。将武器 ID 填入其中后，该武器不会触发无限子弹效果。");
            sb.AppendLine();
            sb.AppendLine("4. <Debug>false</Debug>");
            sb.AppendLine("   是否输出调试日志。true = 输出更多调试信息，false = 关闭调试输出。");
            sb.AppendLine();
            sb.AppendLine("5. <AutoEnableOnJoin>false</AutoEnableOnJoin>");
            sb.AppendLine("   玩家进入服务器时是否默认开启无限子弹。");
            sb.AppendLine("   true = 玩家进服自动开启，但玩家仍可用命令关闭。");
            sb.AppendLine("   false = 玩家需手动使用命令开启。");
            sb.AppendLine();
            sb.AppendLine($"6. <MessageIconUrl>{DefaultIconUrl}</MessageIconUrl>");
            sb.AppendLine("   服务器消息图标链接，默认已内置一张图片地址，可自行修改为其他网络图片链接。");
            sb.AppendLine();
            sb.AppendLine("7. <MessageColor>rocket</MessageColor>");
            sb.AppendLine("   服务器消息颜色，可填写 white、red、rocket 或 #RRGGBB 等值。");
            sb.AppendLine();
            sb.AppendLine("四、当前默认配置提示");
            sb.AppendLine($"1. AutoEnableOnJoin 默认值：{config.AutoEnableOnJoin.ToString().ToLowerInvariant()}");
            sb.AppendLine($"2. MessageIconUrl 默认值：{(string.IsNullOrWhiteSpace(config.MessageIconUrl) ? DefaultIconUrl : config.MessageIconUrl)}");
            sb.AppendLine($"3. MessageColor 当前值：{(string.IsNullOrWhiteSpace(config.MessageColor) ? "rocket" : config.MessageColor)}");
            sb.AppendLine();
            sb.AppendLine("五、开发者信息");
            sb.AppendLine($"开发者：{DeveloperName}");
            sb.AppendLine($"联系方式：{DeveloperContact}");
            sb.AppendLine();
            sb.AppendLine("六、补充说明");
            sb.AppendLine("1. 本说明文件由插件加载时自动生成，生成位置与配置文件位于同一目录。");
            sb.AppendLine("2. 如果你已经存在旧配置文件，新增配置项可能不会自动补进旧文件，请根据需要手动补充。");
            sb.AppendLine();
            sb.AppendLine("祝你使用愉快！");

            return sb.ToString();
        }
    }
}
