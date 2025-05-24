using UnityEngine;

public class Customize : MonoBehaviour
{
    public GameObject[] skins; //stores te three skin types
    public GameObject[] skinBorders; //stores the border for the three skin types
    public GameObject[] bodySkinSamples; //stores all the samples of the body skins
    public GameObject[] headSkinSamples; //stores all the samples of the head skins
    public GameObject[] faceSkinSamples; //stores all the samples of the face skins

    public int bodySkinIndex; //stores the index of the currently active body skin
    public int headSkinIndex; //stores the index of the currently active head skin
    public int faceSkinIndex; //stores the index of the currently active face skin

    private void Start()
    {
        //assigning saved values to the skin indices
        bodySkinIndex = PlayerValues.Instance.bodySkin;
        headSkinIndex = PlayerValues.Instance.headSkin;
        faceSkinIndex = PlayerValues.Instance.faceSkin;

        //activating the currently set body skin
        for (var i = 0; i < bodySkinSamples.Length; i++)
            if (i == bodySkinIndex)
                bodySkinSamples[i].SetActive(true);
            else
                bodySkinSamples[i].SetActive(false);

        //activating the currently set head skin
        for (var i = 0; i < headSkinSamples.Length; i++)
            if (i == headSkinIndex)
                headSkinSamples[i].SetActive(true);
            else
                headSkinSamples[i].SetActive(false);

        //activating the currently set face skin
        for (var i = 0; i < faceSkinSamples.Length; i++)
            if (i == faceSkinIndex)
                faceSkinSamples[i].SetActive(true);
            else
                faceSkinSamples[i].SetActive(false);
    }

    //To open menu for particular skin type
    public void ActivateMenu(int index)
    {
        for (var i = 0; i < 3; i++)
            if (i == index)
            {
                skins[i].SetActive(true);
                skinBorders[i].SetActive(true);
            }
            else
            {
                skins[i].SetActive(false);
                skinBorders[i].SetActive(false);
            }
    }

    //changing the body skin
    public void SetBodySkin(int index)
    {
        PlayerValues.Instance.bodySkin = index;
        bodySkinIndex = index;

        for (var i = 0; i < bodySkinSamples.Length; i++)
            if (i == bodySkinIndex)
                bodySkinSamples[i].SetActive(true);
            else
                bodySkinSamples[i].SetActive(false);
    }

    //changing the head skin
    public void SetHeadSkin(int index)
    {
        PlayerValues.Instance.headSkin = index;
        headSkinIndex = index;

        for (var i = 0; i < headSkinSamples.Length; i++)
            if (i == headSkinIndex)
                headSkinSamples[i].SetActive(true);
            else
                headSkinSamples[i].SetActive(false);
    }

    //changing the face skin
    public void SetFaceSkin(int index)
    {
        PlayerValues.Instance.faceSkin = index;
        faceSkinIndex = index;

        for (var i = 0; i < faceSkinSamples.Length; i++)
            if (i == faceSkinIndex)
                faceSkinSamples[i].SetActive(true);
            else
                faceSkinSamples[i].SetActive(false);
    }
}