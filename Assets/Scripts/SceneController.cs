using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] GameObject colorPicker;
    [SerializeField] GameObject challengePanel;

    public void openColorPicker()
    {
        colorPicker.SetActive(true);
    }

    public void closeColorPicker()
    {
        colorPicker.SetActive(false);
    }

    public void openChallenge()
    {
        challengePanel.SetActive(true);
    }

    public void closeChallenge()
    {
        challengePanel.SetActive(false);
    }

    public void loadGame()
    {
        SceneManager.LoadScene(0);
    }
}
