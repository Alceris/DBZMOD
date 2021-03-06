﻿namespace DBZMOD.Items.DragonBalls
{
    public class OneStarStoneBall : DragonBallItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Inert One Star Dragon Ball");
            Tooltip.SetDefault("It may hold power if returned to its homeworld.");            
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            item.consumable = false;
        }

        public override int GetWhichDragonBall()
        {
            return 1;
        }
    }
}
