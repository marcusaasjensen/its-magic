namespace Client
{
    public class FireWindMessage : Message
    {
        public float fireIntensity;
        public float windIntensity;
        
        public FireWindMessage()
        {
            clientId = "Android";
            recipientId = "TopView";
            type = "FireWind";
        }
        
    }
}