﻿using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public UnitID UnitID { get; set; }

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
            UnitID = Enumerators.UnitID.None;
            Damage = double.MaxValue;
        }

        #region Specific Commands
        #region Universal Commands
        public static Command MoveCommand(uint unitpoolID, Vector3 moveTo)
        {
            Command command = new Command();
            command.CommandType = CommandType.Move;
            command.TargetType = TargetType.Unit;
            command.ID1 = unitpoolID;
            command.Position = moveTo;
            return command;
        }
        public static Command CancelCommand(uint targetID, TargetType target)
        {
            Command command = new Command();
            command.CommandType = CommandType.Cancel;
            command.TargetType = target;
            command.ID1 = targetID;
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
        public static Command BuildUnitCommand(uint unitpoolID, UnitID unitID, uint factoryID)
        {
            Command command = new Command();
            command.CommandType = CommandType.BuildUnit;
            command.TargetType = TargetType.Unit;
            command.ID1 = unitpoolID;
            command.UnitID = unitID;
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
        public static Command AttackCommand(uint unitpoolID, uint targetID, TargetType target)
        {
            Command command = new Command();
            command.CommandType = CommandType.Attack;
            command.TargetType = target;
            command.ID1 = unitpoolID;
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

        #region Serialization
        public void SetFromOtherCommand(Command o)
        {
            CommandType =   o.CommandType;
            TargetType  =    o.TargetType;
            ID1         =           o.ID1;
            ID2         =           o.ID2;
            Position    =      o.Position;
            UnitID      =        o.UnitID;
            Damage      =        o.Damage;
        }

        public string ThisToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void JsonToThis(string json)
        {
            JObject jObject = JObject.Parse(json);
            Command command = JsonConvert.DeserializeObject<Command>(jObject.ToString());

            SetFromOtherCommand(command);
        }
        #endregion

        public override bool Equals(object obj)
        {
            return Equals((Command)obj);
        }
        public bool Equals(Command o)
        {
            return  ((CommandType == o.CommandType) &&
                    (TargetType == o.TargetType) &&
                    (ID1 == o.ID1) &&
                    (ID2 == o.ID2) &&
                    (Position == o.Position) &&
                    (UnitID == o.UnitID) &&
                    (Damage == o.Damage));
        }
    }
}