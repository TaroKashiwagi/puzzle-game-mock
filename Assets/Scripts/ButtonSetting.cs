using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class ButtonSetting : MonoBehaviour
{
    [SerializeField] public float rotation_speed = default;
    [SerializeField] public int count = 0;
    public void RotateButton()
    {
        // transform.DORotate(new Vector3(0f, 360f, 0f), rotation_speed, RotateMode.FastBeyond360)
            // .SetEase(Ease.Linear);
        // transform.DOMove(new Vector3(0,0,0),1f);
    }
    public void OneClick()
    {
        // GetComponent<Button>().interactable = false;
    }
    public void CountButton()
    {
        // count++;
        // Debug.Log(count);
    }
}
