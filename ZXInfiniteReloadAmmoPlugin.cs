using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using ZXLogger = Rocket.Core.Logging.Logger;

namespace ZXInfiniteReloadAmmo
{
    public class ZXInfiniteReloadAmmoPlugin : RocketPlugin<ZXIRAConfiguration>
    {
        public static ZXInfiniteReloadAmmoPlugin Instance { get; private set; }

        private readonly ConcurrentDictionary<CSteamID, byte> _hookedEquipEvent = new ConcurrentDictionary<CSteamID, byte>();

        private readonly ConcurrentDictionary<CSteamID, byte> _enabledPlayers = new ConcurrentDictionary<CSteamID, byte>();

        public override TranslationList DefaultTranslations => new TranslationList
        {
            { "ZX_Usage", "用法：/ZX <ON|OFF>" },
            { "ZX_NoPermission", "你没有权限使用该功能。" },
            { "ZX_ServerDisabled", "无限子弹功能已被服务器关闭。" },
            { "ZX_AlreadyOn", "无限子弹已处于开启状态。" },
            { "ZX_AlreadyOff", "无限子弹已处于关闭状态。" },
            { "ZX_On", "无限子弹已开启。" },
            { "ZX_Off", "无限子弹已关闭。" },

            { "Weapon_Enabled", "该武器（{0}）无限子弹已开启。" },
            { "Weapon_Blacklisted", "该武器（{0}）在服务器黑名单，无法开启无限子弹。" }
        };

        protected override void Load()
        {
            Instance = this;

            if (Configuration.Instance.WeaponBlacklist == null)
                Configuration.Instance.WeaponBlacklist = new List<ushort>();

            UseableGun.onChangeMagazineRequested += OnChangeMagazineRequested;

            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;

            foreach (var sp in Provider.clients)
            {
                var player = UnturnedPlayer.FromSteamPlayer(sp);
                if (player == null)
                    continue;

                try
                {
                    HookEquipEvent(player);

                    if (Configuration.Instance.AutoEnableOnJoin)
                        SetInfiniteEnabled(player, true);
                }
                catch (Exception ex)
                {
                    ZXLogger.LogException(ex);
                }
            }

            ZXUsageDocumentWriter.EnsureUsageFile(this, Configuration.Instance);
            PrintLoadBanner();
        }

        protected override void Unload()
        {
            UseableGun.onChangeMagazineRequested -= OnChangeMagazineRequested;

            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;

            foreach (var sp in Provider.clients)
            {
                var player = UnturnedPlayer.FromSteamPlayer(sp);
                if (player != null)
                    UnhookEquipEvent(player);
            }

            _hookedEquipEvent.Clear();
            _enabledPlayers.Clear();

            Instance = null;

            ZXLogger.Log("[ZXInfiniteReloadAmmo] Unloaded.", ConsoleColor.Yellow);
        }

        private void PrintLoadBanner()
        {
            ZXLogger.Log("========================================", ConsoleColor.Cyan);
            ZXLogger.Log("[ZXInfiniteReloadAmmo] 插件名称：换弹式无限子弹", ConsoleColor.Cyan);
            ZXLogger.Log("[ZXInfiniteReloadAmmo] 插件已成功加载！！", ConsoleColor.Green);
            ZXLogger.Log("========================================", ConsoleColor.Cyan);
            ZXLogger.Log($"[ZXInfiniteReloadAmmo] Enabled={Configuration.Instance.Enabled}, Permission={Configuration.Instance.Permission}", ConsoleColor.Yellow);
            ZXLogger.Log($"[ZXInfiniteReloadAmmo] AutoEnableOnJoin={Configuration.Instance.AutoEnableOnJoin}, BlacklistCount={Configuration.Instance.WeaponBlacklist.Count}", ConsoleColor.Yellow);
            ZXLogger.Log($"[ZXInfiniteReloadAmmo] MessageColor={Configuration.Instance.MessageColor}, MessageIconUrl={Configuration.Instance.MessageIconUrl}", ConsoleColor.Yellow);
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            HookEquipEvent(player);

            if (Configuration.Instance.AutoEnableOnJoin)
                SetInfiniteEnabled(player, true);
        }

        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            UnhookEquipEvent(player);
            if (player != null)
                _enabledPlayers.TryRemove(player.CSteamID, out _);
        }

        private void HookEquipEvent(UnturnedPlayer player)
        {
            try
            {
                if (player?.Player == null)
                    return;

                var id = player.CSteamID;
                if (_hookedEquipEvent.ContainsKey(id))
                    return;

                player.Player.equipment.onEquipRequested += OnEquipRequested;
                _hookedEquipEvent.TryAdd(id, 0);
            }
            catch (Exception ex)
            {
                ZXLogger.LogException(ex);
            }
        }

        private void UnhookEquipEvent(UnturnedPlayer player)
        {
            try
            {
                if (player?.Player == null)
                    return;

                var id = player.CSteamID;
                if (!_hookedEquipEvent.ContainsKey(id))
                    return;

                player.Player.equipment.onEquipRequested -= OnEquipRequested;
                _hookedEquipEvent.TryRemove(id, out _);
            }
            catch (Exception ex)
            {
                ZXLogger.LogException(ex);
            }
        }

