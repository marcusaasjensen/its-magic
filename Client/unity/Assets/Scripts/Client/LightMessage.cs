namespace Client
{
    public class LightMessage : Message
    {
        public float value;
        
        public LightMessage()
        {
            clientId = "Android";
            recipientId = "";
            type = "Light";
        }
    }
}