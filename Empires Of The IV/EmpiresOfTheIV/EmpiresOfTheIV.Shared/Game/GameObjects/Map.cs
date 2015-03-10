using Anarian.DataStructures.Rendering;
using Anarian.Helpers;
using Anarian.Interfaces;
using Anarian.Pathfinding;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects
{
    public class Map : IUpdatable, IRenderable
    {
        public MapName Name { get; protected set; }
        public Vector2 Size { get { return new Vector2(Terrain.HeightData.TerrainWidth, Terrain.HeightData.TerrainHeight); } }

        public Texture2D BackgroundImage;
        public Terrain Terrain;
        public List<FactoryBase> FactoryBases;

        public List<UnitType> AvailableUnitTypes { get; protected set; }

        public Map(MapName mapName, Texture2D backgroundImage, Terrain terrain, params FactoryBase[] bases)
        {
            Name = mapName;

            BackgroundImage = backgroundImage;

            Terrain = terrain;

            FactoryBases = new List<FactoryBase>();
            foreach (var i in bases)
                FactoryBases.Add(i);

            AvailableUnitTypes = new List<UnitType>();
        }

        #region Buildable Unit Types
        public void AddAvailableUnitType(params UnitType[] unitTypes) { foreach (var i in unitTypes) AvailableUnitTypes.Add(i); }
        public void RemoveAvailableUnitType(UnitType unitType) { AvailableUnitTypes.Remove(unitType); }
        public bool IsUnitTypeBuildable(UnitType unitType)
        {
            foreach (var i in AvailableUnitTypes) 
                if (i == unitType) 
                    return true;
            return false;
        }
        #endregion

        #region Intersections
        public Vector3? IntersectTerrain(Ray ray) { return Terrain.Intersects(ray); }
        public FactoryBaseRayIntersection IntersectFactoryBase(Ray ray, out FactoryBase intersectedFactoryBase) 
        { 
            foreach(var factoryBase in FactoryBases)
            {
                FactoryBaseRayIntersection intersectionResult = factoryBase.CheckRayIntersection(ray);
                if (intersectionResult != FactoryBaseRayIntersection.None)
                {
                    intersectedFactoryBase = factoryBase;
                    return intersectionResult;
                }
            }

            intersectedFactoryBase = null;
            return FactoryBaseRayIntersection.None;
        }
        #endregion

        #region Update/Draw
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }

        public void Update(GameTime gameTime)
        {
            Terrain.Update(gameTime);
            foreach(var i in FactoryBases)
                i.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (BackgroundImage != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(BackgroundImage, AnarianConsts.ScreenRectangle, Color.White);
                spriteBatch.End();
            }

            Terrain.Draw(gameTime, spriteBatch, graphics, camera);
            foreach (var i in FactoryBases)
                i.Draw(gameTime, spriteBatch, graphics, camera);
        }
        #endregion
    }
}
