using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtn : MonoBehaviour
{

    public void ChangeSceneByName(string name)
    {
        SceneManager.LoadScene("Level1");
    }
}
