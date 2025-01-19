namespace Client
{
    public class PotionMessage : Message
    {
        public PotionMessage()
        {
            clientId = "SideView";
            type = "Potion";
            recipientId = "TopView";
        }
    }
}