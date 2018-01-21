using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// UI管理器.
/// </summary>
public class UIManager : MonoBehaviour
{
    //声明开始UI界面的引用变量
    private GameObject m_StartUI;
    //声明核心玩法UI界面的引用变量
    private GameObject m_GameUI;
    //声明商城UI界面的引用变量
    private GameObject m_ShopUI;


    //声明开始界面的得分引用变量
    private UILabel m_StartUI_ScoreLabel;
    //声明开始界面的宝石引用变量
    private UILabel m_StartUI_GemLabel;
    //声明开始界面的开始游戏按钮引用变量
    private GameObject m_StartUI_PlayButton;


    //声明核心玩法界面的得分引用变量
    private UILabel m_GameUI_ScoreLabel;
    //声明核心玩法界面的宝石引用变量
    private UILabel m_GameUI_GemLabel;


    //声明商城界面的宝石引用变量

    //获取玩家角色脚本的引用变量
    private PlayerController m_playerController;


    void Start()
    {
        //获取开始UI界面的引用
        m_StartUI = GameObject.Find("Start_UI");
        //获取开始UI界面下的Score_Label子物体的UILabel引用
        m_StartUI_ScoreLabel = m_StartUI.GetComponent<Transform>().FindChild("Start_Score_Label").GetComponent<UILabel>();
        //获取开始UI界面下的Gem_Label子物体的UILabel引用
        m_StartUI_GemLabel = m_StartUI.GetComponent<Transform>().FindChild("Start_Gem_Label").GetComponent<UILabel>();
        //获取开始UI界面下的Play_Button子物体的UILabel引用
        m_StartUI_PlayButton = GameObject.Find("Play_Button");

        UIEventListener.Get(m_StartUI_PlayButton).onClick = StartGameButton;


        //获取核心玩法UI界面的引用
        m_GameUI = GameObject.Find("Game_UI");
        //获取开始UI界面下的Game_Score_Label子物体的UILabel引用
        m_GameUI_ScoreLabel = m_GameUI.GetComponent<Transform>().FindChild("Game_Score_Label").GetComponent<UILabel>();
        //获取开始UI界面下的Game_Gem_Label子物体的UILabel引用
        m_GameUI_GemLabel = m_GameUI.GetComponent<Transform>().FindChild("Game_Gem_Label").GetComponent<UILabel>();


        //获取商城UI界面的引用
        m_ShopUI = GameObject.Find("Shop_UI");


        //获取玩家角色物体上的脚本
        m_playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        //隐藏核心玩法UI界面
        m_GameUI.SetActive(false);
        //隐藏商城UI界面
        m_ShopUI.SetActive(false);
        //调用方法对UI界面进行赋值
        Init();
    }

    /// <summary>
    /// 加载注册表数据
    /// </summary>
    private void Init()
    {
        //这里的0，依然是由于第一赋值，注册表里并没有这个值，给于一个初始值0
        //将注册表里存的键值对赋给开始UI界面下的Score_Label子物体的UILabel组件的text,(这里的+“” 是为了把值强转成String类型)
        m_StartUI_ScoreLabel.text = PlayerPrefs.GetInt("score",0) + "";
        //将注册表里存的键值对赋给开始UI界面下的Gem_Label子物体的UILabel组件的text,(这里的+“/100” 是为了把值强转成String类型)
        m_StartUI_GemLabel.text = PlayerPrefs.GetInt("gem", 0) + "/100";

        //由于这个的得分是刚游戏从新开始的时候，所以这里的得分值是0
        m_GameUI_ScoreLabel.text = "0";
        //将注册表里存的键值对赋给开始UI界面下的Game_Gem_Label子物体的UILabel组件的text,(这里的+“/100” 是为了把值强转成String类型)
        m_GameUI_GemLabel.text = PlayerPrefs.GetInt("gem", 0) + "/100";
    }

    /// <summary>
    /// 核心玩法，更新数据，score分数，gem宝石数
    /// </summary>
    public void UpdateData(int score,int gem)
    {
        //更新核心玩法中的得分显示
        m_GameUI_ScoreLabel.text = score.ToString();
        //更新核心玩法中的宝石显示
        m_GameUI_GemLabel.text = gem + "/100";
        //更新开始界面的宝石显示
        m_StartUI_GemLabel.text = gem + "/100";
    }

    /// <summary>
    /// 开始游戏按钮
    /// </summary>
    /// <param name="go"></param>
    private void StartGameButton(GameObject go)
    {
        Debug.Log("游戏开始");
        //隐藏开始界面UI界面
        m_StartUI.SetActive(false);
        //显示核心玩法UI界面
        m_GameUI.SetActive(true);
        //执行角色控制脚本中的游戏开始方法
        m_playerController.StartGame();
    }

    /// <summary>
    /// 重置UI的方法
    /// </summary>
    public void ResetUI()
    {
        //显示开始界面UI
        m_StartUI.SetActive(true);
        //隐藏核心玩法UI
        m_GameUI.SetActive(false);
        //重置核心玩法界面的得分显示
        m_GameUI_ScoreLabel.text = "0";
    }
}
