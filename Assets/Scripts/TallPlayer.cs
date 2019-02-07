using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BallUserComponent))]
[RequireComponent(typeof(PlayerMover))]
public class TallPlayer : MonoBehaviour, IBallUser, IXzController
{   

    public SmallPlayer above {get; private set;}
    private IXzController xzController;
    private BallUserComponent ballUserComponent;
    // Start is called before the first frame update
    void Awake()
    {
        xzController = GetComponent<PlayerMover>();
        ballUserComponent = GetComponent<BallUserComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ThrowSmallPlayer()
    {

    }
    public void Pass()
    {
        ballUserComponent.Pass(xzController.XLook, xzController.ZLook);
    }
    //IXzController

    
    public float X => xzController.X;
    public float Z => xzController.Z;

    public float XLook => xzController.XLook;
    public float ZLook => xzController.ZLook;
}
