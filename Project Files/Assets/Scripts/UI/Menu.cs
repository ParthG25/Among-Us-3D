using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
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
