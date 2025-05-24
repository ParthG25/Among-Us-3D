using Photon.Pun;
using UnityEngine;

public class Vent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PhotonView>().IsMine)
        {
            VentManager.Instance.activeVent = int.Parse(gameObject.name);
            foreach(Transform child in transform)
                VentManager.Instance.ventNetwork = child.name;

            VentManager.Instance.canVent = true;

            if(GameController.Instance.impostersList.IndexOf(CreateAndJoinRooms.Instance.myNumberInRoom) != -1)
                InterfaceManager.Instance.useActive.SetActive(true);
        }

        VentManager.Instance.onVentCount[int.Parse(gameObject.name)]++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PhotonView>().IsMine)
        {
            VentManager.Instance.canVent = false;
            if (!VentManager.Instance.inVent)
            {
                InterfaceManager.Instance.useActive.SetActive(false);
                
                VentManager.Instance.activeVent = -1;
                VentManager.Instance.ventNetwork = "";
            }
        }

        VentManager.Instance.onVentCount[int.Parse(gameObject.name)]--;
    }
}