using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class SceneManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public void LoadScene(string sceneName)
    {
        GameManager.RoundNum = int.Parse(inputField.text);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
