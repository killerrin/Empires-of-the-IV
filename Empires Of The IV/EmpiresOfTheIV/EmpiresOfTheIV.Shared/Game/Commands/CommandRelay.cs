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
	}
}
