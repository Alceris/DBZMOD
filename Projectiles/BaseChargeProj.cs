using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Audio;
using Util;
using Terraria.Enums;

namespace DBZMOD.Projectiles
{
    // unabashedly stolen from blushie's laser example, and then customized WIP
    public class BaseChargeProj : ModProjectile
    {
        // CHARGE/BEAM SETTINGS, these are the things you can change to affect charge appearance behavior!

        #region Things you should definitely mess with

        // the maximum charge level of the ball     
        public float ChargeLimit = 4;

        // this is the minimum charge level you have to have before you can actually fire the beam
        public float MinimumChargeLevel = 4f;

        // the rate at which charge level increases while channeling
        public float ChargeRate = 0.016f; // approximately 1 level per second.

        // Rate at which Ki is drained while channeling
        public int ChargeKiDrainRate = 12;

        // determines the frequency at which ki drain ticks. Bigger numbers mean slower drain.
        public int ChargeKiDrainWindow = 2;

        // Rate at which Ki is drained while firing the beam *without a charge*
        // in theory this should be higher than your charge ki drain, because that's the advantage of charging now.
        public int FireKiDrainRate = 48;

        // determines the frequency at which ki drain ticks while firing. Again, bigger number, slower drain.
        public int FireKiDrainWindow = 2;

        // the rate at which firing drains the charge level of the ball, play with this for balance.
        public float FireDecayRate = 0.036f;

        // the rate at which the charge decays when not channeling
        public float DecayRate = 0.016f; // very slow decay when not channeling

        // this is the beam the charge beam fires when told to.
        public string BeamProjectileName = "BaseBeamProj";

        // this determines how long the max fade in for beam opacity takes to fully "phase in", at a rate of 1f per frame.
        // For the most part, you want to make this the same as the beam's FadeInTime, *unless* you want the beam to stay partially transparent.
        public float BeamFadeInTime = 300f;

        // the type of dust that should spawn when charging or decaying
        public int DustType = 169;

        // the percentage frequency at which dust spawns each frame

        // rate at which decaying produces dust
        public float DecayDustFrequency = 0.6f;

        // rate at which charging produces dust
        public float ChargeDustFrequency = 0.4f;

        // rate at which dispersal of the charge ball (from weapon swapping) produces dust
        public float DisperseDustFrequency = 1.0f; 

        // the amount of dust that tries to spawn when the charge orb disperses from weapon swapping.
        public int DisperseDustQuantity = 40;

        // Bigger number = slower movement. For reference, 60f is pretty fast. This doesn't have to match the beam speed.
        public float RotationSlowness = 15f;

        // this is the default cooldown when firing the beam, in frames, before you can fire again, regardless of your charge level.
        public int BeamFiringCooldown = 180;

        // the charge ball is just a single texture.
        // these two vars specify its draw origin and size, this is a holdover from when it shared a texture sheet with other beam components.
        public Point ChargeOrigin = new Point(0, 0);
        public Point ChargeSize = new Point(18, 18);

        // vector to reposition the charge ball if it feels too low or too high on the character sprite
        public Vector2 ChannelingOffset = new Vector2(0, 4f);

        // vector to reposition the charge ball when the player *isn't* charging it (or firing the beam) - held to the side kinda.
        public Vector2 NotChannelingOffset = new Vector2(-15, 20f);

        #endregion

        #region Things you probably should not mess with

        public const float MAX_DISTANCE = 1000f;
        public const float STEP_LENGTH = 10f;

        #endregion

        #region Things you should not mess with

        // the maximum ChargeLimit of the attack after bonuses from gear (etc) are applied
        public float FinalChargeLimit = 4;

        // the charge level of the projectile currently, increases with channeling and decays without it.
        public float ChargeLevel = 0.0f;

        // the distance charge particle from the player center, as a factor of its size plus some padding.
        public float ChargeBallHeldDistance
        {
            get
            {
                return (ChargeSize.X + ChargeSize.Y) * projectile.scale / 2 + 5f;
            }
        }

