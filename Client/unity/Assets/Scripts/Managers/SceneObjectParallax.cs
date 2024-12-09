namespace Managers
{
    using UnityEngine;

    public class SceneObjectParallax : MonoBehaviour
    {
        [SerializeField] private Transform parallaxBackground; // Reference to the background texture GameObject
        [SerializeField] private float parallaxFactor = 1f;    // Speed multiplier for the collectibles' parallax effect
        [SerializeField] private float textureWidth;          // Width of the background texture in world units

        private Vector3 startOffset;

        void Start()
        {
            parallaxBackground = GameObject.Find("BackgroundParallax").transform;
            // Calculate the initial offset of the collectible relative to the background
            startOffset = transform.position - parallaxBackground.position;

            // Automatically calculate texture width if not set
            if (textureWidth <= 0)
            {
                SpriteRenderer backgroundSprite = parallaxBackground.GetComponent<SpriteRenderer>();
                if (backgroundSprite)
                {
                    textureWidth = backgroundSprite.bounds.size.x;
                }
                else
                {
                    Debug.LogWarning("Texture width is not set, and no SpriteRenderer found to calculate.");
                }
            }
        }

        void Update()
        {
            // Move the collectible to match the background's parallax position
            Vector3 backgroundPosition = parallaxBackground.position;
            transform.position = new Vector3(
                (backgroundPosition.x * parallaxFactor + startOffset.x) % textureWidth,
                transform.position.y,
                transform.position.z
            );

            // Wrap collectible position within the texture bounds
            if (transform.position.x < parallaxBackground.position.x - textureWidth / 2f)
            {
                transform.position += Vector3.right * textureWidth;
            }
            else if (transform.position.x > parallaxBackground.position.x + textureWidth / 2f)
            {
                transform.position -= Vector3.right * textureWidth;
            }
        }
    }

}