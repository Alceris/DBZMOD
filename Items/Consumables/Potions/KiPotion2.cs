﻿﻿using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Items.Consumables.Potions
{
    public class KiPotion2 : KiPotion
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.consumable = true;
            item.maxStack = 30;
            item.UseSound = SoundID.Item3;
            item.useStyle = 2;
            item.useTurn = true;
            item.useAnimation = 12;
            item.useTime = 12;
            item.value = 400;
            item.rare = 3;
            item.potion = false;
            IsKiPotion = true;
            KiHeal = 640;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ki Potion");
            Tooltip.SetDefault("Restores 640 Ki.");
        }
		 public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CalmKiCrystal", 1);
            recipe.AddIngredient(null, "KiPotion1", 2);
            recipe.AddTile(TileID.Bottles);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}