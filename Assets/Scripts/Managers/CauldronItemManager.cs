using UnityEngine;
using System.Collections;

public class CauldronItemManager : MonoBehaviour
{
    public GameObject itemToCreate; // L'objet à créer quand un item est déposé dans le chaudron
    public Vector3 itemScale = Vector3.one; // Échelle personnalisée de l'objet créé
    private CauldronHeatManager cauldronHeatManager; // Référence au manager du chaudron
    private int itemCount = 0; // Compteur d'items déposés
    public float transitionDuration = 1f; // Durée de la transition
    public float spawnHeight = 3f; // Hauteur de la montée pour l’effet visuel
    public int itemOrderInLayer = -1; // Ordre dans le layer pour que l'objet soit derrière le chaudron

    // Référence au Particle System à instancier
    public ParticleSystem dropletEffect;

    void Start()
    {
        // Obtenir la référence au CauldronHeatManager
        cauldronHeatManager = GetComponent<CauldronHeatManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ingredient"))
        {
            // Vérifier si le chaudron est chaud avant d'ajouter l'ingrédient
            if (cauldronHeatManager != null && cauldronHeatManager.IsHot())
            {
                itemCount++; // Incrémenter seulement si le chaudron est chaud
                Debug.Log("Un ingrédient a été ajouté dans le chaudron. Compteur d'items: " + itemCount);

                // Définir la position pour l'effet de gouttes
                Vector3 effectPosition = transform.position + new Vector3(0, spawnHeight, 0); // Position au-dessus du chaudron

                // Lancer l'effet de gouttes lorsque l'item est déposé
                if (dropletEffect != null)
                {
                    ParticleSystem dropletInstance = Instantiate(dropletEffect, effectPosition, Quaternion.identity);
                    dropletInstance.Play();
                    Debug.Log("L'effet de gouttes a été déclenché à la position : " + effectPosition); // Log pour vérifier la position
                    Destroy(dropletInstance.gameObject, dropletInstance.main.duration); // Détruire l'effet après sa durée
                }

                // Vérifier si le nombre d'items est suffisant pour créer une potion
                if (itemCount >= 2)
                {
                    // Position de départ (dans le chaudron) et position finale (au-dessus)
                    Vector3 spawnPosition = transform.position;
                    Vector3 finalPosition = transform.position + new Vector3(0, spawnHeight, 0); // Monte davantage

                    // Créer l'objet au départ et définir l'échelle
                    GameObject newItem = Instantiate(itemToCreate, spawnPosition, Quaternion.identity);
                    newItem.transform.localScale = itemScale;

                    // Définir l'ordre dans le layer pour que l'objet apparaisse derrière le chaudron
                    SpriteRenderer spriteRenderer = newItem.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.sortingOrder = itemOrderInLayer;
                    }

                    // Lancer la transition
                    StartCoroutine(MoveToPosition(newItem, finalPosition, transitionDuration));

                    // Réinitialiser le compteur d'items
                    itemCount = 0; // Réinitialiser ici car la potion a été créée

                    // Détruire l'ingrédient
                    Destroy(collision.gameObject);
                }
                else
                {
                    // Détruire l'ingrédient même s'il n'est pas créé
                    Destroy(collision.gameObject);
                }
            }
            else
            {
                Debug.Log("Le chaudron n'est pas chaud, l'ingrédient n'est pas ajouté.");
                // L'ingrédient n'est pas détruit, il reste dans le jeu
            }
        }
    }

    // Coroutine pour déplacer l'objet du point de départ au point final
    private IEnumerator MoveToPosition(GameObject item, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = item.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            item.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        item.transform.position = targetPosition;

        // À la fin du mouvement, place l'objet au-dessus en modifiant l'`Order in Layer`
        SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 1; // Par exemple, le faire apparaître au-dessus en changeant l'order
        }
    }
}
