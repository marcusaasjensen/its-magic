namespace Client
{
    public class LockMessage : Message
    {
        public bool isLocked;
        
        public LockMessage()
        {
            clientId = "TopView";
            recipientId = "SideView";
            type = "Lock";
        }
    }
}