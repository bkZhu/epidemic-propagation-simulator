using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fin : MonoBehaviour
{
    public int count = 0;
    public int day = 0;
    public static int end_day = 3;
    void FixedUpdate() {
        //目前是第2天晚结算
        count++;
        if (day == (end_day-1)&&count==23*60) {
            Debug.Log("deal()");
            user_info.deal();
        }
        if (count == 24 * 60) {
            count = 0;
            day++;
        }
    }
}
