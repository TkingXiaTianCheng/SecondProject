using UnityEngine;
using System.Collections;
/// <summary>
/// 角色控制
/// </summary>
public class PlayerController : MonoBehaviour
{
    //声明Transform的引用变量
    private Transform m_Transform;
    //定义地图控制的脚本引用变量
    private MapManager m_MapManager;
    //声明变量获取摄像机的脚本引用
    private CameraFollow m_CameraFollow;
    //声明变量获取UIManager脚本引用
    private UIManager m_UIManager;

    //定义整数类型变量来存储游戏中获得的宝石的数量
    private int gemCount = 0;
    //定义整数类型变量来存储游戏中移动获得的分数
    private int socreCount = 0;

    //声明角色在地图中的位置
    public int pz = 3;
    private int px = 3;

    //声明Bool类型的变量，用来控制游戏是否结束
    private bool life = true;

    //定义两个不同颜色瓷砖上留下的颜色痕迹
    private Color colorOne = new Color(122 / 255f, 85 / 255f, 179 / 255f);
    private Color colorTwo = new Color(126 / 255f, 93 / 255f, 183 / 255f);
    void Start()
    {
        //宝石数量初始化赋值
        //从注册表中取出gem的值，第二个参数为0，是因为开始的时候注册表中是没有gem的值，所以给一个初始值0
        gemCount = PlayerPrefs.GetInt("gem", 0);
        //获取游戏玩家角色身上的Transform组件
        m_Transform = GetComponent<Transform>();
        //获取地图控制器的脚本,先查找到游戏场景中的MapManager物体
        //用于使用其中的mapList集合中的数据
        m_MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        //查找获取摄像机的脚本
        m_CameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        //查找获取UIManager的脚本
        m_UIManager = GameObject.Find("UI Root").GetComponent<UIManager>();
    }

    void Update()
    {     
        if (life)
        {
            PlayerControl();
        }
    }

    public void StartGame()
    {
        //初始化角色的位置
        SetPlayerPos();
        //摄像机开始跟随
        m_CameraFollow.startFollow = true;
        //执行方法地面开始塌陷
        m_MapManager.StartTileDown();
    }
    /// <summary>
    /// 角色的移动控制，左右
    /// </summary>
    private void PlayerControl()
    {

        //向左侧移动
        if (Input.GetKeyDown(KeyCode.A))
        {
            //控制角色的边缘
            if (px != 0)
            {
                pz++;
                AddScoreCount();
            }
            if (pz % 2 == 1 && px != 0)
            {
                px--;
            }
            SetPlayerPos();
            CalculatePosition();
        }
        //向右侧移动
        if (Input.GetKeyDown(KeyCode.D))
        {
            //控制角色的边缘
            if (px != 4 || pz % 2 != 1)
            {
                pz++;
                AddScoreCount();
            }
            if (pz % 2 == 0 && px != 4)
            {
                px++;
            }
            SetPlayerPos();
            CalculatePosition();

        }
    }
    /// <summary>
    /// 设置角色的位置,设置蜗牛痕迹
    /// </summary>
    private void SetPlayerPos()
    {
        //获取玩家在地图上的位置，这里的地图是存储在mapList集合中的
        //通过pz,px连个角标来确定玩家的位置
        Transform playPos = m_MapManager.mapList[pz][px].GetComponent<Transform>();
        //声明一个MeshRenderer的区域性引用变量
        MeshRenderer normal = null;
        //因为每个方块的边长是0.254f，所以增加一个0.254F/2的偏移量
        m_Transform.position = playPos.position + new Vector3(0, 0.254f / 2, 0);
        //旋转方向11就是集合中砖块的旋转
        m_Transform.rotation = playPos.rotation;
        //如果当前玩家所在位置的地面是瓷砖
        if (playPos.tag == "Tile")
        {
            //找到当前玩家所在瓷砖的顶部模型，此模型是相对瓷砖的子物体
            normal = playPos.FindChild("normal_a2").GetComponent<MeshRenderer>();
        }
        //否则判断如果当前玩家所在位置的地面是地面陷阱
        else if (playPos.tag == "Spikes")
        {
            //找到当前玩家所在地面陷阱的顶部模型，此模型是相对地面陷阱的子物体
            normal = playPos.FindChild("moving_spikes_a2").GetComponent<MeshRenderer>();
        }
        //否则判断如果当前玩家所在位置的地面是天空陷阱
        else if (playPos.tag == "Sky_Spikes")
        {
            //找到当前玩家所在天空陷阱的顶部模型，此模型是相对天空陷阱的子物体
            normal = playPos.FindChild("smashing_spikes_a2").GetComponent<MeshRenderer>();
        }
        //如果当期角色位置的地板的MeshRenderer不是空的时候
        //也就是说，当前玩家不在坑洞上的时候，绘制蜗牛痕迹
        if (normal != null)
        {
            //判断单排和双排，对不同的单、双排，绘制蜗牛痕迹
            if (pz % 2 == 0)
            {
                //如果是偶数行，就变为颜色one
                normal.material.color = colorOne;
            }
            else
            {
                //否则，就变为颜色Two
                normal.material.color = colorTwo;
            }
        }
        //否则就说明当期玩家在坑洞上
        else
        {
            //那么就给玩家添加刚体组件，使玩家受重力的影响，开始下落
            gameObject.AddComponent<Rigidbody>();
            //由于这里的结束游戏是因为掉入了坑洞，所以第二个参数填写true,因此真正的游戏会在延迟0.5秒之后结束
            StartCoroutine("GameOver", true);
        }

    }
    /// <summary>
    /// 计算角色位置.[角色是否到达地图边缘]
    /// </summary>
    private void CalculatePosition()
    {
        //如果当前角色在地图上的位置的Z值，小于或等于5的时候，就动态生成之后的场景地图
        if (m_MapManager.mapList.Count - pz <= 12)
        {
            //增加生成概率
            m_MapManager.AddPR();
            //获取当前地图mapList集合中的总数量-1的角标和集合中第0个角标，的物体的Transform组件的位置中的z值,并加上半个底边的偏移量
            float offsetz = m_MapManager.mapList[m_MapManager.mapList.Count - 1][0].GetComponent<Transform>().position.z + m_MapManager.bottomLength / 2;
            //调用地图控制器中的 创建场景的方法，并在地图的末尾位置生成。
            m_MapManager.CreateMapItem(offsetz);
        }
    }
    /// <summary>
    /// 陷阱出发，获得宝石,碰撞检查
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter(Collider collider)
    {
        //如果检测到的物体碰撞的标签是Spikes_Attack，那么执行游戏结束方法
        if (collider.tag == "Spikes_Attack")
        {
            //由于这里的结束游戏是需要立即执行的，所以不需要延迟，因此这里填写的是false;
            StartCoroutine("GameOver", false);
        }
        //如果检测到的物体碰撞的标签是Gem，那么执行添加宝石的方法
        if (collider.tag == "Gem")
        {
            //宝石增加
            AddGemCount();
            //销毁宝石，因为这里的标签物体是一个子物体，所以要销毁当前标签上的父物体（详情查看宝石预设体）
            GameObject.Destroy(collider.gameObject.GetComponent<Transform>().parent.gameObject);

        }
    }
    /// <summary>
    /// 协程.游戏结束，如果参数填写false就直接结束，如果是true，那么就延迟0.5秒结束游戏
    /// </summary>
    public IEnumerator GameOver(bool delay)
    {
        //如果delay是真的话，说明，需要延迟0.5秒之后再执行后续代码
        if (delay == true)
        {
            yield return new WaitForSeconds(0.5f);
        }
        if (life == true)
        {
            Debug.Log("游戏结束~！");
            //关闭摄像机跟随，将startFollow设置为false
            m_CameraFollow.startFollow = false;
            //玩家死亡
            life = false;
            //存储数据
            SaveData();
            //todo:UI相关交互
            //开启协程ResetGame，等待2秒，重置UI
            StartCoroutine("ResetGame");
        }

        //时间缩放=0，意味着游戏时间暂停 使用这个方法容易产生Unity的崩溃
        //Time.timeScale = 0;
    }

