using Environment;
using Managers;
using UnityEngine;

namespace Client
{
    public class SendTest : MonoBehaviour
    {
        [ContextMenu("Send")]
        public void Send()
        {
            WebSocketClient.Instance.SendMessageToServer("Hello, world!");
        }
        [ContextMenu("Vibrate")]
        public void Vibrate()
        {
            var vibrateObject = new Message
            {
                clientId = "TopView",
                type = "vibrate",
                recipientId = "Android"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(vibrateObject));
        }
        
        [ContextMenu("Light")]
        public void getClock()
        {
            var getClock = new SwitchObjectMessage
            {
                clientId = "TopView",
                type = "switchObject",
                recipientId = "Android",
                objectName = "alarmClock",
                objectScene = "forest"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(getClock));
        }
        
        [ContextMenu("Bellows")]
        public void getBellows()
        {
            var getBellows = new SwitchObjectMessage
            {
                clientId = "TopView",
                type = "switchObject",
                recipientId = "Android",
                objectName = "bellows",
                objectScene = "forest"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(getBellows));
        }
        
        [ContextMenu("BagInForest")]
        public void getBagInForest()
        {
            var getBagInForest = new SwitchObjectMessage
            {
                clientId = "TopView",
                type = "switchObject",
                recipientId = "Android",
                objectName = "bag",
                objectScene = "forest"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(getBagInForest));
        }
        
        [ContextMenu("BagInWorkshop")]
        public void getBagInWorkshop()
        {
            var getBagInWorkshop = new SwitchObjectMessage
            {
                clientId = "TopView",
                type = "switchObject",
                recipientId = "Android",
                objectName = "bag",
                objectScene = "workshop"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(getBagInWorkshop));
        }
        
        [ContextMenu("SwitchToWorkshop")]
        public void switchToWorkshop()
        {
            var switchToWorkshop = new SwitchSceneMessage
            {
                clientId = "TopView",
                type = "Scene",
                recipientId = "Android",
                sceneName = "workshop"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(switchToWorkshop));
            SceneController.Instance.TransitionToScene("HouseTopScene");
        }
        
        [ContextMenu("SwitchToForest")]
        public void switchToForest()
        {
            var switchToForest = new SwitchSceneMessage
            {
                clientId = "TopView",
                type = "Scene",
                recipientId = "Android",
                sceneName = "forest"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(switchToForest));
            SceneController.Instance.TransitionToScene("ForestTopScene");
        }
        
        [ContextMenu("AddMushroom")]
        public void addMushroom()
        {
            var addMushroom = new ItemBagMessage
            {
                clientId = "TopView",
                type = "AddItem",
                recipientId = "Android",
                objectId = "1"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(addMushroom));
        }
        
        [ContextMenu("AddAcorn")]
        public void addAcorn()
        {
            var addAcorn = new ItemBagMessage
            {
                clientId = "TopView",
                type = "AddItem",
                recipientId = "Android",
                objectId = "2"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(addAcorn));
        }
        
        [ContextMenu("AddBerry")]
        public void addBerry()
        {
            var addBerry = new ItemBagMessage
            {
                clientId = "TopView",
                type = "AddItem",
                recipientId = "Android",
                objectId = "3"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(addBerry));
        }
        
        [ContextMenu("AddFirefly")]
        public void addFirefly()
        {
            var addBerry = new ItemBagMessage
            {
                clientId = "TopView",
                type = "AddItem",
                recipientId = "Android",
                objectId = "4"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(addBerry));
        }
        
        [ContextMenu("To Day")]
        public void ToDay()
        {
            var toDay = new LightMessage
            {
                clientId = "TopView",
                type = "Light",
                recipientId = "TopView",
                value = 1.0f
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(toDay));
            toDay.recipientId = "SideView";
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(toDay));
        }
        
        [ContextMenu("To Night")]
        public void ToNight()
        {
            var toDay = new LightMessage
            {
                clientId = "TopView",
                type = "Light",
                recipientId = "TopView",
                value = 0.0f
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(toDay));
            toDay.recipientId = "SideView";
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(toDay));
        }
        
        [ContextMenu("BlowWind")]
        public void BlowWind()
        {
            var blowWind = new BreathMessage
            {
                clientId = "Android",
                type = "Wind",
                recipientId = "TopView",
                windIntensity = 1.0f
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(blowWind));
            blowWind.recipientId = "SideView";
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(blowWind));
        }
        
        [ContextMenu("Wind")]
        public void Wind()
        {
            var blowFire = new BreathMessage
            {
                clientId = "Android",
                type = "Fire",
                recipientId = "TopView",
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(blowFire));
            blowFire.recipientId = "SideView";
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(blowFire));
        }
        
        [ContextMenu("Fire")]
        public void Fire()
        {
            var fire = new FireMessage
            {
                clientId = "Android",
                type = "Fire",
                recipientId = "TopView",
                fireIntensity = 1.0f
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(fire));
            fire.recipientId = "SideView";
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(fire));
        }
        
        [ContextMenu("Explode")]
        public void Explode()
        {
            var potionMessage = new PotionMessage();
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(potionMessage));
        }
    }
}