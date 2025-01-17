namespace Client
{
    public class ObjectMessage : Message
    {
        public string targetObject;

        public ObjectMessage()
        {
            clientId = "TopView";
            recipientId = "Android";
            type = "Glow";
        }
        
    }
}