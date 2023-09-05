using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DereGunGame.Types
{
    public class GunLevel
    {
        public List<ItemType> Loadout { get; set; }
        public List<Effect> Effects { get; set; }
        public RoleTypeId Appearance { get; set; }
        public int MaxHealth { get; set; }

        public void spawnPlayer(Player player)
        {
            player.Role.Set(Appearance);
            player.SetFriendlyFire(Appearance, 1);
        }

        public void giveLoadout(Player player)
        {
            player.AddItem(Loadout);
            player.EnableEffects(Effects);
            player.MaxHealth = MaxHealth;
        }

    }
}
