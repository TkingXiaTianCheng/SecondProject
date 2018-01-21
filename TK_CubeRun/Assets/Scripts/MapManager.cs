using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 地图管理器
/// </summary>
public class MapManager : MonoBehaviour
{

    //地图存储数据集合
    public List<GameObject[]> mapList = new List<GameObject[]>();

    //定义角标变量来存储当天塌陷的是第几行场景
    private int index = 0;


    //定义坑洞的生成概率
    private int pr_hole = 0;
    //定义地面陷阱（钉子）的生成概率
    private int pr_spikes = 0;
    //定义天空陷阱（钉子）的生成概率
    private int pr_sky_spikes = 0;
    //定义宝石的生成概率
    private int pr_gem = 2;


    //定义私有的GameObject类型的字段，用于动态加载地板的预制体
    private GameObject m_prefab_tile;
    //定义私有的GameObject类型的字段，用于动态加载墙壁的预制体
    private GameObject m_prefab_wall;
    //定义私有的GameObject类型的字段，用于动态加载地面陷阱（钉子）的预制体
    private GameObject m_prefab_spikes;
    //定义私有的GameObject类型的字段，用于动态加载天空陷阱（钉子）的预制体
    private GameObject m_prefab_sky_spikes;
    //定义私有的GameObject类型的字段，用于动态宝石的预制体
    private GameObject m_prefab_gem;


    //获取MapManager的Transform组件
    private Transform m_Transform;
    //定义引用变量来获取PlayerController脚本
    private PlayerController m_PlayerController;

    //生产预设体的边长是0.254F
    //定义临时变量来存放，计算等腰直角三角形的底边，用来实现单排菱形的平铺
    public float bottomLength = Mathf.Sqrt(2) * 0.254f;

    //定义私有的颜色，使用RGB的方式来表现 墙壁用的颜色
    private Color colorWall = new Color(87 / 255f, 93 / 255f, 169 / 255f);
    //定义私有的颜色，使用RGB的方式来表现 单排用颜色
    private Color colorOne = new Color(124 / 255f, 155 / 255f, 230 / 255f);
    //定义私有的颜色，使用RGB的方式来表现 双排用颜色
    private Color colorTwo = new Color(125 / 255f, 169 / 255f, 233 / 255f);


    void Start()
    {
        //动态加载地板预制体
        m_prefab_tile = Resources.Load("tile_white") as GameObject;
        //动态加载墙壁预制体
        m_prefab_wall = Resources.Load("wall2") as GameObject;
        //动态加载地面陷阱（钉子）预制体
        m_prefab_spikes = Resources.Load("moving_spikes") as GameObject;
        //动态加载天空陷阱（钉子）预制体
        m_prefab_sky_spikes = Resources.Load("smashing_spikes") as GameObject;
        //动态加载宝石预制体
        m_prefab_gem = Resources.Load("gem 2") as GameObject;
        //获取Transform组件
        m_Transform = gameObject.GetComponent<Transform>();
        //获取PlayerController脚本
        m_PlayerController = GameObject.Find("Player").GetComponent<PlayerController>();
        //调用方面创建地图元素
        CreateMapItem(0);

    }

