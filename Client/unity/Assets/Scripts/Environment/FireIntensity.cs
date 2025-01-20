using Client;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Environment
{
    public class FireIntensity : MonoBehaviour
    {
        [SerializeField] private ParticleSystem smallFireParticles;
        [SerializeField] private ParticleSystem mediumFireParticles;
        [SerializeField] private ParticleSystem largeFireParticles;
        [SerializeField] private int maxFireRate = 20;
        [SerializeField] private float stopDelay = 2f;
        [SerializeField] private AudioSource fireSound;
        [SerializeField] private Light2D fireLight;

        private float _lastBlowTime;

        private void Start()
        {
            smallFireParticles.Stop();
            mediumFireParticles.Stop();
            largeFireParticles.Stop();
            fireSound.volume = 0;
            fireLight.intensity = 0;
        }

        private void Update()
        {
            if (Time.time - _lastBlowTime > stopDelay && smallFireParticles.isPlaying && mediumFireParticles.isPlaying && largeFireParticles.isPlaying)
            {
                smallFireParticles.Stop();
                mediumFireParticles.Stop();
                largeFireParticles.Stop();
                fireSound.volume = 0;
                fireLight.intensity = 0;
            }
        }

        public void BringFire(string message)
        {
            var fireMessage = JsonUtility.FromJson<FireMessage>(message);

            if (fireMessage.type != "Fire")
            {
                return;
            }

            _lastBlowTime = Time.time;

            var rateOverTime = maxFireRate * fireMessage.fireIntensity;

            var emission = smallFireParticles.emission;
            emission.rateOverTime = Mathf.RoundToInt(rateOverTime);
            
            fireSound.volume = fireMessage.fireIntensity;
            fireLight.intensity = fireMessage.fireIntensity;

            if (fireMessage.fireIntensity > 0 && fireMessage.fireIntensity <= 0.33)
            {
                smallFireParticles.Play();
                mediumFireParticles.Stop();
                largeFireParticles.Stop();
            }
            else if (fireMessage.fireIntensity > 0.33 && fireMessage.fireIntensity <= 0.66)
            {
                smallFireParticles.Play();
                mediumFireParticles.Play();
                largeFireParticles.Stop();
            }
            else if (fireMessage.fireIntensity > 0.66)
            {
                smallFireParticles.Play();
                mediumFireParticles.Play();
                largeFireParticles.Play();
            }
            else
            {
                smallFireParticles.Stop();
                mediumFireParticles.Stop();
                largeFireParticles.Stop();
            }
        }
        
    }
}