using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class ButonesMultiNe : MonoBehaviour
{
    static bool aperto;
    public void StartClient() {NetworkManager.Singleton.StartClient(); aperto=true;}
    public void StartHost() {NetworkManager.Singleton.StartHost();aperto=true;}

    void Update() { if(aperto) gameObject.SetActive(false);}
}
