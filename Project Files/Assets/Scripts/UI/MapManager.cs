using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    public RectTransform playerIcon, miniMap, fullMiniMap;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
