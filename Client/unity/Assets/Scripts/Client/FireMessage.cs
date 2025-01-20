namespace Client
{
    public class FireMessage : Message
    {
        public float fireIntensity;
        
        public FireMessage()
        {
            clientId = "Android";
            recipientId = "TopView";
            type = "Fire";
        }
        
    }
}