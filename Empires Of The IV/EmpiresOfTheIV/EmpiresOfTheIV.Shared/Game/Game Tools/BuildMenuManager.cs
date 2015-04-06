using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Animation;
using Anarian.Enumerators;
using Anarian.Events;
using Anarian.Helpers;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects;
using EmpiresOfTheIV.Game.GameObjects.Factories;
using EmpiresOfTheIV.Game.Players;
using KillerrinStudiosToolkit.Converters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Game_Tools
{
    public class BuildMenuManager : IUpdatable, IRenderable
    {
        public bool Active { get; set; }

        public SpriteFont SpriteFont;
        public Color GuiColor;
        
        Texture2D m_blankTexture;

        public FactoryBase m_activeFactory;
        public BuildMenuType m_buildMenuType;

        public GUIButton m_purchaseButton1;
        public GUIButton m_purchaseButton2;
        public GUIButton m_purchaseButton3;

        Texture2D m_currencyTexture;
        Texture2D m_metalTexture;
        Texture2D m_energyTexture;
        Texture2D m_unitCapTexture;

        #region GameObjects
        AnimatedGameObject m_unit1;
        Cost m_unit1Cost;

        AnimatedGameObject m_unit2;
        Cost m_unit2Cost;

        AnimatedGameObject m_unit3;
        Cost m_unit3Cost;

        Model m_factory;
        Cost m_factoryCost;

        Matrix m_rotationMatrix;
        #endregion

        Player m_me;
        public Rectangle m_uiRectBackground { get; private set; }

        public BuildMenuManager(Color guiColor, Player me)
        {
            SpriteFont = ResourceManager.Instance.GetAsset(typeof(SpriteFont), "EmpiresOfTheIVFont Small") as SpriteFont;
            GuiColor = guiColor;
            
            m_me = me;

            var buttonTexture = ResourceManager.Instance.GetAsset(typeof(Texture2D), "Purchase Button") as Texture2D;
            m_blankTexture = ResourceManager.Instance.GetAsset(typeof(Texture2D), ResourceManager.EngineReservedAssetNames.blankTextureName) as Texture2D;

            var screenRect = AnarianConsts.ScreenRectangle;
            m_uiRectBackground = new Rectangle((int)(screenRect.Width * 0.3),
                                               (int)(screenRect.Height * 0.3),
                                               (int)(screenRect.Width * 0.5),
                                               (int)(screenRect.Height * 0.5));

            var purchaseButton = new Rectangle(0, (int)(screenRect.Height * 0.7), 150, 75);
            m_purchaseButton1 = new GUIButton(buttonTexture, new Rectangle((int)(screenRect.Width * 0.42), purchaseButton.Y, purchaseButton.Width, purchaseButton.Height), Color.White);
            m_purchaseButton2 = new GUIButton(buttonTexture, new Rectangle(), Color.White);
            m_purchaseButton3 = new GUIButton(buttonTexture, new Rectangle((int)(screenRect.Width * 0.65), purchaseButton.Y, purchaseButton.Width, purchaseButton.Height), Color.White);

            m_purchaseButton2.Active = false;

            m_unit1 = new AnimatedGameObject();
            m_unit1.CullDraw = false;

            m_unit2 = new AnimatedGameObject();
            m_unit2.CullDraw = false;

            m_unit3 = new AnimatedGameObject();
            m_unit3.CullDraw = false;

            switch (m_me.EmpireType)
            {
                case EmpireType.UnanianEmpire:
                    m_unit1.Model3D = ResourceManager.Instance.GetAsset(typeof(AnimatedModel), UnitID.UnanianSoldier.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;
                    m_unit1.Transform.Position = new Vector3(-75, 0, -600);
                    m_unit1.Transform.Scale = new Vector3(0.50f);
                    //m_unit1.PlayClip((ResourceManager.Instance.GetAsset(typeof(AnimatedModel), UnitID.UnanianSoldier.ToString() + "|" + ModelType.Animation.ToString()) as AnimatedModel).Clips[0]).Looping = true;
                    m_unit1Cost = GameFactory.CreateUnitCost(UnitID.UnanianSoldier);

                    m_unit3.Model3D = ResourceManager.Instance.GetAsset(typeof(AnimatedModel), UnitID.UnanianSpaceFighter.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;
                    m_unit3.Transform.Position = new Vector3(150, 25, -600);
                    m_unit3.Transform.Scale = new Vector3(0.15f);
                    m_unit3Cost = GameFactory.CreateUnitCost(UnitID.UnanianSpaceFighter);

                    m_factory = ResourceManager.Instance.GetAsset(typeof(Model), "Unanian Factory") as Model;
                    break;
                case EmpireType.CrescanianConfederation:
                    break;
                case EmpireType.TheKingdomOfEdolas:
                    break;
            }
            m_factoryCost = GameFactory.CreateFactoryCost(m_me.EmpireType);

            m_currencyTexture = ResourceManager.Instance.GetAsset(typeof(Texture2D), "Currency") as Texture2D;
            m_metalTexture = ResourceManager.Instance.GetAsset(typeof(Texture2D), "Metal") as Texture2D;
            m_energyTexture = ResourceManager.Instance.GetAsset(typeof(Texture2D), "Energy") as Texture2D;
            m_unitCapTexture = ResourceManager.Instance.GetAsset(typeof(Texture2D), "Unit Cap") as Texture2D;
        }

        public void Enable(FactoryBase factoryBase, BuildMenuType buildMenuType)
        {
            Active = true;
            m_activeFactory = factoryBase;
            m_buildMenuType = buildMenuType;

            switch (m_buildMenuType)
            {
                case BuildMenuType.BuildFactory:    m_purchaseButton1.Active = true;    break;
                case BuildMenuType.BuildUnit:       
                    m_purchaseButton1.Active = true;
                    m_purchaseButton2.Active = true;
                    m_purchaseButton3.Active = true;
                    break;
            }
        }
        public void Disable()
        {
            Active = false;
            m_activeFactory = null;
            m_buildMenuType = BuildMenuType.None;

            m_purchaseButton1.Active = false;
            m_purchaseButton2.Active = false;
            m_purchaseButton3.Active = false;
        }

        public BuildMenuPurchaseSlot CheckPurchaseInput(PointerPressedEventArgs pointer)
        {
            if (m_purchaseButton1.Intersects(pointer.Position))
                return BuildMenuPurchaseSlot.Slot1;
            else if (m_purchaseButton2.Intersects(pointer.Position))
                return BuildMenuPurchaseSlot.Slot2;
            else if (m_purchaseButton3.Intersects(pointer.Position))
                return BuildMenuPurchaseSlot.Slot3;

            return BuildMenuPurchaseSlot.None;
        }

        public UnitID PurchaseSlotToUnitID(BuildMenuPurchaseSlot slot)
        {
            switch (m_me.EmpireType)
            {
                case EmpireType.UnanianEmpire:
                    if      (slot == BuildMenuPurchaseSlot.Slot1) return UnitID.UnanianSoldier;
                    else if (slot == BuildMenuPurchaseSlot.Slot2) return UnitID.UnanianMIDAF;
                    else if (slot == BuildMenuPurchaseSlot.Slot3) return UnitID.UnanianSpaceFighter;
                    break;
                case EmpireType.CrescanianConfederation:
                    break;
                case EmpireType.TheKingdomOfEdolas:
                    break;
            }

            return UnitID.None;
        }

        #region Update/Draw
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public void Update(GameTime gameTime)
        {
            if (!Active) return;

            m_rotationMatrix = Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds);
            m_unit1.Transform.RotationMatrix = m_rotationMatrix;
            m_unit2.Transform.RotationMatrix = m_rotationMatrix;
            m_unit3.Transform.RotationMatrix = m_rotationMatrix;

            m_unit1.Update(gameTime);
            m_unit2.Update(gameTime);
            m_unit3.Update(gameTime);
        }

        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (!Active) return;

            spriteBatch.Begin();
            spriteBatch.Draw(m_blankTexture, m_purchaseButton1.Position, GuiColor);
            spriteBatch.Draw(m_blankTexture, m_purchaseButton2.Position, GuiColor);
            spriteBatch.Draw(m_blankTexture, m_purchaseButton3.Position, GuiColor);
            spriteBatch.End();

            m_purchaseButton1.Draw(gameTime, spriteBatch, graphics, camera);
            m_purchaseButton2.Draw(gameTime, spriteBatch, graphics, camera);
            m_purchaseButton3.Draw(gameTime, spriteBatch, graphics, camera);

            if (m_buildMenuType == BuildMenuType.BuildFactory)
            {

            }
            else if (m_buildMenuType == BuildMenuType.BuildUnit)
            {
                DrawCost(m_unit1Cost, new Vector2(m_purchaseButton1.Position.Left - 20, m_purchaseButton1.Position.Top - 120), gameTime, spriteBatch);
                //DrawCost(m_unit2Cost, new Vector2(m_purchaseButton2.Position.Left - 20, m_purchaseButton2.Position.Top - 120), gameTime, spriteBatch);
                DrawCost(m_unit3Cost, new Vector2(m_purchaseButton3.Position.Left - 20, m_purchaseButton3.Position.Top - 120), gameTime, spriteBatch);
            }
        }

        public void DrawCost(Cost cost, Vector2 position, GameTime gameTime, SpriteBatch spriteBatch)
        {
            int xDistanceBetweenItems = 110;
            int yDistanceBetweenItems = 50;

            int iconWidth = 40;
            int iconHeight = 40;

            spriteBatch.Begin();
            spriteBatch.Draw(m_currencyTexture, new Rectangle((int)position.X, (int)position.Y, iconWidth, iconHeight), Color.White);
            spriteBatch.DrawString(SpriteFont, ShorthandCurrencyConverter.ConvertToShorthand(cost.CurrencyCost), new Vector2(position.X + iconWidth, position.Y), Color.White);

            spriteBatch.Draw(m_metalTexture, new Rectangle((int)position.X, (int)position.Y + yDistanceBetweenItems, iconWidth, iconHeight), Color.White);
            spriteBatch.DrawString(SpriteFont, ShorthandCurrencyConverter.ConvertToShorthand(cost.MetalCost), new Vector2(position.X + iconWidth, position.Y + yDistanceBetweenItems), Color.White);

            spriteBatch.Draw(m_unitCapTexture, new Rectangle((int)position.X + xDistanceBetweenItems, (int)position.Y + 5, iconWidth - 10, iconHeight - 10), Color.White);
            spriteBatch.DrawString(SpriteFont, ShorthandCurrencyConverter.ConvertToShorthand(cost.UnitCost), new Vector2(position.X + xDistanceBetweenItems + iconWidth, position.Y), Color.White);

            spriteBatch.Draw(m_energyTexture, new Rectangle((int)position.X + xDistanceBetweenItems, (int)position.Y + yDistanceBetweenItems, iconWidth, iconHeight), Color.White);
            spriteBatch.DrawString(SpriteFont, ShorthandCurrencyConverter.ConvertToShorthand(cost.EnergyCost), new Vector2(position.X + xDistanceBetweenItems + iconWidth, position.Y + yDistanceBetweenItems), Color.White);

            spriteBatch.End();
        }

        public void Draw3DModels(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (!Active) return;
            
            //// Since we are also using 2D, Reset the
            //// Graphics Device to Render 3D Models properly
            //graphics.BlendState = BlendState.Opaque;
            //graphics.DepthStencilState = DepthStencilState.Default;
            //graphics.SamplerStates[0] = SamplerState.LinearWrap;
            graphics.RasterizerState.CullMode = CullMode.None;// CullCounterClockwiseFace;// CullClockwiseFace;

            var tempView = camera.View;
            camera.View = Matrix.Identity;

            if (m_buildMenuType == BuildMenuType.BuildFactory)
            {
                m_factory.Draw(Matrix.Identity, Matrix.Identity, camera.Projection);
            }
            else if (m_buildMenuType == BuildMenuType.BuildUnit)
            {
                m_unit1.Draw(gameTime, spriteBatch, graphics, camera);
                m_unit2.Draw(gameTime, spriteBatch, graphics, camera);
                m_unit3.Draw(gameTime, spriteBatch, graphics, camera);
            }

            camera.View = tempView;
        }
        #endregion
    }
}
