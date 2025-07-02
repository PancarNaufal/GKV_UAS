using UnityEngine;

public class Main_Menu : MonoBehaviour
{
    public void Playgame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void Quitgame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
