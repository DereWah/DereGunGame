using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem.Items;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DereGunGame.Types
{
    public class GunLevel
    {

        [Description("Items in the player's inventory. Include also the type of AMMO.")]
        public List<ItemType> Loadout { get; set; }

        [Description("List of effects a player will have on spawning.")]
        public List<Effect> Effects { get; set; }

        [Description("The class the player will look like at this level. I recommend using only human-like classes.")]
        public RoleTypeId Appearance { get; set; }

        public byte ReloadSpeedMultiplier { get; set; } = 2;

        [Description("The max amounth of health the player will have.")]
        public int MaxHealth { get; set; }

        public void spawnPlayer(Player player)
        {
            player.Role.Set(Appearance);
        }

        public void giveLoadout(Player player, DereGunGame plugin)
        {
            plugin.Utilities.ClearInventory(player);

            player.SetFriendlyFire(Appearance, 1);
            player.AddItem(Loadout);
            
            Jailbird jb = player.AddItem(ItemType.Jailbird) as Jailbird;
            jb.FlashDuration = plugin.Config.JailbirdSettings.JailbirdFlashDuration;
            jb.ChargeDamage = plugin.Config.JailbirdSettings.JailbirdChargeDamage;
            jb.MeleeDamage = plugin.Config.JailbirdSettings.JailbirdSwingDamage;
            player.EnableEffects(Effects);
            player.MaxHealth = MaxHealth;
            
            
            player.Health = player.MaxHealth;
        }

    }
}
