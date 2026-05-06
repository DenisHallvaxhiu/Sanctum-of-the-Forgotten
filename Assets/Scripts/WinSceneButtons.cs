using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneButtons : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}