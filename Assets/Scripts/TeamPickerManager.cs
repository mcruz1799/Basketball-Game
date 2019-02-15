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
    public GameObject[] sp1PlayerNums;
    public GameObject sp1ButtonText;
    public GameObject sp1BackText;
    public GameObject sp1ControllerPic;
    public GameObject[] sp2PlayerNums;
    public GameObject sp2ButtonText;
    public GameObject sp2BackText;
    public GameObject sp2ControllerPic;
    public GameObject[] tp1PlayerNums;
    public GameObject tp1ButtonText;
    public GameObject tp1BackText;
    public GameObject tp1ControllerPic;
    public GameObject[] tp2PlayerNums;
    public GameObject tp2ButtonText;
    public GameObject tp2BackText;
    public GameObject tp2ControllerPic;

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
        //DebugDictionary();

    }
    private bool spotOpen(int playerNumber){
        Debug.Log("Player number = " + playerNumber);
        foreach (int value in controllerToPlayer.Values)
        {
            Debug.Log("value = " + value);
            if (value == playerNumber) return false;
        }
        Debug.Log("true");
        return true;
    }
    private void Update() 
    {
        if (controllerToPlayer.Keys.Count == 4)
        {
            // //display text "press start to play"
            startText.gameObject.SetActive(true);
            if (XCI.GetButtonDown(XboxButton.Start)){
                SceneManager.LoadScene("MainGame");
            }
        }
        else
        {   
            startText.gameObject.SetActive(false);
            int playerNum = 0;
            foreach (XboxController controller in controllers)
            { 
                try{
                    if (XCI.GetButtonDown(XboxButton.X,controller)){
                        //Debug.Log("playerNum = " + playerNum);

                        playerSelection.Add(controller, "sp1");
                        selected("sp1",playerNum);
                        setDictionary(XboxButton.X,controller);
                    }
                    if (XCI.GetButtonDown(XboxButton.Y,controller)){
                        playerSelection.Add(controller, "tp1");
                        selected("tp1",playerNum);
                        setDictionary(XboxButton.Y,controller);
                    }
                    if (XCI.GetButtonDown(XboxButton.A,controller)){
                        playerSelection.Add(controller, "sp2");
                        selected("sp2",playerNum);
                        setDictionary(XboxButton.A,controller);
                    }
                    if (XCI.GetButtonDown(XboxButton.B,controller)){
                        playerSelection.Add(controller, "tp2");
                        selected("tp2",playerNum);
                        setDictionary(XboxButton.B,controller);
                    }
                    if (XCI.GetButtonDown(XboxButton.Back,controller)){
                        unselected(playerSelection[controller]);
                        playerSelection.Remove(controller);
                        controllerToPlayer.Remove(controller);
                    }
                    playerNum += 1;
                }catch{Debug.Log("Already assigned");}
            }
            
        }   
    }

    //methods that activate and deactivate appropriate UI based on selection
    void selected(string player, int playerNum)
    {
        playerNum += 1;
        if (player == "sp1"){
            foreach (GameObject obj in sp1PlayerNums)
            {
                if (obj.name.Contains(playerNum.ToString())) obj.SetActive(true);
            }
            sp1BackText.SetActive(true);
            sp1ControllerPic.SetActive(true);
            sp1ButtonText.SetActive(false);
        }
        else if (player == "sp2"){
            foreach (GameObject obj in sp2PlayerNums)
            {
                if (obj.name.Contains(playerNum.ToString())) obj.SetActive(true);
            }
            sp2BackText.SetActive(true);
            sp2ControllerPic.SetActive(true);
            sp2ButtonText.SetActive(false);
        }
        else if (player == "tp1"){
            foreach (GameObject obj in tp1PlayerNums)
            {
                if (obj.name.Contains(playerNum.ToString())) obj.SetActive(true);
            }
            tp1BackText.SetActive(true);
            tp1ControllerPic.SetActive(true);
            tp1ButtonText.SetActive(false);
        }
        else if (player == "tp2"){
            foreach (GameObject obj in tp2PlayerNums)
            {
                if (obj.name.Contains(playerNum.ToString())) obj.SetActive(true);
            }
            tp2BackText.SetActive(true);
            tp2ControllerPic.SetActive(true);
            tp2ButtonText.SetActive(false);
        }
    }
    void unselected(string player)
    {
        if (player == "sp1"){
            foreach (GameObject obj in sp1PlayerNums)
            {
                obj.SetActive(false);
            }
            sp1BackText.SetActive(false);
            sp1ControllerPic.SetActive(false);
            sp1ButtonText.SetActive(true);
        }
        else if (player == "sp2"){
            foreach (GameObject obj in sp2PlayerNums)
            {
                obj.SetActive(false);
            }
            sp2BackText.SetActive(false);
            sp2ControllerPic.SetActive(false);
            sp2ButtonText.SetActive(true);
        }
        else if (player == "tp1"){
            foreach (GameObject obj in tp1PlayerNums)
            {
                obj.SetActive(false);
            }
            tp1BackText.SetActive(false);
            tp1ControllerPic.SetActive(false);
            tp1ButtonText.SetActive(true);
        }
        else if (player == "tp2"){
            foreach (GameObject obj in tp2PlayerNums)
            {
                obj.SetActive(false);
            }
            tp2BackText.SetActive(false);
            tp2ControllerPic.SetActive(false);
            tp2ButtonText.SetActive(true);
        }
    }






    void DebugDictionary(){
        foreach (KeyValuePair<XboxController,int> kvp in controllerToPlayer)
        {
            Debug.Log(kvp.Key + ": " + kvp.Value);
        }
    }
}
