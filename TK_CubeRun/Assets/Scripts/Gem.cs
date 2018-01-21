using UnityEngine;
using System.Collections;

public class Gem : MonoBehaviour
{
    //声明引用变量，获取宝石的Transform组件
    private Transform m_Transform;
    //声明引用变量，获取宝石的子物体gem宝石模型的Transform组件
    private Transform m_gem;

    void Start()
    {
        //获取宝石的Transform组件
        m_Transform = gameObject.GetComponent<Transform>();
        //获取宝石的子物体gem宝石模型的Transform组件
        m_gem = m_Transform.FindChild("gem 3").GetComponent<Transform>();
    }

    void Update()
    {
        m_gem.Rotate(new Vector3(Random.Range(10,30), Random.Range(10, 30), Random.Range(10, 30)) * Time.deltaTime);
    }
}
