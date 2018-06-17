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
	public class SuperKamehameha: KiItem
	{
		public override void SetDefaults()
		{
			// Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools
			item.shoot = mod.ProjectileType("SuperKamehamehaBall");
			item.shootSpeed = 0f;
			item.damage = 122;
			item.knockBack = 2f;
			item.useStyle = 5;
			item.UseSound = SoundID.Item12;
			item.useAnimation = 110;
			item.useTime = 110;
			item.width = 40;
			item.noUseGraphic = true;
			item.height = 40;
			item.autoReuse = false;
			item.value = Item.sellPrice(0, 0, 5, 0);
			item.rare = 3;
            item.channel = true;
            KiDrain = 200;
	    }
	    public override void SetStaticDefaults()
		{
		Tooltip.SetDefault("-Tier 4-");
		DisplayName.SetDefault("Super Kamehameha");
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 20f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
		    recipe.AddIngredient(null, "Kamehameha", 1);
			recipe.AddIngredient(null, "PridefulKiCrystal", 40);
		    recipe.AddIngredient(null, "AngerKiCrystal", 40);
            recipe.AddTile(null, "KiManipulator");
            recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
