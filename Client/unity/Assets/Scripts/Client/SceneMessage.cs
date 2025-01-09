namespace Client
{
    public class SceneMessage : Message
    {
        public string sceneName;
        public SceneMessage()
        {
            clientId = "SideView";
            recipientId = "TopView";
            type = "Scene";
        }
    }
}