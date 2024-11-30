using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public interface ISlashable
    {
        void Slash();
        UnityEvent OnSlash { get; }
    }
}