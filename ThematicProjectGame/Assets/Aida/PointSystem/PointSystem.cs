using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PointSystem : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    private TrackConnecting trackConnecting;
    private GameObject finishTrack;

    public int[] playerPoints = new int[4];
    public float[] timeInFirst = new float[4];
    public TMP_Text[] playerPointsText = new TMP_Text[4];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdatePointUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdatePointUI()
    {
        foreach (PlayerInput player in playerManager.players)
        {
            int index = player.playerIndex;
            string playerName = player.name;
            playerPointsText[index].text = (playerName.Replace("Car(Clone)", " Car") + "\n" + playerPoints[index]);
        }
    }

    
}
