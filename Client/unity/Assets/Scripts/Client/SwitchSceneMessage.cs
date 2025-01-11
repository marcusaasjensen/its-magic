namespace Client
{
    public class SwitchSceneMessage : Message
    {
        public string sceneName;
        
        public SwitchSceneMessage()
        {
            clientId = "TopView";
            recipientId = "Android";
            type = "switchObject";
        }
        
    }
}