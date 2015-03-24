using System;
using System.Collections.Generic;
using System.Text;


namespace EmpiresOfTheIV.Game.Commands
{
    public enum CommandType
    {
        None,
        StartSelection,
        EndSelection,

        ///--------------------
        /// Networked Commands
        ///--------------------
        //-- Universal Commands
        // Movement Commands
        Move,
        // Management Commands
        BuildFactory,
        BuildUnit,
        SetFactoryRallyPoint,
        // Other Commands
        Cancel,

        //-- Host Commands
        // Attack Commands
        Attack,
        Damage,
        Kill,
    }
}
