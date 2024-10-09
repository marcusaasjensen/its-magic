using UnityEngine;

namespace Managers
{
    public class RandomOffsetAnimation : MonoBehaviour
    {
        [SerializeField] private new AnimationClip animation;
        private Animator _animator;
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.Play(animation.name, 0, Random.Range(0f, Mathf.RoundToInt(animation.length)));
        }
    }
}