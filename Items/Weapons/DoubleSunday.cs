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
	public class DoubleSunday: KiItem
	{
		public override void SetDefaults()
		{
			// Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools
			item.shoot = mod.ProjectileType("DoubleSundayBlast");
			item.shootSpeed = 70f;
			item.damage = 36;
			item.knockBack = 2f;
			item.useStyle = 5;
			item.UseSound = SoundID.Item12;
			item.useAnimation = 25;
			item.useTime = 100;
			item.width = 40;
			item.noUseGraphic = true;
			item.height = 40;
			item.autoReuse = false;
			item.value = Item.sellPrice(0, 0, 5, 0);
			item.rare = 2;
	    }
	    public override void SetStaticDefaults()
		{
		Tooltip.SetDefault("-Tier 2.5-");
		DisplayName.SetDefault("Double Sunday");
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			float numberProjectiles = 2 + Main.rand.Next(1); // 3, 4, or 5 shots
			float rotation = MathHelper.ToRadians(20);
			position += Vector2.Normalize(new Vector2(speedX, speedY)) * 20f;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .2f; // Watch out for dividing by 0 if there is only 1 projectile.
				Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
			}
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "EnergyWave", 1);
	        recipe.AddIngredient(null, "PridefulKiCrystal", 35);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
