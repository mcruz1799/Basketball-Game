using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO: Possibly integrate this into GameManager, and setup GameManager with DDOL property.
public class MenuManager : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("MainGame");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
