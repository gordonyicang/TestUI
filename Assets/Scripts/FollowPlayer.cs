using System.Collections;
using UnityEngine;
//�ű������������
public class FollowPlayer : MonoBehaviour
{
    //����һ��Transform���͵�player
    private Transform player;
    //����������������ƫ��λ��
    private Vector3 offsetStation;
    //��Awake���ȡ���ƶ�����Player��transform�������ʵҲ�ǳ�ʼ��������ֶ�
    void Awake()
    {   
        //�õ���������Ǹ�Player���ø�Tag,��ȻҲ������Find����Player���ķ�ʽ�����棻���ǲ�����ʹ�á�
       // player = GameObject.Find("Player").transform;
        player = GameObject.FindGameObjectWithTag("target").transform;
        //����������������λ��
        transform.LookAt(player.position);
        //�õ�ƫ����
        offsetStation = transform.position - player.position;
    }

    void Update()
    {
        //���������λ��= �������ߵ�λ��+��ƫ���������
        transform.position = offsetStation + player.position;
    }
}
