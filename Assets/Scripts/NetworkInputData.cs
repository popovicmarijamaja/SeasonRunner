using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public float MovementDirection;
    public NetworkBool IsJump;
    public NetworkBool IsCrawl;
    public NetworkBool IsShoot;
    public NetworkBool IsMoveLeft;
    public NetworkBool IsMoveCentre;
    public NetworkBool IsMoveRight;

}