    void Update()
    {
        //地图的遍历测试
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string str = "";
            for (int i = 0; i < mapList.Count; i++)
            {
                for (int j = 0; j < mapList[i].Length; j++)
                {
                    str += mapList[i][j].name;
                    mapList[i][j].name = i + "_" + j;
                }
                str += "\n";
            }
            Debug.Log(str);
        }
    }

    /// <summary>
    /// 创建地图元素,offsetZ=“地图生成偏移值”
    /// </summary>
    /// <param name="offsetZ"></param>
    public void CreateMapItem(float offsetZ)
    {
        for (int i = 0; i < 10; i++)
        {
            //单排
            //对应单排生产的个数
            GameObject[] itemOne = new GameObject[6];
            for (int j = 0; j < 6; j++)
            {
                //定义实例化的预制体位置坐标
                Vector3 pos = new Vector3(j * bottomLength, 0, offsetZ + i * bottomLength);
                //定义实例化的预制体的选择角度
                Vector3 rot = new Vector3(-90, 45, 0);
                //创建一个临时变量来存储GameObject的实例化引用
                GameObject tile = null;
                if (j == 0 || j == 5)
                {
                    //墙壁
                    // Quaternion.Euler(Vector3)将欧拉角转换成四元数
                    tile = GameObject.Instantiate(m_prefab_wall, pos, Quaternion.Euler(rot)) as GameObject;
                    //给实例化的预设体修改颜色
                    tile.GetComponent<MeshRenderer>().material.color = colorWall;
                }
                else
                {
                    //单排
                    //定义区域变量pr来存储计算概率方法的返回值
                    int pr = CalculatePR();
                    //如果概率计算方法返回的值是0，代表是瓷砖，所以执行下面的语句块，生成瓷砖
                    if (pr == 0)
                    {
                        //实例化预制体，在Pos的位置，Quaternion.Euler(Vector3)将欧拉角转换成四元数，m_prefab_tile瓷砖的预制体
                        tile = GameObject.Instantiate(m_prefab_tile, pos, Quaternion.Euler(rot)) as GameObject;
                        //给实例化的预设体下的子物体修改颜色
                        tile.GetComponent<Transform>().FindChild("normal_a2").GetComponent<MeshRenderer>().material.color = colorOne;
                        //给实例化的预设体模型修改颜色
                        tile.GetComponent<MeshRenderer>().material.color = colorOne;
                        //声明局域变量，存储宝石生成算法的返回值
                        int gemPr = CalculateGemPR();
                        //如果是1，代表要生成宝石
                        if (gemPr == 1)
                        {
                            //生成宝石
                            //宝石预制体，当前瓷砖的位置+V3类型的偏移量，无旋转
                            GameObject gem = GameObject.Instantiate(m_prefab_gem, tile.GetComponent<Transform>().position + new Vector3(0, 0.06f, 0), Quaternion.identity) as GameObject;
                            //将生成的宝石设置为瓷砖的子物体，为了实现，当瓷砖下落的时候，宝石也会随之下落
                            gem.GetComponent<Transform>().SetParent(tile.GetComponent<Transform>());
                        }
                    }
                    //坑洞
                    //如果概率计算方法返回的值是1，所以执行下面的语句块，生成坑洞
                    else if (pr == 1)
                    {
                        //生成一个新的空物体
                        tile = new GameObject();
                        //确定空物体的位置
                        tile.GetComponent<Transform>().position = pos;
                        //确定空物体的旋转角度
                        tile.GetComponent<Transform>().rotation = Quaternion.Euler(rot);
                    }
                    //地面陷阱（钉子）
                    //如果概率计算方法返回的值是2，所以执行下面的语句块，生成地面陷阱（钉子）
                    else if (pr == 2)
                    {
                        //实例化预制体，在Pos的位置，Quaternion.Euler(Vector3)将欧拉角转换成四元数，m_prefab_spikes地面陷阱（钉子）的预制体
                        tile = GameObject.Instantiate(m_prefab_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                    }
                    //天空陷阱（钉子）
                    //如果概率计算方法返回的值是3，所以执行下面的语句块，生成天空陷阱（钉子）
                    else if (pr == 3)
                    {
                        //实例化预制体，在Pos的位置，Quaternion.Euler(Vector3)将欧拉角转换成四元数，m_prefab_spikes地面陷阱（钉子）的预制体
                        tile = GameObject.Instantiate(m_prefab_sky_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                    }


                }
                //给实例化的预设体添加一个父物体
                tile.GetComponent<Transform>().SetParent(m_Transform);
                //每生产一个墙壁/地板就进行一次存储
                itemOne[j] = tile;
            }
            //每生产完一排，就添加到集合中
            mapList.Add(itemOne);


            //双排
            //对应单排生产的个数
            GameObject[] itemTwo = new GameObject[5];
            for (int j = 0; j < 5; j++)
            {
                //定义实例化的预制体位置坐标
                Vector3 pos = new Vector3(j * bottomLength + bottomLength / 2, 0, offsetZ + i * bottomLength + bottomLength / 2);
                //定义实例化的预制体的选择角度
                Vector3 rot = new Vector3(-90, 45, 0);

                GameObject tile = null;

                //定义区域变量pr来存储计算概率方法的返回值
                int pr = CalculatePR();
                //如果概率计算方法返回的值是0，代表是瓷砖，所以执行下面的语句块，生成瓷砖
                if (pr == 0)
                {
                    //实例化预制体，在Pos的位置，Quaternion.Euler(Vector3)将欧拉角转换成四元数，m_prefab_tile瓷砖的预制体
                    tile = GameObject.Instantiate(m_prefab_tile, pos, Quaternion.Euler(rot)) as GameObject;
                    //给实例化的预设体下的子物体修改颜色
                    tile.GetComponent<Transform>().FindChild("normal_a2").GetComponent<MeshRenderer>().material.color = colorTwo;
                    //给实例化的预设体模型修改颜色
                    tile.GetComponent<MeshRenderer>().material.color = colorTwo;
                }
                //坑洞
                //如果概率计算方法返回的值是1，所以执行下面的语句块，生成坑洞
                else if (pr == 1)
                {
                    //生成一个新的空物体
                    tile = new GameObject();
                    //确定空物体的位置
                    tile.GetComponent<Transform>().position = pos;
                    //确定空物体的旋转角度
                    tile.GetComponent<Transform>().rotation = Quaternion.Euler(rot);
                }
                //地面陷阱（钉子）
                //如果概率计算方法返回的值是2，所以执行下面的语句块，生成地面陷阱（钉子）
                else if (pr == 2)
                {
                    //实例化预制体，在Pos的位置，Quaternion.Euler(Vector3)将欧拉角转换成四元数，m_prefab_spikes地面陷阱（钉子）的预制体
                    tile = GameObject.Instantiate(m_prefab_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                }
                //天空陷阱（钉子）
                //如果概率计算方法返回的值是3，所以执行下面的语句块，生成天空陷阱（钉子）
                else if (pr == 3)
                {
                    //实例化预制体，在Pos的位置，Quaternion.Euler(Vector3)将欧拉角转换成四元数，m_prefab_spikes地面陷阱（钉子）的预制体
                    tile = GameObject.Instantiate(m_prefab_sky_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                }



                //给实例化的预设体添加一个父物体
                tile.GetComponent<Transform>().SetParent(m_Transform);
                //每生产一个地板就进行一次存储
                itemTwo[j] = tile;
            }
            //每生产完一排，就添加到集合中
            mapList.Add(itemTwo);
        }

    }

    /// <summary>
    /// 开启地面塌陷效果
    /// </summary>
    public void StartTileDown()
    {
        //开启协程
        StartCoroutine("TileDown");
    }
    /// <summary>
    /// 停止地面塌陷效果
    /// </summary>
    public void StopTileDown()
    {
        //停止协程
        StopCoroutine("TileDown");
    }
    /// <summary>
    /// 地面塌陷，协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator TileDown()
    {
        //while true是一个死循环
        while (true)
        {
            //在每一次塌陷处理之前进行一次时间间隔的处理
            yield return new WaitForSeconds(0.2f);
            //对当前index行数进行塌陷处理
            //遍历maoList指定[index]，对应的行数，的长度下游戏物体
            for (int i = 0; i < mapList[index].Length; i++)
            {
                //指定行数下对应的i游戏物体,添加一个刚体组件，使得游戏物体受重力的影响，开始下落
                Rigidbody rb = mapList[index][i].AddComponent<Rigidbody>();
                //给遍历的物体添加一个1~10随机大小,随机方向的角旋转速度的值
                rb.angularVelocity = new Vector3(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f)) * Random.Range(1, 10);
                //对每一个物体添加一个定时销毁命令,每0.6秒销毁
                GameObject.Destroy(mapList[index][i], 1f);
            }
            //如果当前塌陷的行号和玩家的位置相等，那么就停止塌陷
            if (m_PlayerController.pz == index)
            {
                //停止协程
                StopTileDown();
                //给当前位置的玩家物体添加刚体组件，使玩家受重力的影响，开始下落
                m_PlayerController.gameObject.AddComponent<Rigidbody>();
                //由于这里的结束游戏是因为地板掉玩家死亡，所以第二个参数填写true,因此真正的游戏会在延迟0.5秒之后结束
                m_PlayerController.StartCoroutine("GameOver", true);
            }
            //移动角标index，开始对下一行进行下落处理
            index++;

        }
    }
    /// <summary>
    /// 计算概率值的方法
    /// 返回0: 瓷砖
    /// 返回1: 坑洞
    /// 返回2: 地面陷阱（钉子）
    /// 返回3: 天空陷阱（钉子）
    /// </summary>
    /// <returns></returns>
    private int CalculatePR()
    {
        //定义随机数pr的取值为1~100；因为随机数的取值是MAX-1来取得
        //之所以在第一段就生成坑洞就是因为随机数取值的时候从最小值0开始取值
        int pr = Random.Range(1, 100);
        //如果随机值小于或等于当前的坑洞的值，那么就返回1
        if (pr <= pr_hole)
        {
            return 1;
        }
        //如果随机值大于31且随机值小于当前的地面陷阱（钉子）的值+30，那么就返回2
        else if (31 < pr && pr < pr_spikes + 30)
        {
            return 2;
        }
        //如果随机值大于61且随机值小于当前的天空陷阱（钉子）的值+60，那么就返回3
        else if (61 < pr && pr < pr_sky_spikes + 60)
        {
            return 3;
        }
        //不生成陷阱
        return 0;
    }
    /// <summary>
    /// 计算宝石的生成概率j
    /// </summary>
    /// <returns>0：不生成，1：生成</returns>
    private int CalculateGemPR()
    {
        //随机1~100之间去一个随机数
        int pr = Random.Range(1, 100);
        //如果随机的pr小于当前宝石生成概率时
        if (pr <= pr_gem)
        {
            //返回1，生成宝石
            return 1;
        }

        //不生成宝石
        return 0;
    }

    /// <summary>
    /// 增加概率
    /// 路障每生成一段，生成坑洞的概率增加2%
    /// 路障每生成一段，生成地面陷阱（钉子）的概率增加1%
    /// </summary>
    public void AddPR()
    {
        //路障每生成一段，生成坑洞的概率增加2%
        pr_hole += 2;
        //路障每生成一段，生成地面陷阱（钉子）的概率增加1%
        pr_spikes += 2;
        //路障每生成一段，生成天空陷阱（钉子）的概率增加1%
        pr_sky_spikes += 2;
    }

    /// <summary>
    /// 重置游戏地图
    /// </summary>
    public void ResetGameMap()
    {
        //重置所有已经生成的物体（删除）
        //声明Transform的数组用来获取当前创建的所以子物体的Transform
        Transform[] sontransform = m_Transform.GetComponentsInChildren<Transform>();
        //通过一个循环来删除掉所有已经创建的场景物体
        //这里的i=1，是从第一个子物体开始，如果是0，就是从父物体开始，会吧自己也删除掉
        for (int i = 1; i < sontransform.Length; i++)
        {
            GameObject.Destroy(sontransform[i].gameObject);
        }

        //重置所有的概率值
        //重置坑洞的生成概率
         pr_hole = 0;
        //重置地面陷阱（钉子）的生成概率
        pr_spikes = 0;
        //重置天空陷阱（钉子）的生成概率
        pr_sky_spikes = 0;
        //重置宝石的生成概率
        pr_gem = 2;

        //重置塌陷脚标
        index = 0;

        //重置地图数据，清空mapList集合
        mapList.Clear();
        //重置地图生成坐标，创建新的场景元素
        CreateMapItem(0);
    }
}
