namespace Client
{
    public class BellowsMessage : Message
    {
        public bool isBlowingInFire;
        
        public BellowsMessage()
        {
            clientId = "TopView";
            recipientId = "Android";
            type = "BlowFire";
        }
    }
}