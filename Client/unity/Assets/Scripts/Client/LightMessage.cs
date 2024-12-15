namespace Client
{
    public class LightMessage : Message
    {
        public string value;
        
        public LightMessage()
        {
            clientId = "Android";
            recipientId = "";
            type = "Light";
        }
    }
}