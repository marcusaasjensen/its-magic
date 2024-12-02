using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class Slashable : MonoBehaviour, ISlashable
    {
        [SerializeField] private UnityEvent onSlash;
        [SerializeField] private float slashRadius = 1f;

        public UnityEvent OnSlash => onSlash;

        public void Slash()
        {
            onSlash.Invoke();
            // Additional logic for slashing behavior, like destroying the object or triggering animations
        }
    }
}