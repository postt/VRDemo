using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
[Serializable]
public struct Point
{
    public int x;
    public int y;
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class GameController : MonoBehaviour
{
    public static GameController Instance
    { get{ return GameObject.FindObjectOfType<GameController>(); }}

    Level level;
    public Level Level
    {
        get
        {
            return level;
        }
    }
    [SerializeField]
    Level[] levels;
    int levelIndex = 0;
    [SerializeField]
    SteamVR_TrackedObject rightTrackedObject;
    SteamVR_Controller.Device rightController;
    [SerializeField]
    SteamVR_TrackedObject leftTrackedObject;
    SteamVR_Controller.Device leftController;
    [SerializeField]
    Transform bombTransform;
    [SerializeField]
    GameObject rayShooter;
    [SerializeField]
    ParticleSystem twinklePartical;
    [SerializeField]
    CoinPartical coinPartical;
    [SerializeField]
    Animator handAnimator;
    [SerializeField]
    GameObject finalBoomPartical;
    [SerializeField]
    Transform headPosition;
    [SerializeField]
    Animator finalBoomLightAnim;

    [SerializeField, Header("Guide")]
    Animator animatorUFO;
    [SerializeField]
    GameObject guideUI;
    [SerializeField]
    GuideController guideController;

    [SerializeField, Header("音效")]
    AudioClip selectingSound;
    [SerializeField]
    AudioClip exchangeSound;
    [SerializeField]
    AudioClip destorySound;
    [SerializeField]
    AudioClip explosionSound;
    [SerializeField]
    AudioClip excellentSound;
    [SerializeField]
    AudioClip heartbeatSound;
    [SerializeField]
    AudioClip coinSound;
    [SerializeField]
    AudioClip finalBoomSound;

    [SerializeField,Header("UI")]
    UI_GemScore scoreUI;
    [SerializeField]
    Image curtain;
    [SerializeField]
    GameObject gameOverUI;
    [SerializeField]
    GameObject gameNextUI;

    GameObject levelOverUI;
    Element[,] elementsMatrix;
    public List<Element> Elements
    {
        get
        {
            List<Element> elements = new List<Element>();
            foreach (Element element in elementsMatrix)
            {
                if (element != null)
                { elements.Add(element); }
            }
            return elements;
        }
    }
    bool showGuide { get { return Level.FirstLevel; } }

    //给UI提供的公共成员
    int currentScore = 0;
    public int CurrentScore
    {
        get
        {
            return currentScore;
        }
    }
    float currentTime = 0;
    public float CurrentTime
    {
        get
        {
            return currentTime;
        }
    }

    int combo = 0;
    Element firstSelectElement;
    Element secondSelectElement;
    Element selectingElement;
    bool releasePlayerControl = false;
    AudioSource AudioSource { get { return GetComponent<AudioSource>(); } }

    IEnumerator Start()
    {
        level = levels[0];
        if(showGuide)
        { yield return Guide(); }
        LoadLevel(levelIndex);
        StartCoroutine(ElementsTranslate());
        StartCoroutine(GameTimeTick());
        StartCoroutine(FullCheck(false));
        StartCoroutine(PlayerInput());
    }

    IEnumerator Guide()
    {
        animatorUFO.Play("UFO_Come");
        yield return new WaitForSeconds(8.5f);
        guideUI.SetActive(true);
        guideController.Next();
        while (true)
        {
            rightController = SteamVR_Controller.Input((int)rightTrackedObject.index);
            if (rightController.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                if (!guideController.Next())
                { break; }
            }
            yield return null;
        }
        guideUI.SetActive(false);
        animatorUFO.Play("UFO_Out");
    }

    void ClearLevel()
    {
        EliminateElements(Elements, false);
        //AudioSource.Stop();
        //currentScore = 0;
        currentTime = 0;
    }

    bool heartbeatFlag = false;
    IEnumerator GameTimeTick()
    {
        while(this.currentTime > 0)
        {
            this.currentTime -= UnityEngine.Time.deltaTime;
            yield return null;
            if(currentTime<=23&&!heartbeatFlag)
            {
                heartbeatFlag = true;
                AudioSource.PlayOneShot(heartbeatSound);
            }
        }
        StartCoroutine(ShowLevelOverUI());
    }

    IEnumerator ShowLevelOverUI()
    {
        if (levelIndex >= levels.Length - 1)
        {
            finalBoomLightAnim.SetTrigger("Boom");
            yield return new WaitForSeconds(0.33f);
            Instantiate(finalBoomPartical, new Vector3(0, 0, 0), finalBoomPartical.transform.rotation);
            AudioSource.PlayOneShot(finalBoomSound);
            GameObject ui = Instantiate(gameOverUI, headPosition.position, headPosition.rotation) as GameObject;
            ui.transform.Translate(new Vector3(0, 0, 0.4f), Space.Self);
            ui.transform.eulerAngles = new Vector3(ui.transform.eulerAngles.x, ui.transform.eulerAngles.y, 0);
            ui.transform.FindChild("YourScore").GetComponent<Text>().text = "Your score : " + this.CurrentScore;
            ui.transform.FindChild("LevelText").GetComponent<Text>().text = "Level " + (levelIndex + 1);
            levelOverUI = ui;
            StartCoroutine(WaitForPlayAgain());
        }
        else
        {
            if (CurrentScore >= Level.TargetScore)
            {
                GameObject ui = Instantiate(gameNextUI, headPosition.position, headPosition.rotation) as GameObject;
                ui.transform.Translate(new Vector3(0, 0, 0.4f), Space.Self);
                ui.transform.eulerAngles = new Vector3(ui.transform.eulerAngles.x, ui.transform.eulerAngles.y, 0);
                ui.transform.FindChild("YourScore").GetComponent<Text>().text = "Your score : " + this.CurrentScore;
                ui.transform.FindChild("LevelText").GetComponent<Text>().text = "Level " + (levelIndex + 1);
                levelOverUI = ui;
                StartCoroutine(WaitForPlayNext());
            }
            else
            {
                finalBoomLightAnim.SetTrigger("Boom");
                yield return new WaitForSeconds(0.33f);
                Instantiate(finalBoomPartical, new Vector3(0, 0, 0), finalBoomPartical.transform.rotation);
                AudioSource.PlayOneShot(finalBoomSound);
                GameObject ui = Instantiate(gameOverUI, headPosition.position, headPosition.rotation) as GameObject;
                ui.transform.Translate(new Vector3(0, 0, 0.4f), Space.Self);
                ui.transform.eulerAngles = new Vector3(ui.transform.eulerAngles.x, ui.transform.eulerAngles.y, 0);
                ui.transform.FindChild("YourScore").GetComponent<Text>().text = "Your score : " + this.CurrentScore;
                ui.transform.FindChild("LevelText").GetComponent<Text>().text = "Level " + (levelIndex + 1);
                levelOverUI = ui;
                StartCoroutine(WaitForPlayAgain());
            }
        }
        while (curtain.color.a < 1)
        {
            curtain.color = new Color(0, 0, 0, curtain.color.a + 0.01f);
            yield return null;
        }
    }

    IEnumerator WaitForPlayAgain()
    {
        ClearLevel();
        yield return new WaitForSeconds(3f);
        while(true)
        {
            rightController = SteamVR_Controller.Input((int)rightTrackedObject.index);
            if (rightController.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                Application.LoadLevelAsync("Start");
                break;
            }
            yield return null;
        }
    }

    IEnumerator WaitForPlayNext()
    {
        ClearLevel();
        yield return new WaitForSeconds(3f);
        while (true)
        {
            rightController = SteamVR_Controller.Input((int)rightTrackedObject.index);
            if (rightController.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                Destroy(levelOverUI.gameObject);
                StartCoroutine(LoadNextLevel());
                break;
            }
            yield return null;
        }
    }

    IEnumerator LoadNextLevel()
    {
        heartbeatFlag = false;
        if (levelIndex < levels.Length)
        {
            levelIndex += 1;
            LoadLevel(levelIndex);
            StartCoroutine(ElementsTranslate());
            StartCoroutine(GameTimeTick());
            StartCoroutine(FullCheck(false));
            StartCoroutine(PlayerInput());
            while (curtain.color.a > 0)
            {
                curtain.color = new Color(0, 0, 0, curtain.color.a - 0.01f);
                yield return null;
            }
        }
    }

    /// <summary>
    /// 玩家输入
    /// </summary>
    IEnumerator PlayerInput()
    {
        while (true)
        {
            if (releasePlayerControl)
            {
                //print("PlayerInput Enabled");
                rightController = SteamVR_Controller.Input((int)rightTrackedObject.index);
                /*
                //手动画
                if (rightController.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
                { handAnimator.SetTrigger("Catch"); }
                else
                { handAnimator.SetTrigger("Idle"); }
                //
                */
                Ray ray = new Ray(rayShooter.transform.position, rayShooter.transform.forward);
                RaycastHit hit;
                Debug.DrawRay(ray.origin, ray.direction * 2f, Color.yellow);

                if (Physics.Raycast(ray, out hit, 2f))
                {
                    //指向元素时
                    if (hit.collider.gameObject.tag == "Element")
                    {
                        if (hit.collider.gameObject.GetComponent<Element>() != selectingElement)
                        {
                            twinklePartical.Play();
                            twinklePartical.transform.position = hit.transform.position;
                            AudioSource.PlayOneShot(selectingSound,0.2f);
                            selectingElement = hit.collider.gameObject.GetComponent<Element>();
                        }
                    }
                    else
                    {
                        twinklePartical.Stop();
                        selectingElement = null;
                    }
                    //选择元素时
                    if (rightController.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
                    {
                        if (hit.collider.gameObject.tag == "Element")
                        {
                            Element element = hit.collider.gameObject.GetComponent<Element>();
                            if (firstSelectElement == null)
                            {
                                firstSelectElement = element;
                            }
                            else if (secondSelectElement == null && element != firstSelectElement && IsNearBy(element, firstSelectElement))
                            {
                                secondSelectElement = element;
                                selectingElement = null;
                                ExchangeElements(firstSelectElement, secondSelectElement);
                                yield return new WaitForSeconds(0.5f);
                                //运行一次全图可消除检测
                                List<Element> eliminableBalloons = CheckEliminableElements();
                                //如果不能消除，则气球换回
                                if (eliminableBalloons.Count == 0)
                                {
                                    yield return new WaitForSeconds(0.1f);
                                    ExchangeElements(firstSelectElement, secondSelectElement);
                                }
                                else
                                {
                                    yield return FullCheck(true);
                                    firstSelectElement = null;
                                    secondSelectElement = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        firstSelectElement = null;
                        secondSelectElement = null;
                    }
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// 对所有元素的Translate操作
    /// </summary>
    IEnumerator ElementsTranslate()
    {
        while (true)
        {
            foreach (Element element in Elements)
            {
                if (element != selectingElement)
                {
                    element.transform.LookAt(this.transform);
                    element.transform.localEulerAngles = new Vector3(0, element.transform.localEulerAngles.y, element.transform.localEulerAngles.z);
                }
            }
            if (selectingElement != null)
            {
                selectingElement.transform.Rotate(new Vector3(0, 1, 0), Space.Self);
            }
            yield return null;
        }
    }

    /// <summary>
    /// 交换两个元素坐标位置
    /// </summary>
    public void ExchangeElements(Element first, Element second)
    {
        Vector3 tempPos = second.transform.localPosition;
        Point tempPoint = second.Coord;

        elementsMatrix[first.Coord.x, first.Coord.y] = second;
        second.SetCoord(first.Coord.x, first.Coord.y, Point2LocalPosition(first.Coord));

        elementsMatrix[tempPoint.x, tempPoint.y] = first;
        first.SetCoord(tempPoint.x, tempPoint.y, Point2LocalPosition(tempPoint));

        AudioSource.PlayOneShot(exchangeSound);
    }

    /// <summary>
    /// 检测两个元素是否为贴近的两个元素
    /// </summary>
    bool IsNearBy(Element first, Element second)
    {
        List<Point> avaliablePoints = new List<Point>();
        if (first.Coord.x != Level.X - 1 && first.Coord.x != 0)
        {
            avaliablePoints.Add(new Point(first.Coord.x + 1, first.Coord.y));
            avaliablePoints.Add(new Point(first.Coord.x - 1, first.Coord.y));
        }
        else if(first.Coord.x == Level.X - 1)
        {
            avaliablePoints.Add(new Point(0, first.Coord.y));
            avaliablePoints.Add(new Point(first.Coord.x - 1, first.Coord.y));
        }
        else if(first.Coord.x == 0)
        {
            avaliablePoints.Add(new Point(first.Coord.x + 1, first.Coord.y));
            avaliablePoints.Add(new Point(Level.X - 1, first.Coord.y));
        }
        avaliablePoints.Add(new Point(first.Coord.x, first.Coord.y + 1));
        avaliablePoints.Add(new Point(first.Coord.x, first.Coord.y - 1));
        bool isNearBy = false;
        foreach (Point point in avaliablePoints)
        {
            if (second.Coord.x == point.x && second.Coord.y == point.y)
            { isNearBy = true; break; }
        }
        return isNearBy;
    }

    /// <summary>
    /// 完整的一轮检测逻辑
    /// </summary>
    IEnumerator FullCheck(bool scoreAvaliable)
    {
        //print("COMBO:"+combo);
        releasePlayerControl = false;
        List<Element> eliminableElements = CheckEliminableElements();
        bool superEleminateFlag = false;
        Point randomPoint = new Point(0, 0);
        if (eliminableElements.Count > 0)
        {
            combo += 1;
            //生成特殊宝石
            if (eliminableElements.Count >= 5 && scoreAvaliable)
            {
                randomPoint = eliminableElements[UnityEngine.Random.Range(0, eliminableElements.Count)].Coord;
                superEleminateFlag = true;
            }
            EliminateElements(eliminableElements, scoreAvaliable);
            if (superEleminateFlag)
            {
                Element element = CreateElement(Level.ElementPrefabs[UnityEngine.Random.Range(0, Level.ElementPrefabs.Length)], randomPoint);
                element.Special = Special.Explosion;
            }
            yield return new WaitForSeconds(0.1f);
            RearrangeMatrix();
            RefillMatrix();
            if (scoreAvaliable)
            { yield return new WaitForSeconds(1f); }
            yield return FullCheck(scoreAvaliable);
        }
        else
        {
            releasePlayerControl = true;
            combo = 0;
        }
    }

    /// <summary>
    /// 载入关卡
    /// </summary>
    void LoadLevel(int index)
    {
        this.level = levels[index];
        this.currentTime = Level.Time;
        foreach (Transform child in this.transform.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }
        elementsMatrix = new Element[Level.X, Level.Y];
        for (int i = 0; i < Level.X; i++)
        {
            for (int j = 0; j < Level.Y; j++)
            {
                CreateElement(Level.ElementPrefabs[UnityEngine.Random.Range(0, Level.ElementPrefabs.Length)], new Point(i, j));
            }
        }
    }

    /// <summary>
    /// 等待所有元素进入静止状态
    /// </summary>
    IEnumerator WaitForAllElements()
    {
        bool flag = true;
        while (flag)
        {
            bool allIdleState = true;
            foreach (Element element in Elements)
            {
                if (!element.IdelState)
                { allIdleState = false; break; }
            }
            if (allIdleState)
            { flag = false; }
            yield return null;
        }
    }

    /// <summary>
    /// 填满矩阵
    /// </summary>
    void RefillMatrix()
    {
        for (int i = 0; i < elementsMatrix.GetLength(1); i++)
        {
            for (int j = 0; j < elementsMatrix.GetLength(0); j++)
            {
                if (elementsMatrix[j, i] == null)
                {
                    Element element = CreateElement(Level.ElementPrefabs[UnityEngine.Random.Range(0, Level.ElementPrefabs.Length)], new Point(j, i));
                    element.gameObject.transform.localPosition = Point2LocalPosition(new Point(j, i + Level.Y));
                }
            }
        }
    }

    /// <summary>
    /// 重新排列元素矩阵
    /// </summary>
    void RearrangeMatrix()
    {
        for (int i = 1; i < elementsMatrix.GetLength(1); i++)
        {
            for (int j = 0; j < elementsMatrix.GetLength(0); j++)
            {
                if (elementsMatrix[j, i] != null)
                {
                    Element tempElement = elementsMatrix[j, i];
                    while (ElementFallDown(tempElement)) { }
                }
            }
        }
    }

    /// <summary>
    /// 元素下落
    /// </summary>
    /// <param name="element"></param>
    /// <returns>如果可以下落返回True</returns>
    bool ElementFallDown(Element element)
    {
        if (element.Coord.y > 0)
        {
            if (elementsMatrix[element.Coord.x, element.Coord.y - 1] == null)
            {
                elementsMatrix[element.Coord.x, element.Coord.y] = null;
                elementsMatrix[element.Coord.x, element.Coord.y - 1] = element;
                element.SetCoord(element.Coord.x, element.Coord.y - 1, Point2LocalPosition(new Point(element.Coord.x, element.Coord.y - 1)));
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 将Point坐标转换到实际的本地位置
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    Vector3 Point2LocalPosition(Point point)
    {
        float angle = 360 / elementsMatrix.GetLength(0);

        return new Vector3(
                 Level.Radius * Mathf.Cos(((point.x) * angle / 180) * Mathf.PI),
                 point.y * Level.ElementsMarginY,
                 Level.Radius * Mathf.Sin(((point.x) * angle / 180) * Mathf.PI)
                 );
    }

    /// <summary>
    /// 创建一个元素
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="coord"></param>
    /// <returns></returns>
    Element CreateElement(Element prefab, Point coord)
    {
        //如果位置上已经存在气球则不能生成气球，返回null
        if (elementsMatrix[coord.x, coord.y] != null)
        {
            print("重叠的生成位置！ (" + coord.x + "," + coord.y + ")");
            return null;
        }
        //生成气球实例
        GameObject gameObjectInstance = Instantiate(prefab.gameObject, Vector3.zero, prefab.transform.rotation) as GameObject;
        gameObjectInstance.transform.SetParent(this.transform);
        gameObjectInstance.transform.localPosition = Point2LocalPosition(coord);

        Element elementInstance = gameObjectInstance.GetComponent<Element>();
        elementsMatrix[coord.x, coord.y] = elementInstance;
        elementInstance.SetCoord(coord.x, coord.y, Point2LocalPosition(coord));
        elementInstance.StartCoroutine("Grow");

        return elementInstance;
    }

    /// <summary>
    /// 全图检查可以消除的元素
    /// </summary>
    /// <returns>可以消除的元素列表</returns>
    List<Element> CheckEliminableElements()
    {
        List<Element> eliminableElements = new List<Element>();
        List<Element> tempElements = new List<Element>();
        List<Element> tempEliminableElements = new List<Element>();
        //--------------------横向检测------------------------
        for (int i = 0; i < elementsMatrix.GetLength(1); i++)
        {
            tempElements.Clear();
            //把单独一行装入临时列表
            for (int j = 0; j < elementsMatrix.GetLength(0); j++)
            {
                tempElements.Add(elementsMatrix[j, i]);
            }
            //额外加两个
            for (int j = 0; j < 3; j++)
            {
                tempElements.Add(elementsMatrix[j, i]);
            }
            //同色计数
            int sameColorCounter;
            //每一行从最左第一个到最右倒数第三个都要检测
            for (int j = 0; j < tempElements.Count - 2; j++)
            {
                //重置同色计数为1
                sameColorCounter = 1;
                //添加检测的起始元素到列表
                tempEliminableElements.Add(tempElements[j]);
                //开始逐个检测其后方的元素
                for (int k = j + 1; k < tempElements.Count; k++)
                {
                    //如果同色就加入列表
                    if (tempElements[j].Type == tempElements[k].Type)
                    {
                        //添加到列表
                        tempEliminableElements.Add(tempElements[k]);
                        //同色计数增加1
                        sameColorCounter += 1;
                    }
                    else//一旦出现不同色时
                    {
                        //如果同色计数不足3
                        if (sameColorCounter < 3)
                        {
                            //则丢弃列表的所有元素
                            tempEliminableElements.Clear();
                        }
                        break;//并停止循环
                    }
                }
                //一个检测完之后，把临时列表的元素加到可消除列表中
                foreach (Element element in tempEliminableElements)
                { eliminableElements.Add(element); }
            }
        }
        //--------------------纵向检测------------------------
        for (int i = 0; i < elementsMatrix.GetLength(0); i++)
        {
            tempElements.Clear();
            //把单独一行装入临时列表
            for (int j = 0; j < elementsMatrix.GetLength(1); j++)
            {
                tempElements.Add(elementsMatrix[i, j]);
            }
            //同色计数
            int sameColorCounter;
            //每一行从最左第一个到最右倒数第三个都要检测
            for (int j = 0; j < tempElements.Count - 2; j++)
            {
                //重置同色计数为1
                sameColorCounter = 1;
                //添加检测的起始元素到列表
                tempEliminableElements.Add(tempElements[j]);
                //开始逐个检测其后方的元素
                for (int k = j + 1; k < tempElements.Count; k++)
                {
                    //如果同色就加入列表
                    if (tempElements[j].Type == tempElements[k].Type)
                    {
                        //添加到列表
                        tempEliminableElements.Add(tempElements[k]);
                        //同色计数增加1
                        sameColorCounter += 1;
                    }
                    else//一旦出现不同色时
                    {
                        //如果同色计数不足3
                        if (sameColorCounter < 3)
                        {
                            //则丢弃列表的所有元素
                            tempEliminableElements.Clear();
                        }
                        break;//并停止循环
                    }
                }
                //一个检测完之后，把临时列表的元素加到可消除列表中
                foreach (Element balloon in tempEliminableElements)
                { eliminableElements.Add(balloon); }
            }
        }
        return eliminableElements.Distinct<Element>().ToList(); ;
    }

    public void PlayCoinSound()
    {
        AudioSource.PlayOneShot(coinSound, 0.33f);
    }

    /// <summary>
    /// 销毁元素
    /// </summary>
    /// <param name="elements"></param>
    void EliminateElements(List<Element> elements,bool scoreAvaliable)
    {
        bool haveSpecial = false;
        int finalValue = 0;
        foreach (Element element in elements)
        {
            if (element == null) { continue; }
            elementsMatrix[element.Coord.x, element.Coord.y] = null;
            if (scoreAvaliable)
            {
                finalValue = (int)((float)element.Value * ((1f + (combo - 1f) / 10f)));
                currentScore += finalValue;
                GameObject ui = Instantiate(scoreUI.gameObject, element.transform.position, element.transform.rotation) as GameObject;
                ui.GetComponent<UI_GemScore>().Set(combo, finalValue);
                ui.transform.Rotate(new Vector3(0, 1, 0), 180);
                ui.transform.Translate(new Vector3(0, 0, -0.2f), Space.Self);
            }
            //特殊宝石的额外消除
            if (element.Special != Special.None)
            {
                haveSpecial = true;
                List<Element> extraEliminateElements = new List<Element>();
                switch (element.Special)
                {
                    case Special.Explosion:
                        foreach (Element elem in Elements)
                        {
                            if (Mathf.Abs(elem.Coord.x - element.Coord.x) <= 1 && Mathf.Abs(elem.Coord.y - element.Coord.y) <= 1 && elem != element)
                            {
                                extraEliminateElements.Add(elem);
                                EliminateElements(extraEliminateElements, scoreAvaliable);
                            }
                        }
                        break;
                }
            }
            //制造金币特效
            if (scoreAvaliable)
            {
                int count = 1;
                for (int i = 0; i < count; i++)
                {
                    GameObject coin = Instantiate(
                        coinPartical.gameObject,
                        new Vector3(
                            UnityEngine.Random.Range(element.transform.position.x - 0.15f, element.transform.position.x + 0.15f),
                            UnityEngine.Random.Range(element.transform.position.y - 0.25f, element.transform.position.y + 0.25f),
                            UnityEngine.Random.Range(element.transform.position.z - 0.15f, element.transform.position.z + 0.15f)
                            ),
                        coinPartical.transform.rotation) as GameObject;
                    coin.GetComponent<CoinPartical>().TargetTransform = bombTransform;
                }
            }
            //销毁
            element.StartCoroutine("DestorySelf");
        }
        if (haveSpecial)
        { AudioSource.PlayOneShot(explosionSound, 1f); }
        else
        { AudioSource.PlayOneShot(destorySound, 0.8f); }
    }
}
