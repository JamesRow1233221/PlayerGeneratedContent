using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicTracks;
    private AudioSource audioSource;
    private int trackIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trackIndex = (Random.Range(0, musicTracks.Length));
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            trackIndex = (trackIndex + 1) % musicTracks.Length;
            Debug.Log(trackIndex);
            audioSource.clip = musicTracks[trackIndex];
            audioSource.Play();
            Debug.Log("Playing track: " + musicTracks[trackIndex].name);
        }
    }
}
