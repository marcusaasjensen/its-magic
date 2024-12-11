namespace Client
{
    [System.Serializable]
    public class FallingObjectMessage : Message
    {
        public string name;
        public int id;
        public float x;
        public float y;
        
        public FallingObjectMessage()
        {
            clientId = "TopView";
            type = "FallingObject";
        }
    }
}