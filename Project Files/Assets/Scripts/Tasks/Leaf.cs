using UnityEngine;
using Random = UnityEngine.Random;

public class Leaf : MonoBehaviour
{
    public Lever lever;
    private RectTransform rectTransform;
    private float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        moveSpeed = Random.Range(7, 10);
        rectTransform.Rotate(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
    }

    private void Update()
    {
        if(lever.leverHeld)
            rectTransform.position = new Vector3(rectTransform.position.x, 
                rectTransform.position.y-moveSpeed, rectTransform.position.z);
    }
}
