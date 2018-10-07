﻿﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Items.Consumables.TestItems
{
    public class SSJGTestItem : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.consumable = true;
            item.maxStack = 1;
            item.UseSound = SoundID.Item3;
            item.useStyle = 2;
            item.useTurn = true;
            item.useAnimation = 17;
            item.useTime = 17;
            item.value = 0;
            item.expert = true;
            item.potion = false;
            item.noUseGraphic = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SSJG Test Item");
            Tooltip.SetDefault("Manually activates the ssjg transformation cutscene and unlocks it.");
        }


        public override bool UseItem(Player player)
        {
            //MyPlayer.ModPlayer(player).SSJ3Transformation();
            UI.TransMenu.MenuSelection = 5;
            MyPlayer.ModPlayer(player).ssjgAchieved = true;
            //MyPlayer.ModPlayer(player).IsTransformingSSJG = true;
            return true;

        }
    }
}