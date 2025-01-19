using Client;
using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    public class Cauldron : MonoBehaviour
    {
        [SerializeField] private GameObject sideCauldron;
        [SerializeField] private float timeToBeOnFire = 3f;
        [SerializeField] private float timeBeforeReset = 1f;
        [SerializeField] private UnityEvent onFireAmountCompleted;
        
        public bool IsPotionReady => _timeLeft <= 0;
        
        private bool _isPositioned;
        private bool _isOnFire;
        private float _timeLeft;
        private float _fireTime;

        private void Start()
        {
            _timeLeft = timeToBeOnFire;
            _isPositioned = false;
        }

        private void Update()
        {
            if (Time.time - _fireTime >= timeBeforeReset)
            {
                _isOnFire = false;
            }
            if (_isOnFire)
            {
                _timeLeft -= Time.deltaTime;
                if (_timeLeft <= 0)
                {
                    onFireAmountCompleted.Invoke();
                    _timeLeft = 0;
                }
            }
        }
        
        public void PositionCauldron()
        {
            var cauldronMessage = new CauldronSnapMessage
            {
                isSnapped = true
            };
            
            _isPositioned = true;
            
            WebSocketClient.Instance.SendMessage(JsonUtility.ToJson(cauldronMessage));
        }
        
        public void RemoveCauldron()
        {
            var cauldronMessage = new CauldronSnapMessage
            {
                isSnapped = false
            };
            
            _isPositioned = false;
            
            WebSocketClient.Instance.SendMessage(JsonUtility.ToJson(cauldronMessage));
        }

        public void UpdateSideCauldron(string message)
        {
            var cauldronSnapMessage = JsonUtility.FromJson<CauldronSnapMessage>(message);
            if(cauldronSnapMessage is not { type: "CauldronSnap" })
            {
                return;
            }
            
            sideCauldron.SetActive(cauldronSnapMessage.isSnapped);
        }
        
        public void SetOnFire(string message)
        {
            if (!_isPositioned)
            {
                return;
            }
            
            var fireMessage = JsonUtility.FromJson<FireMessage>(message);
            if (fireMessage is not { type: "Fire" })
            {
                return;
            }
            
            _fireTime = Time.time;
            
            _isOnFire = fireMessage.fireIntensity > 0;
        }
        
    }
}