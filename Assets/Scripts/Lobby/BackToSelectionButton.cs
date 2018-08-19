using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSelectionButton : MonoBehaviour {

    public string modeSelectionScene;
    public GameObject lobbyManager;
    public void BackButton()
    {
        Destroy(lobbyManager);
        SceneManager.LoadScene(modeSelectionScene);
    }
}
