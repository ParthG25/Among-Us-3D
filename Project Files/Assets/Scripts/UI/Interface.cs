using UnityEngine;

public class Interface : MonoBehaviour
{
    public string interfaceName;
    public bool open;
    
    public void Open()
    {
        open = true;
        gameObject.SetActive(true);                //Opening the particular panel
    }
    
    public void Close()
    {
        open = false;
        gameObject.SetActive(false);               //Closing the particular panel
    }
}