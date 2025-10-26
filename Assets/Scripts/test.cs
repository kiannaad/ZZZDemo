using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class test : NetworkBehaviour
{
   public void Start()
   {
      
   }
   public void Update()
   {
      if (IsClient && Input.GetKeyDown(KeyCode.Space))
      {
         changeposServerRpc(new Vector3(1, 1, 1));
      }
      Debug.Log($"{transform.name} isClient {IsClient}");
   }

   [ServerRpc]
   public void changeposServerRpc(Vector3 pos)
   {
      changeposClientRpc(pos);
   }
   
   [ClientRpc]
   public void changeposClientRpc(Vector3 pos)
   {
      gameObject.SetActive(false);
      transform.position = pos;
      gameObject.SetActive(true);
   }
}
