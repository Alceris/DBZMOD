 using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DBZMOD.Items.Weapons
{
	public class BigBangAttack : KiItem
	{
		public override void SetDefaults()
		{
			item.shoot = mod.ProjectileType("BigBangAttackProjectile");
			item.damage = 32;
			item.shootSpeed = 25f;
			item.knockBack = 5f;
			item.useStyle = 3;
			item.UseSound = SoundID.Item1;
			item.useAnimation = 5;
			item.useTime = 100;
			item.width = 40;
			item.noUseGraphic = true;
			item.height = 40;
			item.autoReuse = false;
			item.value = Item.sellPrice(0, 0, 5, 0);
			item.rare = 4;
            KiDrain = 100;
	    }
	    public override void SetStaticDefaults()
		{
		Tooltip.SetDefault("-Tier 4-");
		DisplayName.SetDefault("Big Bang Attack");
		}


        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "PridefulKiCrystal", 40);
		    recipe.AddIngredient(null, "AngerKiCrystal", 40);
            recipe.AddTile(null, "KiManipulator");
            recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
