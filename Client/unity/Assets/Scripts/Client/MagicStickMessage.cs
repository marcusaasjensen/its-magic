namespace Client
{
    public class MagicStickMessage : Message
    {
        public float rotationInDegrees;
        
        public MagicStickMessage()
        {
            clientId = "TopView";
            type = "MagicStick";
        }
        
    }
}