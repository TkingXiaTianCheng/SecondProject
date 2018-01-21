using UnityEngine;
using System.Collections;
/// <summary>
/// 地面陷阱（钉子）补间动画脚本
/// </summary>
public class Spikes : MonoBehaviour
{
    //声明陷阱的Transform组件引用
    private Transform m_Transform;
    //声明陷阱的子物体Transform组件的引用
    private Transform son_Transform;

    //声明钉子动画的位置变量
    //原始位置
    private Vector3 normalPos;
    //目标位置
    private Vector3 targetPos;

    void Start()
    {
        //获取陷阱的Transform组件
        m_Transform = gameObject.GetComponent<Transform>();
        //获取陷阱的子物体Transform组件的引用
        son_Transform = m_Transform.FindChild("moving_spikes_b").GetComponent<Transform>();

        //钉子的原始位置
        normalPos = son_Transform.position;
        //钉子的目标位置
        targetPos = son_Transform.position + new Vector3(0, 0.15f, 0);


        //开启钉子的补间动画协程
        StartCoroutine("UpAndDown");
    }

    void Update()
    {

    }
    /// <summary>
    /// 钉子的补间动画协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpAndDown()
    {
        while (true)
        {
            //暂停钉子复原的协程（这样在循环中可以保持来回的协程交替开关）
            StopCoroutine("Down");
            //开启钉子向上的协程
            StartCoroutine("Up");
            //等待2秒
            yield return new WaitForSeconds(2.0f);
            //关闭钉子向上的协程
            StopCoroutine("Up");
            //开启钉子复原的协程
            StartCoroutine("Down");
            //等待2秒
            yield return new WaitForSeconds(2.0f);

        }
    }

    /// <summary>
    /// 协程.钉子向上移动
    /// </summary>
    /// <returns></returns>
    private IEnumerator Up()
    {
        while (true)
        {
            //Vector3的Lerp差值算法，从当前的钉子的位置，平滑移动到目标点的位置
            son_Transform.position = Vector3.Lerp(son_Transform.position, targetPos, Time.deltaTime * 30);
            yield return null;
        }
    }
    /// <summary>
    /// 协程.钉子回到初始状态
    /// </summary>
    /// <returns></returns>
    private IEnumerator Down()
    {
        while (true)
        {
            //Vector3的Lerp差值算法，从当前的钉子的位置，平滑移动到原始点的位置
            son_Transform.position = Vector3.Lerp(son_Transform.position, normalPos, Time.deltaTime * 30);
            yield return null;
        }
    }
}
