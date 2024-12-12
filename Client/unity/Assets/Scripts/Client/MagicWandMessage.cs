namespace Client
{
    public class MagicWandMessage : Message
    {
        public float rotationInDegrees;
        
        public MagicWandMessage()
        {
            clientId = "TopView";
            type = "MagicWand";
        }
        
    }
}