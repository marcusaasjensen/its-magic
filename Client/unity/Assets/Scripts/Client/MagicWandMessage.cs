namespace Client
{
    public class MagicWandMessage : Message
    {
        public float rotationInDegrees;
        public float distanceToCenter;
        
        public MagicWandMessage()
        {
            clientId = "TopView";
            recipientId = "SideView";
            type = "MagicWand";
        }
        
    }
}