using UnityEngine;
using System.Collections;
/// <summary>
/// 摄像机跟随角色移动
/// </summary>
public class CameraFollow : MonoBehaviour
{
    //声明物体上的Transform组件的引用变量
    private Transform m_Transform;

    //声明玩家角色的Transform组件的引用变量
    private Transform m_PlayerTransform;

    //声明变量，用来控制是否开始跟随角色移动
    public bool startFollow = false;

    //声明V3类型的字段，用于储存摄像机最原始的位置
    private Vector3 normalPos;
    void Start()
    {
        //获取物体上的Transform组件
        m_Transform = gameObject.GetComponent<Transform>();
        //获取摄像机一开始的位置信息
        normalPos = m_Transform.position;
        //获取玩家角色上的Transform组件
        m_PlayerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    void Update()
    {
        CameraMove();
    }
    /// <summary>
    /// 摄像机移动
    /// </summary>
    void CameraMove()
    {
        //当开始跟随的开关为真的时候，开始执行跟随玩家角色移动
        if (startFollow)
        {
            //定义一个变量来存储角色的位置,加上相机Z轴上的偏移量
            Vector3 nextPos = new Vector3(m_Transform.position.x, m_PlayerTransform.position.y + 1.5f, m_PlayerTransform.position.z);
            //相机的位置和角色的位置保持一致
            m_Transform.position = Vector3.Lerp(m_Transform.position, nextPos, Time.deltaTime);
        }
    }
    public void ResetCamera()
    {
        //还原摄像机的位置
        m_Transform.position = normalPos;
    }
}
