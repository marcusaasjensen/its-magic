namespace Client
{
    public class ItemBagMessage : Message
    {
        public string objectId; // 1 = mushroom, 2 = acorn, 3 = berry
        
        public ItemBagMessage()
        {
            clientId = "TopView";
            recipientId = "Android";
            type = "switchObject";
        }
        
    }
}