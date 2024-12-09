using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Environment; // Pour accéder à FallingObject et FallingObjectCatcher

namespace Managers
{
    public class ItemBarManager : MonoBehaviour
    {
        [SerializeField] private GameObject itemBarContainer; // Container de la barre d'items
        [SerializeField] private FallingObjectCatcher fallingObjectCatcher; // Référence au script FallingObjectCatcher
        private Dictionary<string, Image> itemSlots = new Dictionary<string, Image>(); // Associe ID aux slots

        private void Start()
        {
            // Abonnement à l'événement onFallingObject
            if (fallingObjectCatcher != null)
            {
                fallingObjectCatcher.onFallingObject.AddListener(OnFallingObjectEvent);
            }

            // Initialisation des slots dans la barre d'objets
            InitializeItemSlots();
        }

        private void InitializeItemSlots()
        {
            foreach (Transform child in itemBarContainer.transform)
            {
                var itemImage = child.Find("ItemImage")?.GetComponent<Image>();
                if (itemImage != null)
                {
                    string id = child.gameObject.name; // Utilisation du nom du GameObject comme ID
                    itemSlots.Add(id, itemImage);

                    // Applique l'effet noir transparent à l'item uniquement
                    ApplyShadowEffect(itemImage);
                }
            }
        }

        private void OnFallingObjectEvent(FallingObject fallingObject)
        {
            // Utiliser le nom de l'objet tombant comme ID
            string id = fallingObject.Name; 
            Debug.Log("name : " + id);

            // Vérifie si l'item existe dans la barre
            if (itemSlots.ContainsKey(id))
            {
                Debug.Log($"Mise à jour de l'item bar pour l'objet tombant : {id}");

                // Retire l'effet "ombre" et affiche le sprite normal
                RemoveShadowEffect(itemSlots[id]);
            }
        }

        private void ApplyShadowEffect(Image image)
        {
            // Change la couleur pour rendre le sprite noir et légèrement transparent
            image.color = new Color(0, 0, 0, 0.5f);
        }

        private void RemoveShadowEffect(Image image)
        {
            // Réinitialise la couleur au sprite normal (opaque)
            image.color = Color.white;
        }

        private void OnDestroy()
        {
            // Désabonnement de l'événement onFallingObject
            if (fallingObjectCatcher != null)
            {
                fallingObjectCatcher.onFallingObject.RemoveListener(OnFallingObjectEvent);
            }
        }
    }
}
