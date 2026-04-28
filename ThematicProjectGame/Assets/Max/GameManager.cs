using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
    private GameStates state;

    public GameStates State 
    {
        get
        {
            return state;
        }
        private set
        {
            StateSwitched(value, state);
            state = value;
        }
    }

    public UnityEvent<GameStates, GameStates> stateSwitched;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        state = GameStates.None;
        stateSwitched = new UnityEvent<GameStates, GameStates>();

        Debug.Log(Instance);
    }

    public void StartGame()
    {
        StartPlacingTrack();
    }

    public void StartPlacingTrack()
    {
        State = GameStates.Track;
    }

    public void EndTrackPlacement()
    {
        State = GameStates.Race;
    }

    void StateSwitched(GameStates oldState, GameStates newState) 
    {
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
