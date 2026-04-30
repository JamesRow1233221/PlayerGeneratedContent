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

    static public string trackToLoad;
    static public int RoundNum = 5;

    static public bool PreLoadedTrack = false;

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
        if(PreLoadedTrack)
        {
            ChangeState(GameStates.PreLoadedRace);
        }
        else State = GameStates.Track;
        
    }

    public static void EndTrackPlacement()
    {
        State = GameStates.Race;
    }

    public static void ChangeState(GameStates newState)
    {
        State = newState;
    }
    
    public static void TrackToLoad(string trackName)
    {
        trackToLoad = trackName;
    }
    
    public static bool CheckState(GameStates checkState)
    {
        return State == checkState;
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
    Results,        // on results screen
    PreLoadedRace
}
