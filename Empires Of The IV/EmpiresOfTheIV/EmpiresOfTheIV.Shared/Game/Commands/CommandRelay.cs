using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Collections;
using System.Linq;
using Anarian.Collections;

namespace EmpiresOfTheIV.Game.Commands
{
	public class CommandRelay
	{
		public ThreadSafeList<Command> m_commands;

		public CommandRelay()
		{
			m_commands = new ThreadSafeList<Command>();
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
