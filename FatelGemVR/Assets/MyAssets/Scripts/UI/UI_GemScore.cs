using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_GemScore : MonoBehaviour
{
    [SerializeField]
    Text labelValue;
    [SerializeField]
    Text labelCombo;

    public void Set(int combo,int value)
    {
        if(combo>=2)
        {
            labelCombo.gameObject.SetActive(true);
            labelCombo.text = "COMBO X" + combo;
        }
        else
        {
            labelCombo.gameObject.SetActive(false);
        }
        labelValue.text = "+" + value.ToString();
    }
}
