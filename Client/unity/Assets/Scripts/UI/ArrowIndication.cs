using Client;
using UnityEngine;

namespace UI
{
    public class ArrowIndication : MonoBehaviour
    {
        [SerializeField] private GameObject leftArrow;
        [SerializeField] private GameObject rightArrow;
        
        public void OnArrowMessage(string message)
        {
            var arrowMessage = JsonUtility.FromJson<ArrowMessage>(message);
            if(arrowMessage is not { type: "Arrow" }) return;

            print(message);
            SetArrow(arrowMessage.leftOrRight);
        }

        private void SetArrow(bool isRight)
        {
            if (isRight)
            {
                print(rightArrow.activeSelf);
                rightArrow.SetActive(!rightArrow.activeSelf);
            }
            else
            {
                leftArrow.SetActive(!leftArrow.activeSelf);
            }
        }

        public void Hide()
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
        }
    }
}