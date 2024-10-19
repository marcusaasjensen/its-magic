using UnityEngine;

namespace Environment
{
    public class Draggable : MonoBehaviour
    {
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            
            transform.position = other.transform.position;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

        }
    }
}