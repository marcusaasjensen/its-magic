using Client;
using UnityEngine;

namespace Environment
{
    public class Bellows : MonoBehaviour
    {
        [SerializeField] private ParticleSystem windParticles;
        [SerializeField] private int maxWindRate = 20;
        [SerializeField] private float stopDelay = 2f; // Time in seconds before stopping particles

        private float _lastBlowTime;
        
        private void Start() => windParticles.Stop();

        private void Update()
        {
            if (Time.time - _lastBlowTime > stopDelay && windParticles.isPlaying)
            {
                windParticles.Stop();
            }
        }

        public void Blow(string message)
        {
            var fireWindMessage = JsonUtility.FromJson<BreathMessage>(message);

            if (fireWindMessage.type != "Wind")
            {
                return;
            }

            _lastBlowTime = Time.time;

            var rateOverTime = maxWindRate * fireWindMessage.windIntensity;

            var emission = windParticles.emission;
            emission.rateOverTime = Mathf.RoundToInt(rateOverTime);

            if (fireWindMessage.windIntensity > 0)
            {
                windParticles.Play();
            }
            else
            {
                windParticles.Stop();
            }
        }
    }
}