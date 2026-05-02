# Unturned无限子弹插件（换弹式）

一个用于 Unturned Rocket 服务器的换弹式无限子弹插件。插件不会跳过玩家原本的换弹动作，而是在玩家换弹时自动补满弹匣，从而实现更自然的无限子弹效果。

## 功能

- 玩家可通过命令自行开启或关闭无限子弹。
- 支持权限控制，默认权限节点为 `ZX`。
- 支持玩家进服后自动开启。
- 支持武器黑名单，黑名单内武器不会触发无限子弹。
- 支持自定义聊天消息颜色和消息图标。
- 插件加载后会在插件目录自动生成使用说明文件。

## 构建

本项目目标框架为 `.NET Framework 4.8`，依赖 `RestoreMonarchy.RocketRedist`。

```powershell
dotnet restore
dotnet build "换弹式无限子弹.csproj" -c Release
```

构建成功后，插件 DLL 位于：

```text
bin/Release/net48/换弹式无限子弹.dll
```

## 安装与加载

1. 构建项目，得到 `换弹式无限子弹.dll`。
2. 将 DLL 复制到 Unturned Rocket 服务器目录下的 `Rocket/Plugins` 文件夹。
3. 启动或重启服务器。
4. 服务器控制台出现插件加载日志后，即表示插件已加载。

插件首次加载后会生成配置文件：

```text
Rocket/Plugins/换弹式无限子弹/换弹式无限子弹.configuration.xml
```

## 使用命令

玩家需要拥有配置中设置的权限节点，默认是 `ZX`。

```text
/ZX ON
```

开启无限子弹。

```text
/ZX OFF
```

关闭无限子弹。

## 配置说明

```xml
<Enabled>true</Enabled>
```

是否启用插件整体功能。

```xml
<Permission>ZX</Permission>
```

使用插件功能所需的权限节点。

```xml
<WeaponBlacklist>
  <ushort>58961</ushort>
</WeaponBlacklist>
```

武器黑名单。填入武器 ID 后，该武器不会获得无限子弹效果。

```xml
<Debug>false</Debug>
```

是否输出调试日志。

```xml
<AutoEnableOnJoin>false</AutoEnableOnJoin>
```

玩家进入服务器时是否默认开启无限子弹。即使默认开启，玩家仍可使用 `/ZX OFF` 关闭。

```xml
<MessageIconUrl>https://z3.ax1x.com/2021/02/27/6pIG4g.png</MessageIconUrl>
```

聊天消息图标链接。

```xml
<MessageColor>rocket</MessageColor>
```

聊天消息颜色，可使用 Rocket 支持的颜色名称或 `#RRGGBB` 格式。

## 开发信息

- 插件入口：`ZXInfiniteReloadAmmoPlugin`
- 命令类：`ZXCommand`
- 配置类：`ZXIRAConfiguration`
- 目标框架：`net48`

## 许可证

本项目使用 MIT License 开源。
