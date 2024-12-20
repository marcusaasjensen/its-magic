namespace Client
{
    public class SwitchObjectMessage : Message
    {
        public string objectName;
        public string objectScene;
        
        public SwitchObjectMessage()
        {
            clientId = "TopView";
            recipientId = "Android";
            type = "switchObject";
        }
        
    }
}