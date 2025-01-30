using UnityEngine;

public static class SoundDataManager 
{
    public enum soundType { Test,None };

    [System.Serializable]
    public struct Sound
    {
        public string name;
        public soundType type;
        public AudioClip[] possibleClips;
    }

}
