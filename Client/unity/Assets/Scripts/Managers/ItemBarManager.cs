using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Environment;
using System.Linq;
using Client;

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
        [SerializeField] private GameObject downCommunicationArea; // Référence au GameObject "Down Communication"
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

            /*if (fallingObjectCatcher != null)
            {
                fallingObjectCatcher.onFallingObject.AddListener(OnFallingObjectEvent);
            }*/

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
            if (!itemSlots.TryGetValue(id, out Image itemImage))
            {
                Debug.LogWarning($"L'item '{id}' n'existe pas dans les slots.");
                return;
            }

            if (!activeItems.Contains(id))
            {
                Debug.Log($"L'item '{id}' n'a pas encore été collecté. Animation de refus.");
                StartItemRefusalAnimation(itemImage); // Lancer l'animation de refus
                return;
            }

            Debug.Log($"Item cliqué : {id}");
            StartItemAnimation(itemImage); // Lancer l'animation d'agrandissement

            if (id == "Mushroom" || id == "Firefly")
            {
                Debug.Log("Aucun item trouvé dans la scène pour cet objet. Activation de DownCommunication.");
                StartCoroutine(ShowDownCommunication());
                return;
            }

            // Recherche d'objets correspondants
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


        private IEnumerator ShowDownCommunication()
{
    if (downCommunicationArea == null)
    {
        Debug.LogWarning("DownCommunicationArea n'est pas défini !");
        yield break;
    }

    // Activer DownCommunication
    downCommunicationArea.SetActive(true);
    Debug.Log("DownCommunication activée.");

    // Animation de grossissement et rétrécissement
    RectTransform downRect = downCommunicationArea.GetComponent<RectTransform>();
    Vector3 originalScale = downRect.localScale;
    Vector3 targetScale = originalScale * 1.2f; // Grossissement à 120%

    float elapsedTime = 0f;
    float pulseDuration = animationDuration / 2; // Durée d'un cycle (agrandissement ou rétrécissement)

    for (int i = 0; i < animationLoops; i++)
    {
        // Grossissement
        elapsedTime = 0f;
        while (elapsedTime < pulseDuration)
        {
            downRect.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / pulseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Rétrécissement
        elapsedTime = 0f;
        while (elapsedTime < pulseDuration)
        {
            downRect.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / pulseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // Réinitialiser l'échelle et désactiver DownCommunication
    downRect.localScale = originalScale;
    downCommunicationArea.SetActive(false);
    Debug.Log("DownCommunication désactivée.");
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


        /*private void OnFallingObjectEvent(FallingObject fallingObject)
        {
            string id = fallingObject.Name;

            if (itemSlots.ContainsKey(id))
            {
                RemoveShadowEffect(itemSlots[id]);
                activeItems.Add(id); // Ajoute à la liste des items activés
            }
        }*/

        private void UpdateItemState(string objectId)
        {
            if (!int.TryParse(objectId, out int index) || index < 1 || index > itemBarContainer.transform.childCount)
            {
                return;
            }

            Transform itemTransform = itemBarContainer.transform.GetChild(index - 1);
            if (itemTransform == null)
            {
                return;
            }

            if (activeItems.Contains(itemTransform.name))
            {
                return;
            }

            var itemImage = itemTransform.Find("ItemImage")?.GetComponent<Image>();
            if (itemImage == null)
            {
                return;
            }
            RemoveShadowEffect(itemImage);
            activeItems.Add(itemTransform.name);
        }



        public void UpdateItemStateMessage(string message)
        {
            if (message == null)
            {
                return;
            }

            var itemBagMessage = JsonUtility.FromJson<ItemBagMessage>(message);
            if (itemBagMessage is not { type: "AddItem" })
            {
                return;
            }
            UpdateItemState(itemBagMessage.objectId);
        }

        private void ApplyShadowEffect(Image image)
        {
            image.color = new Color(0, 0, 0, 0.5f); // Noir et semi-transparent
        }

        private void RemoveShadowEffect(Image image)
        {
            image.color = Color.white; // Réinitialise au sprite normal
        }

        private void StartItemAnimation(Image itemImage)
        {
            StartCoroutine(ItemPulseAnimation(itemImage));
        }

        private IEnumerator ItemPulseAnimation(Image itemImage)
        {
            Vector3 originalScale = itemImage.rectTransform.localScale;
            Vector3 targetScale = originalScale * 1.2f; // Agrandissement à 120%

            for (int i = 0; i < animationLoops; i++)
            {
                // Agrandissement
                float elapsedTime = 0f;
                while (elapsedTime < animationDuration / 2)
                {
                    itemImage.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / (animationDuration / 2));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // Rétrécissement
                elapsedTime = 0f;
                while (elapsedTime < animationDuration / 2)
                {
                    itemImage.rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / (animationDuration / 2));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }

            itemImage.rectTransform.localScale = originalScale; // Réinitialise la taille
        }

        private void StartItemRefusalAnimation(Image itemImage)
        {
            StartCoroutine(ItemRefusalAnimation(itemImage));
        }

        private IEnumerator ItemRefusalAnimation(Image itemImage)
        {
            Vector3 originalPosition = itemImage.rectTransform.localPosition; // Position d'origine
            float vibrationDistance = 10f; // Distance du déplacement
            int vibrationLoops = 6; // Nombre de vibrations (chaque boucle = gauche + droite)
            float vibrationSpeed = 0.05f; // Vitesse de chaque vibration (en secondes)

            for (int i = 0; i < vibrationLoops; i++)
            {
                // Déplacement à gauche
                itemImage.rectTransform.localPosition = originalPosition + new Vector3(-vibrationDistance, 0, 0);
                yield return new WaitForSeconds(vibrationSpeed);

                // Déplacement à droite
                itemImage.rectTransform.localPosition = originalPosition + new Vector3(vibrationDistance, 0, 0);
                yield return new WaitForSeconds(vibrationSpeed);
            }

            // Réinitialiser à la position d'origine
            itemImage.rectTransform.localPosition = originalPosition;
        }

    }
}
