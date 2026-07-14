using UnityEngine;

public class OpacityByPlayerY : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] float minOpacity;
    [Tooltip("Y offset needed to start reducing opacity")]
    [SerializeField] float startYOffset;
    [Tooltip("Ending Y offset where the minimum opacity is applied")]
    [SerializeField] float endYOffset;

    [Space]
    [SerializeField] SpriteRenderer[] spriteRenderers;

    float currOpacity;

    private void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
        }

        float diff = playerTransform.position.y - transform.position.y;
        if (diff >= startYOffset)
        {
            float opacity = Mathf.Lerp(1,minOpacity, Mathf.Clamp01(Mathf.InverseLerp(startYOffset, endYOffset, diff)));
            if (opacity != currOpacity)
            {
                currOpacity = opacity;

                foreach (SpriteRenderer renderer in spriteRenderers)
                {
                    renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, opacity);
                }
            }
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }
}
