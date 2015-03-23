using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Collections;
using System.Linq;
using Anarian.Collections;
using EmpiresOfTheIV.Game.Networking;

namespace EmpiresOfTheIV.Game.Commands
{
	public class CommandRelay
	{
		public ThreadSafeList<Command> m_commands;

		public CommandRelay()
		{
			m_commands = new ThreadSafeList<Command>();
		}

		public void AddCommand(Command c, bool sendOverNetwork)
		{
			m_commands.Add(c);
			
			// Propogate Commands accross Network
            if (sendOverNetwork)
            {
                if (Consts.Game.NetworkManager.IsConnected)
                {
                    GamePacket gp = new GamePacket(true, c, GamePacketID.Command);
                    Consts.Game.NetworkManager.SendMessage(gp.ThisToJson());
                }
            }
		}
		public void RemoveAllCompleted()
		{
			List<Command> commandsClone = m_commands.Clone();
			foreach (var command in commandsClone)
			{
				if (command.Completed)
					m_commands.Remove(command);
			}
		}
	}
}
