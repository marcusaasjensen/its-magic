using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Environment; // Pour accéder à FallingObject et FallingObjectCatcher
using System.Linq;

namespace Managers
{
    public class ItemBarManager : MonoBehaviour
    {
        [SerializeField] private GameObject itemBarContainer; // Container de la barre d'items
        [SerializeField] private FallingObjectCatcher fallingObjectCatcher; // Référence au script FallingObjectCatcher
        private Dictionary<string, Image> itemSlots = new Dictionary<string, Image>(); // Associe ID aux slots
        private HashSet<string> activeItems = new HashSet<string>(); // IDs des items activés (sans ombre)
        [SerializeField] private GameObject leftCommunicationArea; // Référence au GameObject "Left Communication"
        [SerializeField] private GameObject rightCommunicationArea; // Référence au GameObject "Right Communication"
        private RectTransform leftButtonRect;
        private RectTransform rightButtonRect;
        [SerializeField] private float animationDuration = 1f; // Durée totale de l'animation
        [SerializeField] private int animationLoops = 3; // Nombre de grossissements/rétrécissements


        private void Start()
        {
            // Récupère les RectTransform des GameObjects
            if (leftCommunicationArea != null)
                leftButtonRect = leftCommunicationArea.GetComponent<RectTransform>();

            if (rightCommunicationArea != null)
                rightButtonRect = rightCommunicationArea.GetComponent<RectTransform>();


            if (fallingObjectCatcher != null)
            {
                fallingObjectCatcher.onFallingObject.AddListener(OnFallingObjectEvent);
            }

            InitializeItemSlots();
        }

        private void InitializeItemSlots()
        {
            foreach (Transform child in itemBarContainer.transform)
            {
                var itemImage = child.Find("ItemImage")?.GetComponent<Image>();
                if (itemImage != null)
                {
                    string id = child.gameObject.name;
                    itemSlots.Add(id, itemImage);
                    ApplyShadowEffect(itemImage);

                    // Ajouter un événement pour le touch input
                    var button = child.GetComponent<Button>();
                    if (button == null)
                        button = child.gameObject.AddComponent<Button>();

                    button.onClick.AddListener(() => OnItemBarClicked(id));
                }
            }
        }

        private void OnItemBarClicked(string id)
        {
            
            if (!activeItems.Contains(id))
            {
                Debug.Log($"L'item '{id}' n'a pas encore été collecté.");
                return;
            }

            Debug.Log($"Item cliqué : {id}");
            GameObject[] allItems = GameObject.FindGameObjectsWithTag("Slashable");

            var matchingItems = allItems
                .Where(obj => obj.GetComponent<FallingObject>()?.Name == id)
                .ToList();

            if (!matchingItems.Any())
            {
                Debug.Log("Aucun item correspondant trouvé dans la scène.");
                return;
            }

            float centerX = Camera.main.transform.position.x;
            GameObject closestItem = null;
            float minDistance = float.MaxValue;

            foreach (var item in matchingItems)
            {
                if (!IsItemVisible(item)) // On ignore les items visibles
                {
                    float distance = Mathf.Abs(item.transform.position.x - centerX);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestItem = item;
                    }
                }
            }

            if (closestItem != null)
            {
                if (closestItem.transform.position.x < centerX)
                {
                    Debug.Log($"L'item le plus proche ({id}) est à GAUCHE.");
                    StartButtonAnimation(leftButtonRect);
                }
                else
                {
                    Debug.Log($"L'item le plus proche ({id}) est à DROITE.");
                    StartButtonAnimation(rightButtonRect);
                }
            }
            else
            {
                Debug.Log("Tous les items sont visibles. Pas besoin de déplacer.");
            }
        }


        private void StartButtonAnimation(RectTransform button)
        {
            StartCoroutine(ButtonPulseAnimation(button));
        }

        private IEnumerator ButtonPulseAnimation(RectTransform button)
        {
            Vector3 originalScale = button.localScale;
            Vector3 targetScale = originalScale * 1.2f; // Grossissement à 120%

            for (int i = 0; i < animationLoops; i++)
            {
                // Grossissement
                float elapsedTime = 0f;
                while (elapsedTime < animationDuration / 2)
                {
                    button.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / (animationDuration / 2));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Rétrecissement
                elapsedTime = 0f;
                while (elapsedTime < animationDuration / 2)
                {
                    button.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / (animationDuration / 2));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }

            button.localScale = originalScale; // Réinitialise la taille
        }


        private bool IsItemVisible(GameObject item)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            var renderer = item.GetComponent<Renderer>();

            if (renderer == null) return false; // Pas de Renderer, on considère l'objet comme invisible
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }


        private void OnFallingObjectEvent(FallingObject fallingObject)
        {
            string id = fallingObject.Name;

            if (itemSlots.ContainsKey(id))
            {
                RemoveShadowEffect(itemSlots[id]);
                activeItems.Add(id); // Ajoute à la liste des items activés
            }
        }

        private void ApplyShadowEffect(Image image)
        {
            image.color = new Color(0, 0, 0, 0.5f); // Noir et semi-transparent
        }

        private void RemoveShadowEffect(Image image)
        {
            image.color = Color.white; // Réinitialise au sprite normal
        }

        private void OnDestroy()
        {
            if (fallingObjectCatcher != null)
            {
                fallingObjectCatcher.onFallingObject.RemoveListener(OnFallingObjectEvent);
            }
        }
    }
}
