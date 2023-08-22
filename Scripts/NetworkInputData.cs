using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte BUTTON1 = 0x01;
    public const byte BUTTON2 = 0x02;

    public Vector3 direction;
    public byte buttons;
}
