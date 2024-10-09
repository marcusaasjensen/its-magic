using System;
using UnityEngine;

namespace Environment
{
    public class TriggerTest : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            print(other.gameObject);
        }
    }
}