using Anarian.Interfaces;
using EmpiresOfTheIV.Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Game_Tools
{
    public class UnitPool : IUpdatable, IRenderable
    {
        public int TotalUnitsInPool { get; protected set; }

        public List<Unit> m_activeUnits;
        public List<Unit> m_inactiveUnits;
        public List<Unit> m_myActiveUnits;

        public bool AreAnyUnitsCurrentlySelected;

        public UnitPool(int totalUnitsInPool)
        {
            TotalUnitsInPool = totalUnitsInPool;
            m_activeUnits = new List<Unit>(TotalUnitsInPool);
            m_inactiveUnits = new List<Unit>(TotalUnitsInPool);
            m_myActiveUnits = new List<Unit>(TotalUnitsInPool / 2);

            AreAnyUnitsCurrentlySelected = false;
        }

        public List<Unit> GetAllSelectedUnits()
        {
            List<Unit> selectedUnits = new List<Unit>();
            foreach (var unit in m_activeUnits)
            {
                if (unit.Selected)
                    selectedUnits.Add(unit);
            }

            return selectedUnits;
        }

        public List<Unit> GetAllMyActiveUnits(uint myPlayerID)
        {
            m_myActiveUnits.Clear();
            foreach (var unit in m_activeUnits)
            {
                if (unit.PlayerID == myPlayerID)
                    m_myActiveUnits.Add(unit);
            }

            return m_myActiveUnits;
        }

        public void Clear()
        {
            m_activeUnits.Clear();
            m_inactiveUnits.Clear();
        }

        public bool SwapPool(uint unitID, out Unit swappedUnit)
        {
            Unit unit;
            PoolStatus poolLocation = FindUnit(unitID, out unit);

            swappedUnit = unit;

            switch (poolLocation)
            {
                case PoolStatus.Active:
                    m_activeUnits.Remove(unit);
                    m_inactiveUnits.Add(unit);
                    return true;
                case PoolStatus.Inactive:
                    m_inactiveUnits.Remove(unit);
                    m_activeUnits.Add(unit);
                    return true;
                case PoolStatus.None:
                default:
                    return false;
            }
        }

        public Unit FirstInactiveOfPlayer(uint playerID)
        {
            foreach (var unit in m_inactiveUnits)
            {
                if (unit.PlayerID == playerID)
                    return unit;
            }
            return null;
        }

        #region Find
        public PoolStatus Find(uint unitID)
        {
            foreach (var unit in m_activeUnits)
            {
                if (unit.UnitID == unitID)
                    return PoolStatus.Active;
            }
            foreach (var unit in m_inactiveUnits)
            {
                if (unit.UnitID == unitID)
                    return PoolStatus.Inactive;
            }

            return PoolStatus.None;
        }
        public PoolStatus FindUnit(uint unitID, out Unit unit)
        {
            foreach (var u in m_activeUnits)
            {
                if (u.UnitID == unitID)
                {
                    unit = u;
                    return PoolStatus.Active;
                }
            }
            foreach (var u in m_inactiveUnits)
            {
                if (u.UnitID == unitID)
                {
                    unit = u;
                    return PoolStatus.Inactive;
                }
            }

            unit = null;
            return PoolStatus.None;
        }

        public Unit FindUnit(PoolStatus poolLocation, uint unitID)
        {
            switch (poolLocation)
            {
                case PoolStatus.Active:
                    foreach (var unit in m_activeUnits)
                    {
                        if (unit.UnitID == unitID)
                            return unit;
                    }
                    break;
                case PoolStatus.Inactive:
                    foreach (var unit in m_inactiveUnits)
                    {
                        if (unit.UnitID == unitID)
                            return unit;
                    }
                    break;
                case PoolStatus.None:
                default:
                    break;
                    
            }

            return null;
        }
        #endregion

        #region Update/Draw
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public void Update(GameTime gameTime)
        {
            foreach (var unit in m_activeUnits)
            {
                unit.Update(gameTime);
            }
        }

        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera, bool creatingShadowMap = false)
        {
            foreach (var unit in m_activeUnits)
            {
                unit.Draw(gameTime, spriteBatch, graphics, camera, creatingShadowMap);
            }
        }
        #endregion
    }
}
