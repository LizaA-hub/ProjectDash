using UnityEngine;

public static class SoundDataManager 
{
    public enum soundType { Main,UI_Click, UI_Slider,None };

    [System.Serializable]
    public struct Sound
    {
        public string name;
        public soundType type;
        public AudioClip[] possibleClips;
    }

}
