using UnityEngine;
using System.Collections;
using Client;

namespace Transition
{
    using System.Collections;
    using UnityEngine;

    public class HaloEffectController : MonoBehaviour
    {
        public float effectDuration = 2.0f;
        public RuntimeAnimatorController bagAnimatorController;
        public RuntimeAnimatorController bellowsAnimatorController;
        public RuntimeAnimatorController almarnClockAnimatorController;

        private IEnumerator ApplyHaloEffect(GameObject targetObject)
        {
            Animator animator = targetObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"L'objet {targetObject.name} ne possède pas de composant Animator !");
                yield break;
            }

            if (targetObject.name == "Bag")
            {
                animator.runtimeAnimatorController = bagAnimatorController;
                Debug.Log($"Contrôleur d'animation {bagAnimatorController.name} assigné à {targetObject.name}.");
            }
            else if (targetObject.name == "Bellows")
            {
                animator.runtimeAnimatorController = bellowsAnimatorController;
                Debug.Log($"Contrôleur d'animation {bellowsAnimatorController.name} assigné à {targetObject.name}.");
            }
            else if (targetObject.name == "AlarmClock")
            {
                animator.runtimeAnimatorController = almarnClockAnimatorController;
                Debug.Log($"Contrôleur d'animation {almarnClockAnimatorController.name} assigné à {targetObject.name}.");
            }
            yield return new WaitForSeconds(effectDuration);
            animator.runtimeAnimatorController = null;
            Debug.Log($"Effet terminé. Contrôleur d'animation réinitialisé pour {targetObject.name}.");
        }

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
    }
}