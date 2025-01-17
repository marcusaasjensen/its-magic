using UnityEngine;
using System.Collections;
using Client;

namespace Transition
{
    public class HaloEffectController : MonoBehaviour
    {
        public float effectDuration = 2f; 
        public void HaloObject(string message)
        {
            ObjectMessage objectMessage = JsonUtility.FromJson<ObjectMessage>(message);

            if (objectMessage != null && objectMessage.type == "Glow")
            {
                GameObject targetObject = GameObject.Find(objectMessage.targetObject);
                if (targetObject != null)
                {
                    StartCoroutine(ApplyHaloEffect(targetObject));
                }
            }
        }

        private IEnumerator ApplyHaloEffect(GameObject targetObject)
        {
            GelatineEffect gelatineEffect = targetObject.AddComponent<GelatineEffect>();

            yield return new WaitForSeconds(effectDuration);

            Destroy(gelatineEffect);
            Debug.Log($"Effet de gélatine terminé pour l'objet {targetObject.name}.");
        }
    }
}
