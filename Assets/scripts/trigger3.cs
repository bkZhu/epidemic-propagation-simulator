using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigger3 : MonoBehaviour
{

    private static Hashtable stu_stime = new Hashtable();//姓名和s_time
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "stu")//如果碰上学生
        {
            nav stu = other.gameObject.GetComponent<nav>();
            Debug.Log(other.name + "在" + stu.count + "进入水果店");
            stu_stime.Add(other.name, stu.count);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "stu")//如果碰上学生
        {
            nav stu = other.gameObject.GetComponent<nav>();
            Debug.Log(other.name + "在" + stu.count + "离开水果店");
            Debug.Log("添加数据:" + other.name + ",3," + stu.day + "," + stu_stime[other.name] + "," + stu.count);
            user_info.list.Add(other.name + ",3," + stu.day + "," + stu_stime[other.name] + "," + stu.count);
            stu_stime.Remove(other.name);
        }
    }
}
