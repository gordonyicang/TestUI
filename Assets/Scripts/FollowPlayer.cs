using System.Collections;
using UnityEngine;
//脚本挂在摄像机上
public class FollowPlayer : MonoBehaviour
{
    //定义一个Transform类型的player
    private Transform player;
    //定义摄像机与人物的偏移位置
    private Vector3 offsetStation;
    //在Awake里获取到移动物体Player的transform组件，其实也是初始化定义的字段
    void Awake()
    {   
        //得到组件，先是给Player设置个Tag,当然也可以用Find来找Player名的方式，下面；但是不建议使用。
       // player = GameObject.Find("Player").transform;
        player = GameObject.FindGameObjectWithTag("target").transform;
        //让摄像机朝向人物的位置
        transform.LookAt(player.position);
        //得到偏移量
        offsetStation = transform.position - player.position;
    }

    void Update()
    {
        //让摄像机的位置= 人物行走的位置+与偏移量的相加
        transform.position = offsetStation + player.position;
    }
}