        // Any nonzero number is on cooldown
        public float BeamCooldown
        {
            get
            {
                return projectile.ai[1];
            }
            set
            {
                projectile.ai[1] = value;
            }
        }

        // responsible for tracking if the player changed weapons in use, nullifying their charge immediately.
        private int weaponBinding = -1;

        // whether or not the ball is actively firing a blast. When this switches from true to false, the beam dissipates.
        public bool IsSustainingFire = false;

        private Rectangle _chargeRectangle;
        public Rectangle ChargeRectangle
        {
            get
            {
                if (_chargeRectangle == null)
                {
                    _chargeRectangle = new Rectangle(ChargeOrigin.X, ChargeOrigin.Y, ChargeSize.X, ChargeSize.Y);
                }
                return _chargeRectangle;
            }
        }

        #endregion

        public override void SetDefaults()
        {
            projectile.width = ChargeSize.X;
            projectile.height = ChargeSize.Y;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.hide = false;
        }

        public float GetTransparency()
        {
            return projectile.alpha / 255f;
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // don't draw the ball when firing.
            if (IsSustainingFire)
                return false;
            float originalRotation = -1.57f;
            // float experimentalRotation = 200f;
            DrawChargeBall(spriteBatch, Main.projectileTexture[projectile.type], projectile.damage, originalRotation, 1f, MAX_DISTANCE, Color.White);
            return false;
        }

        public Vector2 GetPlayerHandPosition(Player drawPlayer)
        {
            var Position = drawPlayer.position;
            var handVector = new Vector2((float)((int)(Position.X - (float)(drawPlayer.bodyFrame.Width / 2) + (float)(drawPlayer.width / 2))), (float)((int)(Position.Y + (float)drawPlayer.height - (float)drawPlayer.bodyFrame.Height + 4f))) + drawPlayer.bodyPosition + new Vector2((float)(drawPlayer.bodyFrame.Width / 2), (float)(drawPlayer.bodyFrame.Height / 2));
            return handVector;
        }

        public Vector2 GetChargeBallPosition()
        {
            Player player = Main.player[projectile.owner];
            Vector2 positionOffset = player.channel ? ChannelingOffset + projectile.velocity * ChargeBallHeldDistance : NotChannelingOffset + GetPlayerHandPosition(player);
            return Main.player[projectile.owner].Center + positionOffset;
        }

        // The core function of drawing a charge ball
        public void DrawChargeBall(SpriteBatch spriteBatch, Texture2D texture, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color))
        {
            float r = projectile.velocity.ToRotation() + rotation;
            // DebugUtil.Log(string.Format("Trying to draw charge ball, transparency is {0}", GetTransparency()));
            spriteBatch.Draw(texture, GetChargeBallPosition() - Main.screenPosition,
                new Rectangle(0, 0, ChargeSize.X, ChargeSize.Y), color * GetTransparency(), r, new Vector2(ChargeSize.X * .5f, ChargeSize.Y * .5f), scale, 0, 0.99f);
        }

