using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private string FirstSceneGame = "BossRoom";

    public void NewGameStart()
    {
        SceneManager.LoadScene(FirstSceneGame);
    }
}