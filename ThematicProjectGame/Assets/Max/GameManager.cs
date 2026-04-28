using UnityEngine;
using UnityEngine.Events;

public static class GameManager
{
    static private GameStates state;

    static public GameStates State 
    {
        get
        {
            return state;
        }
        private set
        {
            StateSwitched(state, value);
            state = value;
        }
    }

    static public UnityEvent<GameStates, GameStates> stateSwitched;

    [RuntimeInitializeOnLoadMethod]
    public static void StartManager()
    {
        Debug.Log("Game Manager Started");

        state = GameStates.None;
        stateSwitched = new UnityEvent<GameStates, GameStates>();
    }

    public static void StartGame()
    {
        StartPlacingTrack();
    }

    public static void StartPlacingTrack()
    {
        State = GameStates.Track;
    }

    public static void EndTrackPlacement()
    {
        State = GameStates.Race;
    }

    static void StateSwitched(GameStates oldState, GameStates newState)
    {
        Debug.Log("STATE SWITCH: " + oldState.ToString() + " -> " + newState.ToString());
        stateSwitched.Invoke(oldState, newState);
    }
}

public enum GameStates
{
    None,           // not currently in a game
    Track,          // placing track
    TrackToRace,    // transitioning from track placement to racing
    Race,           // racing
    RaceToTrack,    // transitioning from racing to track placement
    Results         // on results screen
}
