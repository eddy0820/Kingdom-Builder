using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoHolder : MonoBehaviour
{
    [ReadOnly, SerializeField] Sex playerSex;
    public Sex PlayerSex => playerSex;
    [ReadOnly, SerializeField] SkinColors playerSkinColors;
    public SkinColors PlayerSkinColors => playerSkinColors;
    [ReadOnly, SerializeField] int earID;
    public int EarID => earID;
    [ReadOnly, SerializeField] int faceID;
    public int FaceID => faceID;
    [ReadOnly, SerializeField] Color facePaintColor;
    public Color FacePaintColor => facePaintColor;
    [ReadOnly, SerializeField] Color eyeColor;
    public Color EyeColor => eyeColor;
    [ReadOnly, SerializeField] int hairID;
    public int HairID => hairID;
    [ReadOnly, SerializeField] int eyeBrowID;
    public int EyeBrowID => eyeBrowID;
    [ReadOnly, SerializeField] int facialHairID;
    public int FacialHairID => facialHairID;
    [ReadOnly, SerializeField] Color hairColor;
    public Color HairColor => hairColor;
    [ReadOnly, SerializeField] Color underwearColor;
    public Color UnderwearColor => underwearColor;
  

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetSex(Sex _sex)
    {
        playerSex = _sex;
    }

    public void SetEar(int _num)
    {
        earID = _num;
    }

    public void SetFace(int _num)
    {
        faceID = _num;
    }

    public void SetFacePaintColor(Color _color)
    {
        facePaintColor = _color;
    }

    public void SetEyeColor(Color _color)
    {
        eyeColor = _color;
    }

    public void SetHair(int _num)
    {
        hairID = _num;
    }

    public void SetEyeBrow(int _num)
    {
        eyeBrowID = _num;
    }

    public void SetFacialHair(int _num)
    {
        facialHairID = _num;
    }

    public void SetHairColor(Color _color)
    {
        hairColor = _color;
    }

    public void SetUnderwearColor(Color _color)
    {
        underwearColor = _color;
    }

    [System.Serializable]
    public class SkinColors
    {
        [ReadOnly, SerializeField] Color skin;
        public Color Skin => skin;
        [ReadOnly, SerializeField] Color stubble;
        public Color Stubble => stubble;
        [ReadOnly, SerializeField] Color scar;
        public Color Scar => scar;

        public void SetSkinColors(Color _skin, Color _stubble, Color _scar)
        {
            skin = _skin;
            stubble = _stubble;
            scar = _scar;
        }

        public void SetScarColor(Color _scar)
        {
            scar = _scar;
        }
    }

    public enum Sex
    {
        Male,
        Female
    }
}
