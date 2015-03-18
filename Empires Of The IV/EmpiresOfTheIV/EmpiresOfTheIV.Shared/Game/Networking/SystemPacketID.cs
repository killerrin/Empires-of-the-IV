using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Networking
{
    public enum SystemPacketID
    {
        None,                       // Nothing
        Ack,
        ConnectionTick,

        // Client Only
        RequestSetupData,           // Sent by the players once they have loaded the game lobby to get the current game information
        JoinTeam1,                  // Sent by the players when one wants to change to Team1
        JoinTeam2,                  // Sent by the players when one wants to change to Team2
        GameLoaded,                 // Sent by the players once they have finished loading the game

        // Host Only
        GameModeChanged,            // Sent by the Host to notify the players of a change in Game Mode
        UnitMaxChanged,             // Sent by the Host to notify the players of a change in Unit Map
        MapChanged,                 // Sent by the Host to notify the players of a change in Map
        TeamsChanged,               // Sent by the Host to notify the players of a change in Teams

        GameStart,                  // Sent by the Host to notify the players to begin displaying the Countdown
        TransitionAndGameLoad,      // Sent by the Host to notify the players to transition the page to the InGamePage and begin Loading
        GameBegin,                  // Sent by the Host when it has recieved notice that everyone has loaded and the Game is ready to begin
        
        Pause,                      // Sent by the Host to notify the players to change GameState to Paused
        Resume,                     // Sent by the Host to notify the players to change GameState to Resumed

        GameSync,                   // Sent by the Host to notify everyone of a GameSync
        
        // Anyone
        Chat,
        WaitingForData,             // A General Waiting for Data state
        Quit                        // Sent by anyone to notify the Host of a Quit 
    }
}
