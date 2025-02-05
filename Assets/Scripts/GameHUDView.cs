using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameHUDView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _colorValueText;


    private ColorInfo _validColorInfo;

    public void SetValidColorInfo(ColorInfo validColorInfo)
    {
        _validColorInfo = validColorInfo;
        _colorValueText.text = validColorInfo.name;
        _colorValueText.faceColor = validColorInfo.color;
    }

}
