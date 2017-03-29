using UnityEngine;
using System.Collections;

public enum ElementType
{
    Ruby, Sapphire, Emerald, Topaz, Amethyst, Durian
}

public enum Special
{
    None, Explosion, 
}

public class Element : MonoBehaviour
{
    [SerializeField]
    Point coord;
    public Point Coord { get { return coord; } }
    [SerializeField]
    int value;
    public int Value { get { return value; } }
    [SerializeField]
    ElementType type;
    public ElementType Type { get { return type; } }
    [SerializeField]
    Special special;
    public Special Special
    {
        get { return special; }
        set
        {
            special = value;
            switch(value)
            {
                case Special.None:
                    foreach(Transform child in this.transform.FindChild("Special").GetComponentInChildren<Transform>())
                    { child.gameObject.SetActive(false); }
                    break;
                case Special.Explosion:
                    this.transform.FindChild("Special").FindChild("Explosion").gameObject.SetActive(true);
                    break;
            }
        }
    }
    [SerializeField]
    GameObject specialGroupPrefab;
    [SerializeField]
    GameObject[] destoryPartical;

    bool idelState = true;
    public bool IdelState { get { return idelState; } }
    Vector3 targetPos;

    void Awake()
    {
        //GetComponentInChildren<Renderer>().sortingLayerName = "Gem";
        //GetComponentInChildren<Renderer>().sortingOrder = 1;
        GameObject specialGroup = Instantiate(specialGroupPrefab) as GameObject;
        specialGroup.transform.SetParent(this.transform);
        specialGroup.transform.localPosition = Vector3.zero;
        specialGroup.gameObject.name = "Special";
        this.Special = special;
    }

    void Start()
    {
        StartCoroutine("TransformLerping");
    }

    /// <summary>
    /// 元素生长
    /// </summary>
    /// <returns></returns>
    public IEnumerator Grow()
    {
        idelState = false;
        float speed = 7.5f;
        this.transform.localScale = new Vector3(0, 0, 0);
        while (this.transform.localScale.x < 1)
        {
            float lerping = Mathf.Lerp(this.transform.localScale.x, 1, Time.deltaTime * speed);
            this.transform.localScale = new Vector3(lerping, lerping, lerping);
            if (this.transform.localScale.x > 0.99f)
            {
                idelState = true;
                break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 设置坐标
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    public void SetCoord(int x, int y, Vector3 targetPos)
    {
        coord = new Point(x, y);
        this.targetPos = targetPos;
    }

    /// <summary>
    /// 向目标位置移动
    /// </summary>
    IEnumerator TransformLerping()
    {
        float speed = 5f;
        while (true)
        {
            Vector3 lerping = new Vector3(
                Mathf.Lerp(this.transform.localPosition.x, targetPos.x, speed * Time.deltaTime),
                Mathf.Lerp(this.transform.localPosition.y, targetPos.y, speed * Time.deltaTime),
                Mathf.Lerp(this.transform.localPosition.z, targetPos.z, speed * Time.deltaTime)
                );
            this.transform.localPosition = lerping;

            if (Vector3.Distance(this.transform.localPosition, targetPos) < 0.01f)
            { idelState = true; }
            else
            { idelState = false; }

            yield return null;
        }
    }

    /// <summary>
    /// 销毁自身
    /// </summary>
    public IEnumerator DestorySelf()
    {
        bool boomFlag = false;
        this.GetComponent<Collider>().enabled = false;
        while (this.transform.localScale.x < 1.25f)
        {
            float newScale = this.transform.localScale.x;
            newScale += 0.025f;
            this.transform.localScale = new Vector3(newScale, newScale, newScale);
            //粒子提早释放
            if (!boomFlag && this.transform.localScale.x > 1.25f - 0.05f)
            {
                Instantiate(destoryPartical[(int)this.special], this.transform.position, this.transform.rotation);
                boomFlag = true;
            }
            yield return null;
        }
        //销毁
        Destroy(this.gameObject);
    }
}
