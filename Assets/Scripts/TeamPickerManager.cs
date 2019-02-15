using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XboxCtrlrInput;

public class TeamPickerManager : MonoBehaviour
{
    private XboxController[] controllers = new XboxController[] { XboxController.First, XboxController.Second, XboxController.Third, XboxController.Fourth };
    //private bool starting;
    public Dictionary<XboxController, int> controllerToPlayer;
    private Dictionary<XboxController, string> playerSelection;
    private XboxButton button;
    private static TeamPickerManager Instance = null;
    //UI
    public Text startText;
    public GameObject[] playerSetups;    

    void Awake()
    {
        // If there is not already an instance of TeamPickerManager, set it to this.
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
        playerSelection = new Dictionary<XboxController,string>();
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
            int playerNum = 0;
            foreach (XboxController controller in controllers)
            { 
                if (XCI.GetButtonDown(XboxButton.X,controller)){
                    selected(GameObject.Find("smallPlayer1"), playerNum);
                    playerSelection.Add(controller, "smallPlayer1");
                    setDictionary(XboxButton.X,controller);
                }
                if (XCI.GetButtonDown(XboxButton.Y,controller)){
                    selected(GameObject.Find("tallPlayer1"), playerNum);
                    playerSelection.Add(controller, "tallPlayer1");
                    setDictionary(XboxButton.Y,controller);
                }
                if (XCI.GetButtonDown(XboxButton.A,controller)){
                    selected(GameObject.Find("smallPlayer2"), playerNum);
                    playerSelection.Add(controller, "smallPlayer2");
                    setDictionary(XboxButton.A,controller);
                }
                if (XCI.GetButtonDown(XboxButton.B,controller)){
                    selected(GameObject.Find("tallPlayer2"), playerNum);
                    playerSelection.Add(controller, "tallPlayer2");
                    setDictionary(XboxButton.B,controller);
                }
                if (XCI.GetButtonDown(XboxButton.Back,controller)){
                    unselected(GameObject.Find(playerSelection[controller]), playerNum);
                    controllerToPlayer.Remove(controller);
                }
                playerNum += 1;
            }
            
        }   
    }

    //methods that activate and deactivate appropriate UI based on selection
    void selected(GameObject playerSetup, int playerNum)
    {
        playerNum += 1;
        GameObject.Find("/" + playerSetup.name + "/controller").SetActive(true);
        GameObject.Find("/" + playerSetup.name + "/p" + playerNum.ToString()).SetActive(true);
        GameObject.Find("/" + playerSetup.name + "/backText").SetActive(true);
        GameObject.Find("/" + playerSetup.name + "/buttonText").SetActive(false);
    }

    void unselected(GameObject playerSetup, int playerNum)
    {
        GameObject.Find("/" + playerSetup.name + "/controller").SetActive(false);
        GameObject.Find("/" + playerSetup.name + "/p" + playerNum.ToString()).SetActive(false);
        GameObject.Find("/" + playerSetup.name + "/backText").SetActive(false);
        GameObject.Find("/" + playerSetup.name + "/buttonText").SetActive(true);
    }




    void DebugDictionary(){
        foreach (KeyValuePair<XboxController,int> kvp in controllerToPlayer)
        {
            Debug.Log(kvp.Key + ": " + kvp.Value);
        }
    }
}
