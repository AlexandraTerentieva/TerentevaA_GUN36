using UnityEngine;

public class SceneController : MonoBehaviour
{
    public void OpenMainScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void OpenGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
}