using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Slider mouseSensitivitySlider;
    public TMP_Text mouseSensitivitySliderText;
    public Slider masterVolumeSlider;
    public TMP_Text masterVolumeSliderText;
    public TMP_InputField enteredName;

    // Start is called before the first frame update
    private void Start()
    {
        mouseSensitivitySlider.value = PlayerValues.Instance.mouseSensitivity;
        mouseSensitivitySliderText.text = PlayerValues.Instance.mouseSensitivity.ToString();
        masterVolumeSlider.value = PlayerValues.Instance.masterVolume;
        masterVolumeSliderText.text = (int) (PlayerValues.Instance.masterVolume * 100) + "%";
        enteredName.text = PlayerValues.Instance.playerName;
        
        enteredName.onValueChanged.AddListener((newValue) =>
        {
            PlayerValues.Instance.playerName = newValue;
        });
        mouseSensitivitySlider.onValueChanged.AddListener((newValue) =>
        {
            mouseSensitivitySliderText.text = newValue.ToString();
            PlayerValues.Instance.mouseSensitivity = newValue;
        });
        masterVolumeSlider.onValueChanged.AddListener((newValue) =>
        {
            masterVolumeSliderText.text = (int)(newValue * 100) + "%";
            PlayerValues.Instance.masterVolume = newValue;
        });
    }
}
