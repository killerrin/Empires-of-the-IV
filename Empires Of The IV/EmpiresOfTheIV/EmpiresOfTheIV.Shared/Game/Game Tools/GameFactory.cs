using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Animation;
using Anarian.Enumerators;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects;
using EmpiresOfTheIV.Game.GameObjects.Factories;
using EmpiresOfTheIV.Game.GameObjects.ParticleEmitters;
using EmpiresOfTheIV.Game.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EmpiresOfTheIV.Game.Game_Tools
{
    public static class GameFactory
    {
        public static void CreateFactoryOnFactoryBase(FactoryBase factoryBase, Player owner)
        {
            Debug.WriteLine("CreateFactoryOnFactoryBase");

            factoryBase.PlayerID = owner.ID;

            factoryBase.CurrentRallyPoint = factoryBase.DefaultRallyPoint;
            factoryBase.DamageTakenThisFrame = 0.0;

            factoryBase.Factory = new Factory();
            factoryBase.Factory.Transform.Position = factoryBase.Base.Transform.Position;
            factoryBase.Factory.Transform.Scale = factoryBase.Base.Transform.Scale;
            factoryBase.Factory.Transform.Rotation = factoryBase.Base.Transform.Rotation;
            factoryBase.Factory.Transform.CreateAllMatrices();

            factoryBase.Factory.Active = true;
            factoryBase.Factory.CullDraw = false;
            factoryBase.Factory.UpdateBoundsEveryFrame = false;
            factoryBase.Factory.RenderBounds = false;

            factoryBase.Factory.Health.Active = true;
            factoryBase.Factory.Health.Alive = true;
            factoryBase.Factory.Health.Invincible = false;
            factoryBase.Factory.Health.RegenerateHealth = true;
            factoryBase.Factory.Health.RegenerationRate = 0.0001f;
            factoryBase.Factory.Health.RegenerationTimer.Interval = TimeSpan.FromSeconds(5.0);
            factoryBase.Factory.Health.MaxHealth = 500.0f;
            factoryBase.Factory.HealthBarOffset.X -= 150;
            factoryBase.Factory.HealthBarOffset.Y += 80;

            switch (owner.EmpireType)
            {
                case EmpireType.UnanianEmpire:              factoryBase.Factory.Model3D = ResourceManager.Instance.GetAsset(typeof(Model), "Unanian Factory") as Model; break;
                case EmpireType.CrescanianConfederation:    factoryBase.Factory.Model3D = ResourceManager.Instance.GetAsset(typeof(Model), "Crescanian Factory") as Model; break;
                case EmpireType.TheKingdomOfEdolas:         factoryBase.Factory.Model3D = ResourceManager.Instance.GetAsset(typeof(Model), "Edolas Factory") as Model; break;
            }

            factoryBase.Factory.Health.Reset();
        }

        public static void CreateUnit(Unit unitPoolUnit, UnitID unitID, Vector3 spawnPosition)
        {
            Debug.WriteLine("CreateUnit");
            unitPoolUnit.Reset();

            unitPoolUnit.UnitName = unitID;
            unitPoolUnit.UnitCost = CreateUnitCost(unitID);

            unitPoolUnit.Active = true;
            unitPoolUnit.CullDraw = true;
            unitPoolUnit.UpdateBoundsEveryFrame = true;
            unitPoolUnit.Health.RegenerateHealth = false;
            unitPoolUnit.Health.Invincible = false;

            unitPoolUnit.RenderBounds = false;

            switch (unitID)
            {
                case UnitID.UnanianSoldier:
                    unitPoolUnit.UnitType = UnitType.Soldier;

                    unitPoolUnit.HeightAboveTerrain = 0.0f;

                    unitPoolUnit.AttackTimer.Interval = TimeSpan.FromSeconds(0.7);
                    unitPoolUnit.AttackDamage = 1.5f;
                    unitPoolUnit.Health.MaxHealth = 15.0f;
                    unitPoolUnit.DeathParticleEmitter = new BloodSplatterParticleSystem(Vector2.Zero, 20, unitPoolUnit.Transform.WorldPosition);

                    unitPoolUnit.Transform.MovementSpeed = 0.004f;
                    unitPoolUnit.Transform.Scale = new Vector3(0.015f);
                    unitPoolUnit.HealthBarOffset.X = -30;

                    unitPoolUnit.Model3D = ResourceManager.Instance.GetAsset(typeof(AnimatedModel), UnitID.UnanianSoldier.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;
                    unitPoolUnit.MovementClip = (ResourceManager.Instance.GetAsset(typeof(AnimatedModel), UnitID.UnanianSoldier.ToString() + "|" + ModelType.Animation.ToString()) as AnimatedModel).Clips[0];
                    
                    var usp = unitPoolUnit.PlayClip(unitPoolUnit.MovementClip);
                    usp.Looping = true;

                    unitPoolUnit.SightRange.Radius = 10.0f;
                    break;
                case UnitID.UnanianMIDAF:
                    unitPoolUnit.UnitType = UnitType.Soldier;

                    unitPoolUnit.HeightAboveTerrain = 0.0f;
                    
                    unitPoolUnit.AttackTimer.Interval = TimeSpan.FromSeconds(0.6);
                    unitPoolUnit.AttackDamage = 3f;
                    unitPoolUnit.Health.MaxHealth = 35.0f;
                    unitPoolUnit.DeathParticleEmitter = new BloodSplatterParticleSystem(Vector2.Zero, 20, unitPoolUnit.Transform.WorldPosition);

                    unitPoolUnit.Transform.MovementSpeed = 0.005f;
                    unitPoolUnit.Transform.Scale = new Vector3(0.015f);
                    unitPoolUnit.HealthBarOffset.X = -40;

                    unitPoolUnit.SightRange.Radius = 12.0f;
                    break;
                case UnitID.UnanianSpaceFighter:
                    unitPoolUnit.UnitType = UnitType.Space;

                    unitPoolUnit.HeightAboveTerrain = 10.0f;

                    unitPoolUnit.AttackTimer.Interval = TimeSpan.FromSeconds(0.5);
                    unitPoolUnit.AttackDamage = 6.0f;
                    unitPoolUnit.Health.MaxHealth = 75.0f;
                    unitPoolUnit.DeathParticleEmitter = new ExplosionParticleSystem(Vector2.Zero, 20, unitPoolUnit.Transform.WorldPosition);

                    unitPoolUnit.Transform.MovementSpeed = 0.006f;
                    unitPoolUnit.Transform.Scale = new Vector3(0.005f);
                    unitPoolUnit.HealthBarOffset.X = -50;

                    unitPoolUnit.Model3D = ResourceManager.Instance.GetAsset(typeof(AnimatedModel), UnitID.UnanianSpaceFighter.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;

                    unitPoolUnit.SightRange.Radius = 15.0f;
                    break;
            }

            unitPoolUnit.Transform.Position = spawnPosition;
            unitPoolUnit.Transform.CreateAllMatrices();
            unitPoolUnit.Health.Reset();

            unitPoolUnit.DeathParticleEmitter.OnNoActiveParticlesRemaining += unitPoolUnit.ExplosionParticleEmitter_OnNoActiveParticlesRemaining;
        }

        public static UnitType UnitTypeFromUnitID(UnitID unitID)
        {
            switch (unitID)
            {
                case UnitID.UnanianSoldier:         return UnitType.Soldier;
                case UnitID.UnanianMIDAF:           return UnitType.Soldier;
                case UnitID.UnanianSpaceFighter:    return UnitType.Space;
                case UnitID.None:
                default: return UnitType.None;
            }
        }

        public static SoundName SoundNameFromUnitID(UnitID unitID)
        {
            switch (unitID)
            {
                case UnitID.UnanianSoldier:         return SoundName.DeathUnanianSoldier;
                case UnitID.UnanianMIDAF:           return SoundName.DeathUnanianSoldier;
                case UnitID.UnanianSpaceFighter:    return SoundName.DeathUnanianSpaceFighter;
                case UnitID.None:
                default: return SoundName.None;
            }
        }

        public static Cost CreateUnitCost(UnitID unitID)
        {
            switch (unitID)
            {
                case UnitID.UnanianSoldier:         return new Cost(1000.0, 2000.0, 500.0,  1.0);
                case UnitID.UnanianMIDAF:           return new Cost(2000.0, 4000.0, 2000.0, 2.0);
                case UnitID.UnanianSpaceFighter:    return new Cost(5000.0, 5000.0, 5000.0, 2.0);
                case UnitID.None:
                default: return new Cost(0.0, 0.0, 0.0, 0.0);
            }
        }

        public static Cost CreateFactoryCost(EmpireType empireType)
        {
            switch (empireType)
            {
                case EmpireType.UnanianEmpire:
                case EmpireType.CrescanianConfederation:
                case EmpireType.TheKingdomOfEdolas:
                default: return Cost.FromEconomyCost(50000, 50000, 50000);
            }
        }
    }
}
