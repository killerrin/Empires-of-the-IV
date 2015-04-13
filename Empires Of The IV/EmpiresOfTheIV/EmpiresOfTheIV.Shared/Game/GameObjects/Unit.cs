using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Animation.Aux;
using Anarian.DataStructures.Components;
using Anarian.Interfaces;
using Anarian.Helpers;
using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using EmpiresOfTheIV.Game.Players;
using Microsoft.Xna.Framework.Audio;

namespace EmpiresOfTheIV.Game.GameObjects
{
    public class Unit : AnimatedGameObject,
                        IUpdatable, IRenderable, ISelectableEntity
    {
        #region Fields/Properties
        public uint UnitID { get; protected set; }
        public uint PlayerID;
        
        public UnitType UnitType;
        public UnitID UnitName;
        public Cost UnitCost;

        public BoundingSphere SightRange;
        public AudioEmitter AudioEmitter;

        public Timer AttackTimer;

        bool m_selected;
        public bool Selectable { get; set; }
        public bool Selected
        {
            get { return m_selected; }
            set {
                m_selected = value;
                RenderBounds = value;
            }
        }

        public Health Health { get { return GetComponent(typeof(Health)) as Health; } }
        public Mana Mana { get { return GetComponent(typeof(Mana)) as Mana; } }

        private Texture2D blankTexture;
        public Point HealthBarOffset;

        public AnimationClip MovementClip;
        public AnimationClip IdleClip;
        public AnimationClip AttackClip;

        public float HeightAboveTerrain;
        public float AttackDamage;
        public double DamageTakenThisFrame;
        #endregion

        public Unit(uint unitID, UnitType unitType)
            : base()
        {
            UnitID = unitID;
            UnitType = unitType;
            UnitName = Enumerators.UnitID.None;

            // By default, the ID is set to the max value to get out of the way
            PlayerID = uint.MaxValue;

            // Setup base Selection rules
            Selectable = true;
            Selected = false;

            // Cash the refrences to textures
            blankTexture = ResourceManager.Instance.GetAsset(typeof(Texture2D), ResourceManager.EngineReservedAssetNames.blankTextureName) as Texture2D;
            
            // Add Unit Specific Components
            AddComponent(typeof(Health));
            AddComponent(typeof(Mana));

            // Other Variables
            MovementClip = null;
            IdleClip = null;
            AttackClip = null;

            HeightAboveTerrain = 0.0f;
            AttackDamage = 1.0f;
            HealthBarOffset = new Point(75, 25);

            SightRange = new BoundingSphere();
            AudioEmitter = new AudioEmitter();
            AttackTimer = new Timer(TimeSpan.FromSeconds(1.0));

            UnitCost = Cost.FromUnitCost(0.0);
        }

        public override void Reset()
        {
            //base.Reset();

            UnitType = Enumerators.UnitType.None;
            UnitName = Enumerators.UnitID.None;
            
            Selectable = true;
            Selected = false;

            MovementClip = null;
            IdleClip = null;
            AttackClip = null;

            HeightAboveTerrain = 0.0f;
            DamageTakenThisFrame = 0.0;
            HealthBarOffset = new Point(60, 30);

            SightRange = new BoundingSphere();
            AttackTimer.Reset();

            Health.Reset();
            Mana.Reset();
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        
        bool ISelectableEntity.Selectable
        {
            get { return Selectable; }
            set { Selectable = value; }
        }

        bool ISelectableEntity.Selected
        {
            get { return Selected; }
            set { Selected = value; }
        }
        #endregion

        #region Helper Methods
        public bool InAttackRange(BoundingSphere o)
        {
            return SightRange.Intersects(o);
        }

        public bool CanAttack()
        {
            if (!Health.Alive) return false;

            return AttackTimer.Progress == Anarian.Enumerators.ProgressStatus.Completed;
        }

        public bool Attack()
        {
            if (!CanAttack()) return false;

            AttackTimer.Reset();
            return true;
        }

        public bool TakeDamage(float damage)
        {
            Health.DecreaseHealth(damage);
            return true;
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update our positions in the SightRange and Audio Emitter
            var worldPos = m_transform.WorldPosition;
            SightRange.Center = worldPos;
            AudioEmitter.Position = worldPos;

            // Update Attack Timer
            AttackTimer.Update(gameTime);
        }

        public virtual bool Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera, bool creatingShadowMap = false)
        {
            bool result = base.Draw(gameTime, spriteBatch, graphics, camera);

            if (!result) return false;

            // Render the Attack Range
            //SightRange.RenderBoundingSphere(graphics, Matrix.Identity, camera.View, camera.Projection, Color.Red);
            
            if (!creatingShadowMap)
                return DrawHealth(gameTime, spriteBatch, graphics, camera);
            return true;
        }

        public bool DrawHealth(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (m_model == null) return false;

            #region Draw the Health
            Vector3 screenPos3D = graphics.Viewport.Project(m_transform.WorldPosition, camera.Projection, camera.View, camera.World);
            Vector2 screenPos2D = new Vector2(screenPos3D.X, screenPos3D.Y);
            Rectangle healthRectOutline = new Rectangle((int)(screenPos2D.X - graphics.Viewport.X) + HealthBarOffset.X,
                                                        (int)(screenPos2D.Y - graphics.Viewport.Y) + HealthBarOffset.Y,
                                                        (int)Health.MaxHealth + 2,
                                                        5);
            Rectangle healthRect = new Rectangle(healthRectOutline.X + 1,
                                                 healthRectOutline.Y + 1,
                                                 (int)(MathHelper.Clamp(Health.CurrentHealth, 0, healthRectOutline.Width - 2)),
                                                 healthRectOutline.Height - 2);

            // Draw the Health rectangle
            spriteBatch.Begin();
            spriteBatch.Draw(blankTexture, healthRectOutline, Color.Black);
            spriteBatch.Draw(blankTexture, healthRect, Color.Red);
            spriteBatch.End();
            #endregion

            return true;
        }

        protected override void SetupEffects(Effect effect, GraphicsDevice graphics, ICamera camera, GameTime gameTime)
        {
            base.SetupEffects(effect, graphics, camera, gameTime);
        }
    }
}
