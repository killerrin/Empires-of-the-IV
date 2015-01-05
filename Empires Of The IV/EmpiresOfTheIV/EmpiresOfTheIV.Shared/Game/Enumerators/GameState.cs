using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Enumerators
{
    public enum GameState
    {
        None,
        SplashScreen,
        
        MainMenu,
            PlayGame,
                Singleplayer, //--|
                Multiplayer,  //--|
                              //  |
                    EmpireSelection,
                        InGame, //--|
                        Paused, //--|
                                //  |
                             GameOver,

            Options,
            Credits     
    }
}
