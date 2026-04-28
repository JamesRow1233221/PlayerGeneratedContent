using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrackSelectionUI : MonoBehaviour
{
    public TrackConnecting trackConnecting;
    public Transform buttonContainer;
    public GameObject buttonPrefab;
    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;

    private List<Button> trackButtons = new List<Button>();
    private int currentSelectedIndex = 0;
    public int players = 4;

    private void Start()
    {
        if(trackConnecting == null)
        {
            Debug.LogError("TrackConnecting reference not assigned!");
            return;
        }

        GenerateTrackButtons();
    }

    void GenerateTrackButtons()
    {
        TrackConnecting.TrackType[] trackTypes = trackConnecting.GetTrackTypes();

        for(int i = 0; i < trackTypes.Length; i++)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            Button button = buttonObj.GetComponent<Button>();
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();

            if(buttonText != null)
            {
                buttonText.text = trackTypes[i].name;
            }
            else
            {
                Debug.LogWarning("Button Text is missing!");
            }
           

            int index = i;
            button.onClick.AddListener(() => OnTrackButtonClicked(index));
            button.interactable = false;
            trackButtons.Add(button);
        }
        List<Button> tempBut = new List<Button>(trackButtons);
        for(int i=0; i < players; i++)
        {
            int g = Random.Range(0, tempBut.Count);
            tempBut[g].interactable = true;
            tempBut.RemoveAt(g);
        }
        

        UpdateButtonColors();
    }

    void OnTrackButtonClicked(int index)
    {
        currentSelectedIndex = index;
        trackConnecting.SelectTrackType(index, trackButtons[index]);
        UpdateButtonColors();
    }

    void UpdateButtonColors()
    {
        for(int i = 0; i < trackButtons.Count; i++)
        {
            ColorBlock colors = trackButtons[i].colors;
            colors.normalColor = (i == currentSelectedIndex) ? selectedColor : normalColor;
            trackButtons[i].colors = colors;
        }
    }
}
    