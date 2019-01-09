﻿﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DBZMOD.Buffs
{
    public class ASSJBuff : TransBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Ascended Super Saiyan");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            DamageMulti = 1.75f;
            SpeedMulti = 1.75f;
            KiDrainRate = 1.15f;
            KiDrainRateWithMastery = 0.575f;
            KiDrainBuffMulti = 1.4f;
            BaseDefenceBonus = 5;
            Description.SetDefault(AssembleTransBuffDescription());
        }

        // per Nova's design, mastery applies to ASSJ and USSJ
        public override void Update(Player player, ref int buffIndex)
        {
            bool isMastered = MyPlayer.ModPlayer(player).MasteryLevel1 >= 1;

            KiDrainRate = !isMastered ? KiDrainRate : KiDrainRateWithMastery;
            base.Update(player, ref buffIndex);
        }
    }
}

