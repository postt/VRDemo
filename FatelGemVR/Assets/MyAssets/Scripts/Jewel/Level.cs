using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour 
{
    [SerializeField,Header("首关")]
    bool firstLevel = false;
    [SerializeField,Header("时间")]
    float time;
    [SerializeField, Header("目标分数")]
    uint targetScore;
    [SerializeField, Header("可生成的元素")]
    Element[] elementPrefabs;
    [SerializeField, Header("行列数")]
    int x;
    [SerializeField]
    int y;
    [SerializeField, Header("每行之间间距")]
    float elementsMarginY = 1;
    [SerializeField, Header("半径")]
    float radius;

    public Element[] ElementPrefabs
    {
        get
        {
            return elementPrefabs;
        }
    }
    public uint TargetScore
    {
        get
        {
            return targetScore;
        }
    }
    public int X
    {
        get
        {
            return x;
        }
    }
    public int Y
    {
        get
        {
            return y;
        }
    }
    public float Radius
    {
        get
        {
            return radius;
        }
    }
    public float ElementsMarginY
    {
        get
        {
            return elementsMarginY;
        }
    }
    public float Time
    {
        get
        {
            return time;
        }
    }

    public bool FirstLevel
    {
        get
        {
            return firstLevel;
        }
    }
}
