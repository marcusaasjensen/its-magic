using Client;
using UnityEngine;

namespace Environment
{
    public class Mushroom : Collectible
    {
        protected override void WhenCollected()
        {
            print("Mushroom Collected!");
            var addMushroom = new ItemBagMessage
            {
                clientId = "TopView",
                type = "AddItem",
                recipientId = "Android",
                objectId = "1"
            };
            WebSocketClient.Instance.SendMessageToServer(JsonUtility.ToJson(addMushroom));
        }
    }
}