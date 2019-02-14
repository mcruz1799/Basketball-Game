using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using UnityEngine.UI;
public class ControllerManager : MonoBehaviour
{   
    public XboxController controller;
    //private XboxController[] controllers = new XboxController[] { XboxController.First, XboxController.Second, XboxController.Third, XboxController.Fourth };
    public Dictionary<XboxController,int> controllerToPlayer;

    public void setDictionary(Button button)
    {
        if (button.CompareTag("SmallPlayer1"))
        {
            controllerToPlayer.Add(controller, 0);
        }
        if (button.CompareTag("TallPlayer1"))
        {
            controllerToPlayer.Add(controller, 1);
        }
        if (button.CompareTag("SmallPlayer2"))
        {
            controllerToPlayer.Add(controller, 2);
        }
        if (button.CompareTag("TallPlayer2"))
        {
            controllerToPlayer.Add(controller, 3);
        }
    }
}
