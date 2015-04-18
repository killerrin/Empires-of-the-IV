using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Collections;
using System.Linq;
using Anarian.Collections;
using EmpiresOfTheIV.Game.Networking;
using EmpiresOfTheIV.Game.Commands;
using KillerrinStudiosToolkit.Enumerators;

namespace EmpiresOfTheIV.Game.Game_Tools
{
	public class CommandRelay
	{
		public ThreadSafeList<Command> m_commands;
		Command m_previouslyAddedCommand;

        private object m_inboundCommandLockObject = new object();
		public ThreadSafeList<Command> m_inboundCommands;
		public List<Command> m_outboundCommands;

		public CommandRelay()
		{
			m_commands = new ThreadSafeList<Command>();
			m_previouslyAddedCommand = null;

            m_inboundCommands = new ThreadSafeList<Command>();
            m_outboundCommands = new List<Command>();
		}

        #region Management
        /// <summary>
        /// Adds the Command to the required relay
        /// </summary>
        /// <param name="c">The Command we wish to add</param>
        /// <param name="networkDirection"> Any of the following: 
        ///     Local - if we wish to add directly to the Command Relay                              
        ///     Outbound - if we wish to add to the outbound list and aggregated later                             
        ///     Inbound - If the Command came from another device
        /// </param>
		public void AddCommand(Command c, NetworkTrafficDirection networkDirection)
		{		
            if (networkDirection == NetworkTrafficDirection.Local)
            {
                if (c == m_previouslyAddedCommand)
                    return;

                // If the command is a move command, we check to see if our command relay
                // has other movement commands for that unit. If it does, we remove them
                if (c.CommandType == CommandType.Move)
                    RemoveAllOfRequirements(CommandType.Move, c.ID1);

                // Finally, add the command
                m_commands.Add(c);
                m_previouslyAddedCommand = c;
            }
            else if (networkDirection == NetworkTrafficDirection.Outbound)
            {
                m_outboundCommands.Add(c);
            }
            else if (networkDirection == NetworkTrafficDirection.Inbound)
            {
                lock (m_inboundCommandLockObject)
                {
                    m_inboundCommands.Add(c);
                }
            }
		}

        public void AggregateAndSendCommands()
        {
            foreach (var command in m_outboundCommands)
            {
                AddCommand(command, NetworkTrafficDirection.Local);
            }

            if (Consts.Game.NetworkManager.IsConnected)
            {
                if (m_outboundCommands.Count > 0)
                {
                    GamePacket gp = new GamePacket(true, m_outboundCommands, GamePacketID.Command);
                    Consts.Game.NetworkManager.SendMessage(gp.ThisToJson());
                }
            }
            m_outboundCommands.Clear();


            lock (m_inboundCommandLockObject)
            {   
                var inboundCommands = m_inboundCommands.Clone();
                foreach (var command in inboundCommands)
                {
                    AddCommand(command, NetworkTrafficDirection.Local);
                }
                m_inboundCommands.Clear();
            }
        }

		public void Complete(Command c)
		{
			c.Completed = true;
			m_commands.Remove(c);
		}

		#region Remove
		public void RemoveFirstInstanceOf(CommandType commandType, uint id, TargetType targetType = TargetType.None)
		{
			List<Command> commandsClone = m_commands.Clone();
			foreach (var command in commandsClone)
			{
				if (command.CommandType == commandType &&
					command.ID1 == id)
				{
					if (targetType == TargetType.None) { Complete(command); return; }
					else
					{
						if (command.TargetType == targetType)
						{
							Complete(command);
							return;
						}
					}
				}
			}
		}
		 
		public void RemoveAllOfRequirements(CommandType commandType, uint id, TargetType targetType = TargetType.None)
		{
			List<Command> commandsClone = m_commands.Clone();
			foreach (var command in commandsClone)
			{
				if (command.CommandType == commandType &&
					command.ID1 == id)
				{
					if (targetType == TargetType.None) { Complete(command); }
					else
					{
						if (command.TargetType == targetType)
							Complete(command);
					}
				}
			}
		}

		public void RemoveAllCompleted()
		{
			List<Command> commandsClone = m_commands.Clone();
			foreach (var command in commandsClone)
			{
				if (command.Completed)
					Complete(command);
			}
		}
		#endregion

		public List<Command> GetAllCommandsOfType(CommandType commandType)
		{
			List<Command> temp = new List<Command>();

			List<Command> commandsClone = m_commands.Clone();
			foreach (var command in commandsClone)
			{
				if (command.CommandType == commandType)
					temp.Add(command);
			}

			return temp;
        }

        #endregion
    }
}
