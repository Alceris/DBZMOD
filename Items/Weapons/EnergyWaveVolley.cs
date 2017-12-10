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
	public class EnergyWaveVolley : KiItem
	{
		public override void SetDefaults()
		{
			// Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools
			item.shoot = mod.ProjectileType("EnergyWaveVolleyBlast");
			item.shootSpeed = 90f;
			item.damage = 45;
			item.knockBack = 2f;
			item.useStyle = 5;
			item.UseSound = SoundID.Item12;
			item.useAnimation = 25;
			item.useTime = 13;
			item.width = 40;
			item.noUseGraphic = true;
			item.height = 40;
			item.autoReuse = true;
			item.value = Item.sellPrice(0, 0, 5, 0);
			item.rare = 3;
	    }
	    public override void SetStaticDefaults()
		{
		Tooltip.SetDefault("-Tier 3-");
		DisplayName.SetDefault("EnergyWaveVolley");
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(20));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			return true;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "PridefulKiCrystal", 45);
            recipe.AddTile(null, "KiManipulator");
            recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
