using System.Collections;
using UnityEngine;

public class DragArcParticleSystem : MonoBehaviour
{
    [Header("Paramètres des Arcs")]
    [SerializeField] private float rayonInitial = 1f;
    [SerializeField] private float vitesseExpansion = 2f;
    [SerializeField] private float angleArc = 90f;
    [SerializeField] private float dureeVieArc = 2f;
    [SerializeField] private float epaisseurLigne = 0.1f;

    [Header("Paramètres de Génération")]
    [SerializeField] private float delaiEntreArcs = 0.5f;
    [SerializeField] private Color couleurArc = Color.cyan;
    [SerializeField] private Material materialArc;

    private Coroutine animationCoroutine;
    private float lastDragTime;
    private bool isDragging;
    private Animator targetAnimator;

    private void Awake()
    {
        // Récupère l'Animator au démarrage
        targetAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Vérifie si le drag a cessé
        if (isDragging && Time.time - lastDragTime > dureeVieArc)
        {
            StopArcAnimation();
        }
    }

    public void OnDragged()
    {
        // Met à jour le temps du dernier drag
        lastDragTime = Time.time;

        // Si l'animation n'est pas active, démarrez-la
        if (!isDragging)
        {
            StartArcAnimation();
        }
    }

    private void StartArcAnimation()
    {
        isDragging = true;

        // Désactive l'Animator si il existe
        if (targetAnimator != null)
        {
            targetAnimator.enabled = false;
        }

        animationCoroutine = StartCoroutine(GenererArcsEnBoucle());
    }

    private void StopArcAnimation()
    {
        isDragging = false;
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // Réactive l'Animator
        if (targetAnimator != null)
        {
            targetAnimator.enabled = true;
        }
    }

    private IEnumerator GenererArcsEnBoucle()
    {
        while (isDragging)
        {
            GenererArcs();
            yield return new WaitForSeconds(delaiEntreArcs);
        }
    }

    private void GenererArcs()
    {
        for (int i = 0; i < 4; i++) // Génère 4 arcs à 90° d'écart
        {
            float angleDebut = i * 90f;
            float rayonActuel = rayonInitial;

            // Crée un nouveau GameObject pour l'arc
            GameObject arcObj = new GameObject("Arc");
            arcObj.transform.SetParent(transform); // Attache l'arc comme enfant de l'objet
            arcObj.transform.localPosition = Vector3.zero; // Assure que l'arc suit la position locale

            LineRenderer lineRenderer = arcObj.AddComponent<LineRenderer>();
            lineRenderer.startWidth = epaisseurLigne;
            lineRenderer.endWidth = epaisseurLigne;
            lineRenderer.material = materialArc;
            lineRenderer.useWorldSpace = false; // Utilise l'espace local pour suivre le parent

            // Animation de la vibration
            StartCoroutine(AnimerVibrationsArc(lineRenderer, angleDebut, rayonActuel));
        }
    }

    private IEnumerator AnimerVibrationsArc(LineRenderer lineRenderer, float angleDebut, float rayonInitial)
    {
        float rayonActuel = rayonInitial;
        float tempsEcoule = 0f;

        // Animation de vibration avec une fonction sinusoïdale
        while (tempsEcoule < dureeVieArc)
        {
            // Calcul de l'oscillation sinusoïdale pour simuler les vibrations
            float amplitudeVibration = Mathf.Sin(tempsEcoule * vitesseExpansion) * 0.2f; // Oscillation
            rayonActuel = rayonInitial + amplitudeVibration;

            Vector3[] positions = new Vector3[30];
            for (int j = 0; j < positions.Length; j++)
            {
                float angle = angleDebut + j / (float)(positions.Length - 1) * angleArc;
                float radians = angle * Mathf.Deg2Rad;

                float x = rayonActuel * Mathf.Cos(radians);
                float y = rayonActuel * Mathf.Sin(radians);

                positions[j] = new Vector3(x, y, 0); // Position relative dans l'espace local
            }

            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);

            tempsEcoule += Time.deltaTime;
            yield return null;
        }

        // Cleanup après la fin de l'animation
        Destroy(lineRenderer.gameObject, dureeVieArc);
    }
}