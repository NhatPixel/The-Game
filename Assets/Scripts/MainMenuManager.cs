using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LoadChatRoom()
    {
        SceneManager.LoadScene("ChatRoom");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
