using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [SerializeField]
    Slider musicSlider, sfxSlider;
    [SerializeField]
    TMP_Text musicValue, sfxValue, deleteText;
    [SerializeField]
    GameObject deletePanel, yesButton, noButton, backButton;
    float musicVolume, sfxVolume;



    private void OnEnable()
    {
        musicVolume = GameManagerV2.instance.GetMusicVolume();
        sfxVolume = GameManagerV2.instance.GetSfxVolume(); 
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }
    public void ModifyMusic(float value)
    {
        musicVolume = value;
        musicValue.text = musicVolume.ToString();
        GameManagerV2.instance.SetMusicVolume(musicVolume);
    }

    public void ModifySfx(float value)
    {
        sfxVolume = value;
        sfxValue.text = sfxVolume.ToString();
        GameManagerV2.instance.SetSfxVolume(sfxVolume);
    }

    public void DeleteDatas()
    {
        OpendDeletePanel();
    }

    public void ResetDatas()
    {
        musicVolume = 10f;
        sfxVolume = 10f;
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;
    }

    public void ClosePanel()
    {
        GameManagerV2.instance.Save();
        gameObject.SetActive(false);
    }

    public void ConfirmDeletion()
    {
        GameManagerV2.instance.DeleteSave();
        ResetDatas();
        yesButton.SetActive(false);
        noButton.SetActive(false);
        backButton.SetActive(true);
        deleteText.text = "Save deleted";
    }

    public void BackToOptions()
    {
        deletePanel.SetActive(false);
    }

    private void OpendDeletePanel()
    {
        deletePanel.SetActive(true);
        yesButton.SetActive(true);
        noButton.SetActive(true);
        backButton.SetActive(false);
        deleteText.text = "Are you sure you want to delete your progression and preferences?";
    }

    

}

