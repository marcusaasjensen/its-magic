namespace Client
{
    public class LockMessage : Message
    {
        public bool isLocked;
        
        public LockMessage()
        {
            clientId = "SideView";
            recipientId = "TopView";
            type = "Lock";
        }
    }
}