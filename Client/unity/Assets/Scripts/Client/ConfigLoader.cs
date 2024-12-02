using UnityEngine;

namespace Client
{
    [System.Serializable]
    public class ServerConfig
    {
        public string serverIp;
        public string serverPort;
    }

    public static class ConfigLoader
    {
        public static ServerConfig LoadConfig()
        {
            var configText = Resources.Load<TextAsset>("config");
            return JsonUtility.FromJson<ServerConfig>(configText.text);
        }
    }
}