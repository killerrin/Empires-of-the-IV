using System;
using System.Collections.Generic;
using System.Text;


namespace EmpiresOfTheIV.Game.Enumerators
{
    public enum CommandType
    {
        // Movement Commands
        Move,
        Patrol,

        // Attack Commands
        Attack,
        Damage,

        // Management Commands
        Build,

        // Other Commands
        Cancel,
        Remove
    }
}
