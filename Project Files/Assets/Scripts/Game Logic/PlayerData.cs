[System.Serializable]
public class PlayerData
{
    public bool isUnlocked;
    public string playerName;
    public float mouseSensitivity ;
    public float masterVolume;
    public int bodySkin;
    public int headSkin;
    public int faceSkin;

    public PlayerData(PlayerValues playerValues)
    {
        playerName = playerValues.playerName;
        mouseSensitivity = playerValues.mouseSensitivity;
        masterVolume = playerValues.masterVolume;
        bodySkin = playerValues.bodySkin;
        headSkin = playerValues.headSkin;
        faceSkin = playerValues.faceSkin;
        isUnlocked = playerValues.isUnlocked;
    }
}
