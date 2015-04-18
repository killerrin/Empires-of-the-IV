using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Enumerators
{

    /// <summary>
    /// The MenuState
    /// </summary>
    public enum GameState
    {
        None,
        SplashScreen,
        
        MainMenu,
            Singleplayer,
            BluetoothMultiplayer,
            LanMultiplayer,

                GameLobby,
                    EmpireSelection,

                        InGame,
                                GameOver,

            Options,
            Credits     
    }
}
