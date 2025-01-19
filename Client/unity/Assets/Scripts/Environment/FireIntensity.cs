using Client;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Environment
{
    public class FireIntensity : MonoBehaviour
    {
        [SerializeField] private ParticleSystem fireParticles;
        [SerializeField] private int maxFireRate = 20;
        [SerializeField] private float stopDelay = 2f;
        [SerializeField] private AudioSource fireSound;
        [SerializeField] private Light2D fireLight;

        private float _lastBlowTime;

        private void Start()
        {
            fireParticles.Stop();
            fireSound.volume = 0;
            fireLight.intensity = 0;
        }

        private void Update()
        {
            if (Time.time - _lastBlowTime > stopDelay && fireParticles.isPlaying)
            {
                fireParticles.Stop();
                fireSound.volume = 0;
                fireLight.intensity = 0;
            }
        }

        public void BringFire(string message)
        {
            var fireWindMessage = JsonUtility.FromJson<FireWindMessage>(message);

            if (fireWindMessage.type != "FireWind")
            {
                return;
            }

            _lastBlowTime = Time.time;

            var rateOverTime = maxFireRate * fireWindMessage.fireIntensity;

            var emission = fireParticles.emission;
            emission.rateOverTime = Mathf.RoundToInt(rateOverTime);
            
            fireSound.volume = fireWindMessage.fireIntensity;
            fireLight.intensity = fireWindMessage.fireIntensity;

            if (fireWindMessage.fireIntensity > 0)
            {
                fireParticles.Play();
            }
            else
            {
                fireParticles.Stop();
            }
        }
        
    }
}