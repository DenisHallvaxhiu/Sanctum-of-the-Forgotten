using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private string FirstSceneGame = "PuzzleRoom";

    public void NewGameStart()
    {
        SceneManager.LoadScene(FirstSceneGame);
    }
}