        public void HandleChargingKi(Player player)
        {

            FinalChargeLimit = ChargeLimit + MyPlayer.ModPlayer(player).ChargeLimitAdd;

            // stop channeling if the player is out of ki
            if (MyPlayer.ModPlayer(player).IsKiDepleted())
            {
                player.channel = false;
            }

            // keep alive routine.
            if (projectile.timeLeft < 4)
            {
                projectile.timeLeft = 10;
            }

            MyPlayer modPlayer = MyPlayer.ModPlayer(player);

            // The energy in the projectile decays if the player stops channeling.
            if (!player.channel && !modPlayer.IsMouseRightHeld && !IsSustainingFire)
            {
                if (ChargeLevel > 0f)
                {
                    ChargeLevel = Math.Max(0, ChargeLevel - DecayRate);

                    // dust 169 is some green dust off of final shine (I think). 0.4 is a middling frequency (2/5th of the time), ball is decaying.
                    // don't draw the ball when firing.
                    if (!IsSustainingFire)
                        ProjectileUtil.DoChargeDust(GetChargeBallPosition(), DustType, DecayDustFrequency, true, ChargeSize.ToVector2());
                }
                else
                {
                    // the charge level zeroed out, kill the projectile.
                    projectile.Kill();
                }
            }

            // charge the ball if the proper keys are held.
            // increment the charge timer if channeling and apply slowdown effect
            if (player.channel && projectile.active && modPlayer.IsMouseRightHeld && !IsSustainingFire)
            {
                // drain ki from the player when charging
                if (Main.time > 0 && Math.Ceiling(Main.time % ChargeKiDrainWindow) == 0 && !modPlayer.IsKiDepleted())
                {
                    MyPlayer.ModPlayer(player).AddKi(-ChargeKiDrainRate);
                }

                // increase the charge
                ChargeLevel = Math.Min(FinalChargeLimit, ChargeRate + ChargeLevel);

                // slow down the player while charging.
                ProjectileUtil.ApplyChannelingSlowdown(player);

                // dust 169 is some green dust off of final shine (I think). 0.2 is a relatively low frequency (1/5th of the time), ball is not decaying.
                if (!IsSustainingFire)
                    ProjectileUtil.DoChargeDust(GetChargeBallPosition(), DustType, ChargeDustFrequency, false, ChargeSize.ToVector2());                
            }
        }

        private bool WasSustainingFire = false;

        Projectile MyProjectile = null;
        public void HandleFiring(Player player, Vector2 mouseVector)
        {
            MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();

            // minimum charge level is required to fire in the first place, but once you fire, you can keep firing.
            if ((ChargeLevel >= MinimumChargeLevel || IsSustainingFire) && modPlayer.IsMouseLeftHeld)
            {
                if (!WasSustainingFire)
                {
                    IsSustainingFire = true;

                    // fire the laser!
                    MyProjectile = Projectile.NewProjectileDirect(projectile.position, projectile.velocity, mod.ProjectileType(BeamProjectileName), projectile.damage, projectile.knockBack, projectile.owner);
                }

                MyProjectile.velocity = projectile.velocity;

                // if the player has charge left, drain the ball
                if (ChargeLevel > 0f)
                {
                    ChargeLevel = Math.Max(0f, ChargeLevel - FireDecayRate);
                }
                else if(!modPlayer.IsKiDepleted())
                {
                    if (Main.time > 0 && Main.time % FireKiDrainWindow == 0)
                    {
                        modPlayer.AddKi(-FireKiDrainRate);
                    }
                } else
                {
                    IsSustainingFire = false;

                    if (MyProjectile != null)
                    {
                        ProjectileUtil.StartKillRoutine(MyProjectile);
                    }
                }                
            }
            else
            {
                IsSustainingFire = false;

                if (MyProjectile != null)
                {
                    ProjectileUtil.StartKillRoutine(MyProjectile);
                }
            }
            WasSustainingFire = IsSustainingFire;
        }

        public void HandleChargeBallVisibility()
        {
            var chargeVisibility = (int)Math.Ceiling((Math.Sqrt(ChargeLevel) / Math.Sqrt(FinalChargeLimit)) * 255f);
            projectile.alpha = chargeVisibility;
            projectile.hide = ChargeLevel <= 0f;
        }

        public bool ShouldHandleWeaponChangesAndContinue(Player player)
        {
            if (player.HeldItem == null)
            {
                if (MyProjectile != null)
                {
                    ProjectileUtil.StartKillRoutine(MyProjectile);
                }
                projectile.Kill();
                return false;
            }

            if (weaponBinding == -1)
            {
                weaponBinding = player.HeldItem.type;
            }
            else
            {
                if (player.HeldItem.type != weaponBinding)
                {
                    // do a buttload of decay dust
                    for (var i = 0; i < DisperseDustQuantity; i++)
                    {
                        ProjectileUtil.DoChargeDust(GetChargeBallPosition(), DustType, DisperseDustFrequency, true, ChargeSize.ToVector2());
                    }
                    if (MyProjectile != null)
                    {
                        ProjectileUtil.StartKillRoutine(MyProjectile);
                    }
                    projectile.Kill();
                    return false;
                }
            }
            // weapon's correct, keep doing what you're doing.
            return true;
        }