    /// <summary>
    /// 重置角色
    /// </summary>
    private void ResetPlayer()
    {
        //重置角色身上的组件，移除掉角色身上的刚体组件
        GameObject.Destroy(gameObject.GetComponent<Rigidbody>());
        //重置角色的坐标点
        pz = 3;
        px = 3;
        //重置角色的存活状态，复活角色
        life = true;
        //重置核心玩法的的分数计数器
        socreCount = 0;

    }
    /// <summary>
    /// 协程.重新开始游戏
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetGame()
    {
        //延迟等在2秒
        yield return new WaitForSeconds(2f);
        //调用执行重置角色的方法
        ResetPlayer();
        //重置游戏地图
        m_MapManager.ResetGameMap();
        //重置摄像机
        m_CameraFollow.ResetCamera();
        //调用执行重置UI的方法
        m_UIManager.ResetUI();
    }
    /// <summary>
    /// 增加宝石数
    /// </summary>
    private void AddGemCount()
    {
        //每次调用此方法说明获得了宝石，所以每次宝石数都会+1
        gemCount++;
        //测试宝石数量的增加
        Debug.Log("宝石数：" + gemCount);
        //执行更新核心玩法数据的方法
        m_UIManager.UpdateData(socreCount, gemCount);

    }
    /// <summary>
    /// 增加分数的方法
    /// </summary>
    private void AddScoreCount()
    {
        //每次调用此方法说明玩家移动了，所以每次移动分数都会+1
        socreCount++;
        Debug.Log("得分：" + socreCount);
        //执行更新核心玩法数据的方法
        m_UIManager.UpdateData(socreCount, gemCount);
    }
    /// <summary>
    /// 存储数据
    /// </summary>
    private void SaveData()
    {
        //向注册表中存储宝石数据，键值对的形式
        PlayerPrefs.SetInt("gem", gemCount);
        //判断当前的得分是否比之前的得分高，如果高，就替换，如果不高就不替换
        //从注册表中取出score的值，第二个参数为0，是因为开始的时候注册表中是没有score的值，所以给一个初始值0
        if (socreCount > PlayerPrefs.GetInt("score", 0))
        {
            //将目前的最大值保存到注册表中
            PlayerPrefs.SetInt("score", socreCount);
        }
    }
}
