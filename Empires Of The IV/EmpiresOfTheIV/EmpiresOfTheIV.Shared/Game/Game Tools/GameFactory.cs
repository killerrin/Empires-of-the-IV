using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Animation;
using Anarian.Enumerators;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects;
using EmpiresOfTheIV.Game.GameObjects.Factories;
using EmpiresOfTheIV.Game.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Game_Tools
{
    public static class GameFactory
    {
        public static void CreateFactoryOnFactoryBase(FactoryBase factoryBase, Player owner)
        {
            factoryBase.Owner = owner.ID;

            factoryBase.Factory = new Factory();
            factoryBase.Factory.Transform.Position = factoryBase.Base.Transform.Position;
            factoryBase.Factory.Transform.Scale = factoryBase.Base.Transform.Scale;
            factoryBase.Factory.Transform.Rotation = factoryBase.Base.Transform.Rotation;
            factoryBase.Factory.Transform.CreateAllMatrices();
            factoryBase.Factory.Active = true;
            factoryBase.Factory.CullDraw = true;
            factoryBase.Factory.RenderBounds = true;
            factoryBase.Factory.Health.MaxHealth = 200.0f;

            switch (owner.EmpireType)
            {
                case EmpireType.UnanianEmpire:              factoryBase.Factory.Model3D = ResourceManager.Instance.GetAsset(typeof(Model), "Unanian Factory") as Model; break;
                case EmpireType.CrescanianConfederation:    factoryBase.Factory.Model3D = ResourceManager.Instance.GetAsset(typeof(Model), "Crescanian Factory") as Model; break;
                case EmpireType.TheKingdomOfEdolas:         factoryBase.Factory.Model3D = ResourceManager.Instance.GetAsset(typeof(Model), "Edolas Factory") as Model; break;
            }
        }

        public static void CreateUnit(Unit unitPoolUnit, UnitID unitID, Vector3 spawnPosition)
        {
            unitPoolUnit.Reset();

            unitPoolUnit.CullDraw = true;
            unitPoolUnit.RenderBounds = true;
            unitPoolUnit.Active = true;
            unitPoolUnit.Health.Alive = true;

            switch (unitID)
            {
                case UnitID.UnanianSoldier:
                    unitPoolUnit.UnitType = UnitType.Soldier;

                    unitPoolUnit.HeightAboveTerrain = 0.0f;
                    unitPoolUnit.Health.MaxHealth = 100.0f;

                    unitPoolUnit.Transform.MovementSpeed = 0.004f;
                    unitPoolUnit.Transform.Scale = new Vector3(0.015f);
                    unitPoolUnit.Transform.Position = spawnPosition;
                    unitPoolUnit.Transform.CreateAllMatrices();
                    
                    unitPoolUnit.Model3D = ResourceManager.Instance.GetAsset(typeof(AnimatedModel), UnitID.UnanianSoldier.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;
                    unitPoolUnit.MovementClip = (ResourceManager.Instance.GetAsset(typeof(AnimatedModel), UnitID.UnanianSoldier.ToString() + "|" + ModelType.Animation.ToString()) as AnimatedModel).Clips[0];
                    
                    var usp = unitPoolUnit.PlayClip(unitPoolUnit.MovementClip);
                    usp.Looping = true;

                    break;
                case UnitID.UnanianMIDAF:
                    break;
                case UnitID.UnanianSpaceFighter:
                    unitPoolUnit.UnitType = UnitType.Space;

                    unitPoolUnit.Health.MaxHealth = 100.0f;
                    unitPoolUnit.HeightAboveTerrain = 10.0f;

                    unitPoolUnit.Transform.MovementSpeed = 0.006f;
                    unitPoolUnit.Transform.Scale = new Vector3(0.005f);
                    unitPoolUnit.Transform.Position = spawnPosition;
                    unitPoolUnit.Transform.CreateAllMatrices();

                    unitPoolUnit.Model3D = ResourceManager.Instance.GetAsset(typeof(AnimatedModel), UnitID.UnanianSpaceFighter.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;
                    break;
            }

            unitPoolUnit.Health.Reset();
            unitPoolUnit.Mana.Reset();
        }
    }
}