        public bool HasZXPermission(UnturnedPlayer player)
        {
            if (player == null)
                return false;

            string perm = Configuration.Instance.Permission;
            if (string.IsNullOrEmpty(perm))
                return false;

            return R.Permissions.HasPermission(player, new List<string> { perm });
        }

        public bool IsBlacklisted(ushort weaponId)
        {
            return Configuration.Instance.WeaponBlacklist != null &&
                   Configuration.Instance.WeaponBlacklist.Contains(weaponId);
        }

        public bool IsInfiniteEnabled(UnturnedPlayer player)
        {
            return player != null && _enabledPlayers.ContainsKey(player.CSteamID);
        }

        public void SetInfiniteEnabled(UnturnedPlayer player, bool enabled)
        {
            if (player == null)
                return;

            if (enabled)
                _enabledPlayers.TryAdd(player.CSteamID, 0);
            else
                _enabledPlayers.TryRemove(player.CSteamID, out _);
        }

        private Color GetMessageColor()
        {
            string name = Configuration.Instance.MessageColor ?? "rocket";
            return Rocket.Unturned.Chat.UnturnedChat.GetColorFromName(name, Palette.SERVER);
        }

        private string GetIconUrlOrNull()
        {
            string url = Configuration.Instance.MessageIconUrl;
            if (string.IsNullOrWhiteSpace(url))
                return null;

            return url.Trim();
        }

        public void SendTo(UnturnedPlayer player, string message, bool rich = true)
        {
            if (player?.Player == null)
                return;

            SteamPlayer toPlayer = player.Player.channel.owner;
            if (toPlayer == null)
                return;

            ChatManager.serverSendMessage(
                message,
                GetMessageColor(),
                fromPlayer: null,
                toPlayer: toPlayer,
                mode: EChatMode.SAY,
                iconURL: GetIconUrlOrNull(),
                useRichTextFormatting: rich
            );
        }

        public void SendKey(UnturnedPlayer player, string key, params object[] args)
        {
            string msg = Translate(key, args);
            SendTo(player, msg, rich: true);
        }

        private void OnEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            try
            {
                if (!Configuration.Instance.Enabled)
                    return;
                if (!shouldAllow)
                    return;
                if (equipment?.player == null)
                    return;

                var player = UnturnedPlayer.FromPlayer(equipment.player);
                if (player == null)
                    return;

                if (!HasZXPermission(player))
                    return;
                if (!IsInfiniteEnabled(player))
                    return;

                if (!(asset is ItemGunAsset gunAsset))
                    return;

                ushort weaponId = gunAsset.id;
                string weaponName = gunAsset.itemName;

                if (IsBlacklisted(weaponId))
                    SendKey(player, "Weapon_Blacklisted", weaponName);
                else
                    SendKey(player, "Weapon_Enabled", weaponName);

                if (Configuration.Instance.Debug)
                    ZXLogger.Log($"[ZXInfiniteReloadAmmo] {player.CharacterName} equipped {weaponId} ({weaponName}) blacklisted={IsBlacklisted(weaponId)}", ConsoleColor.Gray);
            }
            catch (Exception ex)
            {
                ZXLogger.LogException(ex);
            }
        }

        private void OnChangeMagazineRequested(PlayerEquipment equipment, UseableGun gun, Item oldItem, ItemJar newItem, ref bool shouldAllow)
        {
            try
            {
                if (!Configuration.Instance.Enabled)
                    return;
                if (!shouldAllow)
                    return;

                if (equipment?.player == null)
                    return;
                if (gun?.equippedGunAsset == null)
                    return;

                var player = UnturnedPlayer.FromPlayer(equipment.player);
                if (player == null)
                    return;

                if (!HasZXPermission(player))
                    return;
                if (!IsInfiniteEnabled(player))
                    return;

                ushort weaponId = gun.equippedGunAsset.id;
                if (IsBlacklisted(weaponId))
                    return;

                if (oldItem == null || newItem?.item == null)
                    return;

                RefillIfMagazine(oldItem);
                RefillIfMagazine(newItem.item);

                if (Configuration.Instance.Debug)
                    ZXLogger.Log($"[ZXInfiniteReloadAmmo] {player.CharacterName} reloaded weapon {weaponId}. Magazines refilled.", ConsoleColor.Gray);
            }
            catch (Exception ex)
            {
                ZXLogger.LogException(ex);
            }
        }

        private void RefillIfMagazine(Item item)
        {
            if (item == null)
                return;

            ItemAsset asset = Assets.find(EAssetType.ITEM, item.id) as ItemAsset;
            if (asset == null)
                return;

            if (!(asset is ItemMagazineAsset))
                return;

            // asset.amount 对 ItemMagazineAsset 即为弹匣容量
            item.amount = asset.amount;
        }
    }
}
