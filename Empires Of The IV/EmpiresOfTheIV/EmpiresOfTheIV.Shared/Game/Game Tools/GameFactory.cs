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
            factoryBase.Factory.UpdateBoundsEveryFrame = false;
            //factoryBase.Factory.RenderBounds = true;

            factoryBase.Factory.Health.Alive = true;
            factoryBase.Factory.Health.RegenerateHealth = true;
            factoryBase.Factory.Health.RegenerationRate = 0.5f;
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

            unitPoolUnit.UnitName = unitID;

            unitPoolUnit.Active = true;
            unitPoolUnit.CullDraw = true;
            unitPoolUnit.UpdateBoundsEveryFrame = true;
            //unitPoolUnit.RenderBounds = true;

            unitPoolUnit.Health.RegenerateHealth = false;

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

                    unitPoolUnit.AttackRange.Radius = 8.0f;
                    unitPoolUnit.UnitCost = Cost.FromUnitCost(1.0);
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

                    unitPoolUnit.AttackRange.Radius = 15.0f;
                    unitPoolUnit.UnitCost = Cost.FromUnitCost(2.0);
                    break;
            }

            unitPoolUnit.Health.Reset();
            unitPoolUnit.Mana.Reset();
        }
    }
}