        // helper field lets us limit mouse movement's impact on the charge ball rotation.
        private Vector2 OldMouseVector = Vector2.Zero;

        // the old screen position helps us offset the MouseWorld vector by our screen position so it's more stable.
        private Vector2 OldScreenPosition = Vector2.Zero;

        // The AI of the projectile
        public override void AI()
        {
            // capture the current mouse vector, we're going to normalize movement prior to updating the charge ball location.
            Vector2 mouseVector = Main.MouseWorld;
            Vector2 screenPosition = Main.screenPosition;
            if (OldMouseVector != Vector2.Zero)
            {
                Vector2 mouseMovementVector = (mouseVector - OldMouseVector) / RotationSlowness;
                //DebugUtil.Log(string.Format("Mouse Movement Vector {0} {1}", mouseMovementVector.X, mouseMovementVector.Y));
                Vector2 screenChange = screenPosition - OldScreenPosition;
                //DebugUtil.Log(string.Format("Weirdness with vectors. Mouse Vector {0} {1} Old Mouse Vector {2} {3}", mouseVector.X, mouseVector.Y, OldMouseVector.X, OldMouseVector.Y));
                mouseVector = OldMouseVector + mouseMovementVector + screenChange;
            }

            // capture the player instance so we can toss it around.
            Player player = Main.player[projectile.owner];

            // track whether charge level has changed by snapshotting it.
            float oldChargeLevel = ChargeLevel;

            // handles the initial binding to a weapon and determines if the player has changed items, which should kill the projectile.
            if (!ShouldHandleWeaponChangesAndContinue(player))
                return;

            // handle.. handling the charge.
            UpdateChargeBallLocationAndDirection(player, mouseVector);

            // handle pouring ki into the ball (or decaying if not channeling)
            HandleChargingKi(player);

            // handle whether the ball should be visible, and how visible.
            HandleChargeBallVisibility();

            // figure out if the player is shooting and fire the laser
            HandleFiring(player, mouseVector);

            // capture the current mouse vector as the previous mouse vector.
            OldMouseVector = mouseVector;

            // capture the current screen position as the previous screen position.
            OldScreenPosition = screenPosition;

            // If we just crossed a threshold, display combat text for the charge level increase.
            if (Math.Floor(oldChargeLevel) != Math.Floor(ChargeLevel) && oldChargeLevel != ChargeLevel)
            {
                Color chargeColor = oldChargeLevel < ChargeLevel ? new Color(51, 224, 255) : new Color(251, 74, 55);
                CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y, player.width, player.height), chargeColor, (int)ChargeLevel, false, false);
            }
        }

        public void UpdateChargeBallLocationAndDirection(Player player, Vector2 mouseVector)
        {
            // custom channeling handler
            if (player.GetModPlayer<MyPlayer>().IsMouseRightHeld)
                player.channel = true;

            // Multiplayer support here, only run this code if the client running it is the owner of the projectile
            if (projectile.owner == Main.myPlayer)
            {
                Vector2 diff = mouseVector - player.Center;
                diff.Normalize();
                projectile.velocity = diff;
                projectile.direction = mouseVector.X > player.position.X ? 1 : -1;
                projectile.netUpdate = true;
            }
            projectile.position = player.Center - new Vector2(0, ChargeSize.Y / 2f) + projectile.velocity * ChargeBallHeldDistance;
            projectile.timeLeft = 2;
            player.heldProj = projectile.whoAmI;
            if (player.channel)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
                int dir = projectile.direction;
                player.ChangeDir(dir);
                player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * dir, projectile.velocity.X * dir);
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}