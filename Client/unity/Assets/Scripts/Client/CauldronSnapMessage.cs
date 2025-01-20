namespace Client
{
    public class CauldronSnapMessage : Message
    {
        public bool isSnapped;

        public CauldronSnapMessage()
        {
            recipientId = "SideView";
            type = "CauldronSnap";
            clientId = "TopView";
        }
    }
}