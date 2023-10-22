using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }
    }

    // SoundManager

    public SoundAudioClip[] soundAudioClipArray;

    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }

    // ItemWorld

    public Transform pfItemWorld;

    public Transform pfItemWorldBlueKey;

    public ItemSprite[] itemSpriteArray;

    [System.Serializable]
    public class ItemSprite
    {
        public Item.ItemType itemType;
        public Sprite sprite;
    }

    // Rock

    public Transform pfRock;
}
