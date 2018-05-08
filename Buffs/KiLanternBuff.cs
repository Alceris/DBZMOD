﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;

namespace DBZMOD.Buffs
{
    public class KiLanternBuff : ModBuff
    {
        private int LanternTimer;
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Ki Lantern");
            Description.SetDefault("Gives some slight ki regen.");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            LanternTimer++;
            if (LanternTimer > 6 && MyPlayer.ModPlayer(player).KiCurrent >= 0)
            {
                MyPlayer.ModPlayer(player).KiCurrent += 1;
                LanternTimer = 0;
            }
        }
    }
}
