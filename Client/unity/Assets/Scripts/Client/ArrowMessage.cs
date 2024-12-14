namespace Client
{
    public class ArrowMessage : Message
    {
        public bool leftOrRight;
        public ArrowMessage()
        {
            clientId = "SideView";
            type = "Arrow";
        }
    }
}