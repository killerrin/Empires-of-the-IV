using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Commands
{
    public class Command
    {
        #region Fields/Properties
        /// <summary>
        /// The Command Type to provide context
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// Used to provide Context to the Command
        /// </summary>
        public TargetType TargetType { get; set; }

        /// <summary>
        /// Primary Unit or Factory ID
        /// </summary>
        public uint ID1 { get; set; }

        /// <summary>
        /// Secondary Unit or Factory ID. Used to specify target
        /// </summary>
        public uint ID2 { get; set; }

        /// <summary>
        /// A Position used for movements or selection
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Used in the building of units
        /// </summary>
        public UnitType UnitType { get; set; }

        /// <summary>
        /// Used for storing Damage Given to an Object
        /// </summary>
        public double Damage { get; set; }
        #endregion

        public Command()
        {
            CommandType = CommandType.None;
            TargetType = Commands.TargetType.None;
            ID1 = uint.MaxValue;
            ID2 = uint.MaxValue;
            Position = new Vector3();
            UnitType = Enumerators.UnitType.None;
            Damage = double.MaxValue;
        }

        #region Specific Commands
        #region Universal Commands
        public static Command MoveCommand(uint unitID, Vector3 moveTo)
        {
            Command command = new Command();
            command.CommandType = CommandType.Move;
            command.TargetType = TargetType.Unit;
            command.ID1 = unitID;
            command.Position = moveTo;
            return command;
        }
        public static Command CancelCommand(uint id, TargetType target)
        {
            Command command = new Command();
            command.CommandType = CommandType.Cancel;
            command.TargetType = target;
            command.ID1 = id;
            return command;
        }
        public static Command BuildFactoryCommand(uint factoryID)
        {
            Command command = new Command();
            command.CommandType = CommandType.BuildFactory;
            command.TargetType = TargetType.Factory;
            command.ID1 = factoryID;
            return command;
        }
        public static Command BuildUnitCommand(uint unitId, UnitType unitType, uint factoryID)
        {
            Command command = new Command();
            command.CommandType = CommandType.BuildUnit;
            command.TargetType = TargetType.Unit;
            command.ID1 = unitId;
            command.UnitType = unitType;
            command.ID2 = factoryID;
            return command;
        }
        public static Command SetFactoryRallyPointCommand(uint factoryID, Vector3 positionMoveTo)
        {
            Command command = new Command();
            command.CommandType = CommandType.SetFactoryRallyPoint;
            command.TargetType = TargetType.Factory;
            command.ID1 = factoryID;
            command.Position = positionMoveTo;
            return command;
        }
        #endregion

        #region Host Only Commands
        public static Command AttackCommand(uint unitID, uint targetID, TargetType target)
        {
            Command command = new Command();
            command.CommandType = CommandType.Attack;
            command.TargetType = target;
            command.ID1 = unitID;
            command.ID2 = targetID;
            return command;
        }
        public static Command DamageCommand(uint targetID, TargetType target, double damage)
        {
            Command command = new Command();
            command.CommandType = CommandType.Damage;
            command.TargetType = target;
            command.ID1 = targetID;
            command.Damage = damage;
            return command;
        }
        public static Command KillCommand(uint targetID, TargetType target)
        {
            Command command = new Command();
            command.CommandType = CommandType.Kill;
            command.TargetType = target;
            command.ID1 = targetID;
            return command;
        }
        #endregion
        #endregion
    }
}
