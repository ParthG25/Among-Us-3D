using UnityEngine;

public class SelfDestroyDeadBody : MonoBehaviour
{
    //this class is created for removing the dead bodies when required
    private void Update()
    {
        if (InterfaceManager.Instance.justReported)
        {
            Destroy(gameObject);
        }
    }
}
