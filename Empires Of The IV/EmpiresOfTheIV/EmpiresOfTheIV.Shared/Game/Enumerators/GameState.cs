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
            PlayGame,
                Singleplayer,
                BluetoothMultiplayer,
                LanMultiplayer,

                    GameLobby,
                        EmpireSelection,

                            InGame, //--|
                            Paused, //--|
                                    //  |
                                    GameOver,

            Options,
            Credits     
    }
}
