using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class TeamPickerManager : MonoBehaviour
{
    private XboxController[] controllers = new XboxController[] { XboxController.First, XboxController.Second, XboxController.Third, XboxController.Fourth };
    public Text startText;
    private bool starting;
    private ControllerManager cm;
    private Dictionary<XboxController, int> controllerToPlayer;
    private XboxButton button;
    private static TeamPickerManager Instance = null;

    void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //Set TeamPickerManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
        controllerToPlayer = new Dictionary<XboxController,int>();
    }
    public void setDictionary(XboxButton button, XboxController controller)
    {
        if (!controllerToPlayer.ContainsKey(controller)){
            if (button == XboxButton.X)
            {
                if (spotOpen(0)) controllerToPlayer.Add(controller, 0);
            }
            if (button == XboxButton.A)
            {
                if (spotOpen(1)) controllerToPlayer.Add(controller, 1);
            }
            if (button == XboxButton.B)
            {
                if (spotOpen(2)) controllerToPlayer.Add(controller, 2);
            }
            if (button == XboxButton.Y)
            {
                if (spotOpen(3)) controllerToPlayer.Add(controller, 3);
            }
        }
        DebugDictionary();

    }
    private bool spotOpen(int playerNumber){
        foreach (int value in controllerToPlayer.Values)
        {
            if (value == playerNumber) return false;
        }
        return true;
    }
    private void Update() 
    {
        if (controllerToPlayer.Keys.Count == 4)
        {
            // //display text "press start to play"
            // startText.gameObject.SetActive(true);
            // if (XCI.GetButtonDown(XboxButton.Start)){
            //     SceneManager.LoadScene("MainGame");
            // }
        }
        else
        {   
            startText.gameObject.SetActive(false);
            foreach (XboxController controller in controllers)
            {
                if (XCI.GetButtonDown(XboxButton.X,controller)){
                    setDictionary(XboxButton.X,controller);
                }
                if (XCI.GetButtonDown(XboxButton.Y,controller)){
                    setDictionary(XboxButton.Y,controller);
                }
                if (XCI.GetButtonDown(XboxButton.A,controller)){
                    setDictionary(XboxButton.A,controller);
                }
                if (XCI.GetButtonDown(XboxButton.B,controller)){
                    setDictionary(XboxButton.B,controller);
                }
                if (XCI.GetButtonDown(XboxButton.Back,controller)){
                    controllerToPlayer.Remove(controller);
                }
            }
            
        }   
    }




    void DebugDictionary(){
        foreach (KeyValuePair<XboxController,int> kvp in controllerToPlayer)
        {
            Debug.Log(kvp.Key + ": " + kvp.Value);
        }
    }
}
