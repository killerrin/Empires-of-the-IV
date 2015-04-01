using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Collections;
using System.Linq;
using Anarian.Collections;
using EmpiresOfTheIV.Game.Networking;
using EmpiresOfTheIV.Game.Commands;

namespace EmpiresOfTheIV.Game.Game_Tools
{
	public class CommandRelay
	{
		public ThreadSafeList<Command> m_commands;
		Command m_previouslyAddedCommand;

		public CommandRelay()
		{
			m_commands = new ThreadSafeList<Command>();
			m_previouslyAddedCommand = null;
		}

		public void AddCommand(Command c, bool sendOverNetwork)
		{
			if (c == m_previouslyAddedCommand)
				return;
			
			// Propogate Commands accross Network
			if (Consts.Game.NetworkManager.IsConnected)
			{
				if (sendOverNetwork)
				{
					GamePacket gp = new GamePacket(true, c, GamePacketID.Command);
					Consts.Game.NetworkManager.SendMessage(gp.ThisToJson());
				}
			}

			// If the command is a move command, we check to see if our command relay
			// has other movement commands for that unit. If it does, we remove them
			if (c.CommandType == CommandType.Move)
				RemoveAllOfRequirements(CommandType.Move, c.ID1);

			// Finally, add the command
			m_commands.Add(c);
			m_previouslyAddedCommand = c;
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
	}
}
