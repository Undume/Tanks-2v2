using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionModeController : MonoBehaviour
{
    public string scene2v2;
    public string sceneFFA;
    
    public void ModeTeam()
    {
       SceneManager.LoadScene(scene2v2);
    }

    public void ModeFFA()
    {
        SceneManager.LoadScene(sceneFFA);
    }
}