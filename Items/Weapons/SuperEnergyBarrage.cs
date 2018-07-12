﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Items.Weapons
{
    public class SuperEnergyBarrage : KiItem
    {
        public override void SetDefaults()
        {
            item.shoot = mod.ProjectileType("SuperEnergyBarrageProj");
            item.shootSpeed = 40f;
            item.damage = 98;
            item.knockBack = 5f;
            item.useStyle = 1;
            item.UseSound = SoundID.Item1;
            item.useAnimation = 15;
            item.useTime = 10;
            item.width = 50;
            item.noUseGraphic = true;
            item.height = 50;
            item.autoReuse = true;
            item.value = Item.sellPrice(0, 5, 5, 0);
            item.rare = 5;
            KiDrain = 125;
        }
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("-Tier 5-");
            DisplayName.SetDefault("Super Energy Barrage");
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(12));
            speedX = perturbedSpeed.X;
            speedY = perturbedSpeed.Y;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AngerKiCrystal", 45);
            recipe.AddTile(null, "KiManipulator");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
