using UnityEngine;

public class Container : MonoBehaviour
{
    public GameObject emptySlot;  // Le slot vide dans le conteneur où le sprite de l'objet collecté sera affiché
    private SpriteRenderer emptySlotRenderer;
    private GameObject collectedObject;  // Référence à l'objet collecté dans le conteneur

    private void Start()
    {
        emptySlotRenderer = emptySlot.GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Draggable") && collectedObject == null)
        {
            collectedObject = collision.gameObject;
            emptySlotRenderer.sprite = collectedObject.GetComponent<SpriteRenderer>().sprite;
            collectedObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        if (collectedObject != null)
        {
            collectedObject.SetActive(true);
            collectedObject.transform.position = transform.position + new Vector3(1, 1, 0);
            emptySlotRenderer.sprite = null;
            collectedObject = null;
        }
    }
}