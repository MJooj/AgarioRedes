using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;

[DisallowMultipleComponent]
public class TransformSinc : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
