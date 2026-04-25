using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

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
    public GameStates State { get; private set; } = GameStates.None;
}
