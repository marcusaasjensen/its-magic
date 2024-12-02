using UnityEngine;

namespace Environment
{
    public class FallingObject : Slashable
    {
        [SerializeField] private new string name;
        
        public string Name => name;
    }
}