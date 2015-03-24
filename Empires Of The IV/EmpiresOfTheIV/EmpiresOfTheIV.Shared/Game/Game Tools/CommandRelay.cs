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

            // Finally, add the command
            m_commands.Add(c);
            m_previouslyAddedCommand = c;
		}

        public void Complete(Command c)
        {
            c.Completed = true;
            m_commands.Remove(c);
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
