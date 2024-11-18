namespace Utils
{
    public class MonoBehaviourSingleton<T> : UnityEngine.MonoBehaviour where T : UnityEngine.MonoBehaviour
    {
        public static T Instance { get; private set; }
        
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}