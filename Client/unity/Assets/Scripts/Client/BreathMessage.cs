namespace Client
{
    public class BreathMessage : Message
    {
        public float windIntensity;
        
        public BreathMessage()
        {
            clientId = "Android";
            recipientId = "TopView";
            type = "Wind";
        }
        
    }
}