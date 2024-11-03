using UnityEngine;

public class CauldronHeatManager : MonoBehaviour
{
    public float clicksPerSecondRequired = 5f; // Nombre de clics requis par seconde pour chauffer
    public float checkInterval = 1f; // Temps entre chaque vérification en secondes
    public ParticleSystem fireParticles; // Référence au Particle System des flammes
    public ParticleSystem sparksEffect; // Référence au Particle System des étincelles
    [SerializeField] private Collider2D heatZoneCollider; // Référence au BoxCollider2D de la zone de chaleur

    private int clickCount = 0;
    private bool isHot = false;
    private float timer = 0f;

    void Start()
    {
        // Assurez-vous que les particules ne sont pas jouées au départ
        if (fireParticles != null)
        {
            fireParticles.Stop();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Détecter les clics de souris
        if (Input.GetMouseButtonDown(0)) // 0 = clic gauche
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (heatZoneCollider != null && heatZoneCollider == Physics2D.OverlapPoint(mousePosition))
            {
                clickCount++;

                // Joue l'effet d'étincelles au clic
                if (sparksEffect != null)
                {
                    ParticleSystem effect = Instantiate(sparksEffect, mousePosition, Quaternion.identity);
                    effect.Play();
                    Destroy(effect.gameObject, effect.main.duration); // Détruire l'effet après sa durée
                }
            }
        }

        // Vérifier le statut du chaudron après chaque intervalle
        if (timer >= checkInterval)
        {
            if (clickCount >= clicksPerSecondRequired)
            {
                SetCauldronHot(true);
            }
            else
            {
                SetCauldronHot(false);
            }

            // Réinitialiser le compteur et le timer
            clickCount = 0;
            timer = 0f;
        }
    }

    void SetCauldronHot(bool hot)
    {
        if (isHot != hot)
        {
            isHot = hot;
            if (isHot)
            {
                Debug.Log("Le chaudron est maintenant chaud !");
                // Démarrer le système de particules
                if (fireParticles != null)
                {
                    fireParticles.Play();
                }
            }
            else
            {
                Debug.Log("Le chaudron refroidit...");
                // Arrêter le système de particules
                if (fireParticles != null)
                {
                    fireParticles.Stop();
                }
            }
        }
    }

    public bool IsHot()
    {
        return isHot;
    }
}
