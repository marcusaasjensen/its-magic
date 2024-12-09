﻿using Client;
using UnityEngine;

namespace Environment
{
    public class FallingObjectManager : MonoBehaviour
    {
        [SerializeField] private GameObject fallenObjectPrefab;

        public void Fall(string message)
        {
            if(message == null)
            {
                return;
            }
            
            var fallingObjectMessage = JsonUtility.FromJson<FallingObjectMessage>(message);
            if(fallingObjectMessage is not { type: "FallingObject" })
            {
                return;
            }
            
            var fallenObject = Instantiate(fallenObjectPrefab, new Vector3(fallingObjectMessage.x, 0, 0), Quaternion.identity);
        }
    }
}