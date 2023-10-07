using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DereGunGame.Types
{
    public class JailbirdSettings
    {
        public float JailbirdSwingDamage { get; set; } = 30;
        public float JailbirdChargeDamage { get; set; } = 50;
        public float JailbirdFlashDuration { get; set; } = 0.01f;

        [Description("Settings for the melee weapon used for humiliations.")]
        public int HumiliationPenalty { get; set; } = -3;
    }
}
