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
    private BoxCollider hitbox;
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

    //IBallUser
    //-----------------------------------------------
    public void Pass()
    {
        ballUserComponent.Pass(xzController.XLook, xzController.ZLook);
    }

    public bool Steal()
    {
        return ballUserComponent.Steal(hitbox);
    }
    public bool HasBall => ballUserComponent.HasBall;
    
    
    //IXzController
    //------------------------------------------
    public float X => xzController.X;
    public float Z => xzController.Z;

    public float XLook => xzController.XLook;
    public float ZLook => xzController.ZLook;
    public void Move(float xMove, float zMove) {
        xzController.Move(xMove, zMove);
    }
    public void SetRotation(float xLook, float zLook) {
        xzController.SetRotation(xLook, zLook);
    }




}
