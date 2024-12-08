using Client;
using Environment;
using UnityEngine;

namespace Player
{
    public class MagicStick : Draggable
    {
        private float _currentRotationInDegrees;
       [SerializeField]
        private bool isLocked;
        private SpriteRenderer _magicWandSpriteRenderer;
        [SerializeField]
        private Sprite mangicWand;
        [SerializeField]
        private Sprite lockedMangicWand;
        private AudioSource _audioSource;
        [SerializeField]
        private AudioClip[] audioClip;
        private bool _hasPlayedLockedSound = false;
        private bool _hasPlayedUnlockedSound = false;
        
        private void Start()
        {
            var childTransform = transform.Find("magic_wand");
            if (childTransform != null)
            {
                _audioSource = GetComponent<AudioSource>();
                _magicWandSpriteRenderer = childTransform.GetComponent<SpriteRenderer>();
            }
        }
        
        private new void Update()
        {
            base.Update();

            if (isLocked)
            {
                if (!_hasPlayedLockedSound)
                {
                    _audioSource.clip = audioClip[0];
                    _audioSource.Play();
                    _hasPlayedLockedSound = true;
                    _hasPlayedUnlockedSound = false; 
                }

                _magicWandSpriteRenderer.sprite = lockedMangicWand;
            }
            else
            {
                if (!_hasPlayedUnlockedSound) 
                {
                    _audioSource.clip = audioClip[1];
                    _audioSource.Play();
                    _hasPlayedUnlockedSound = true; 
                    _hasPlayedLockedSound = false; 
                }

                _magicWandSpriteRenderer.sprite = mangicWand;
                _currentRotationInDegrees = transform.rotation.eulerAngles.z < 0
                    ? 360 + transform.rotation.eulerAngles.z
                    : transform.rotation.eulerAngles.z;

                var magicStickMessage = new MagicStickMessage
                {
                    rotationInDegrees = _currentRotationInDegrees
                };
                WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(magicStickMessage));
            }
        }
    }
}