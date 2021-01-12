using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class user_info 
{
    public static List<string> list = new List<string>();
    private static List<string[]> travel_info = new List<string[]>();
    //pos[地点,天数,时间]
    private static int[,,] pos = new int[4,7,24*60];//用于找出lev1
    private static int[,,] pos2 = new int[4,7,24*60];//用于找出lev2
    public static Hashtable tb1;
    public static Hashtable tb2;
    public static Hashtable travel_tb = new Hashtable();//<string,List<string[]>>
    public static void show()
    {
        for (int i = 0; i < list.Count;i++ )
            Debug.Log(list[i]);
    }
    public static void  deal() 
    {
        //填充pos
        foreach(string content in list){
            string[] tmp = content.Split(',');
            travel_info.Add(tmp);
            ht_add(travel_tb, tmp[0], tmp);
            if (tmp[0].Equals("stu0")) 
            {
                //Debug.Log(content);
                //Debug.Log(tmp.Length);
                int state = int.Parse(tmp[1]);
                int day = int.Parse(tmp[2]);
                int s_time = int.Parse(tmp[3]);
                int e_time = int.Parse(tmp[4]);
                for (int i = s_time; i <= e_time;i++ )
                    pos[state, day, i] = 1;
            }
        }
        Hashtable lev1_ht = calc_lev1(pos,new Hashtable());//计算出一级密接者,结果名单放在lev1_ht中
        show_contacts(lev1_ht, 1);
        tb1 = new Hashtable(lev1_ht);
        fix_pos2(lev1_ht);//填充pos2
        Hashtable lev2_ht = calc_lev1(pos2, lev1_ht);//危险时间用一级密接者来填充
        show_contacts(lev2_ht, 2);
        tb2 = new Hashtable(lev2_ht);
    }
    public static void show_contacts(Hashtable levx_ht, int x) 
    {

        int cnt = 0;
        Debug.Log("一共" + levx_ht.Count + "位lev_"+x);
        foreach (string name in levx_ht.Keys) {
            Debug.Log("第"+cnt+"位"+name+" is lve_"+x);
            cnt++;
        }
    }
    public static Hashtable calc_lev1(int[, ,] pos_array, Hashtable fa_ht)
    {
        //fa_ht指的是这个构成危险时间的成员集合
        Hashtable lev_ht = new Hashtable();
        for(int i=0;i<travel_info.Count;i++)
        {
            string[] info = travel_info[i];
            string name = info[0];
            if (name.Equals("stu0") || lev_ht.ContainsKey(name) || fa_ht.ContainsKey(name))
            {
                continue;
            }
            
            int state = int.Parse(info[1]);
            int day = int.Parse(info[2]);
            int s_time = int.Parse(info[3]);
            int e_time = int.Parse(info[4]);
            //特判如果起始和终点在危险时间内，直接add
            if (pos_array[state,day,s_time] == 1||pos_array[state,day,e_time]==1) {
                ht_add(lev_ht, name, info);
                continue;
            }
            //从开始到结束判断有没有遇见
            for (int ti = s_time; ti <= e_time; ti++) {
                if (pos_array[state, day, ti] == 1)
                {
                    ht_add(lev_ht, name, info);
                    break;
                } 
            }
        }
        return lev_ht;
    }

    public static void fix_pos2(Hashtable lev1_ht) 
    {   //从lev1 stu中，找到这个stu遇见潜伏者的时间，将之后的时间都加到pos2中
        //Hashtable lev2_stime = new Hashtable();//记录lev2 name和int[最早天数,最早时间]
        foreach (string name in lev1_ht.Keys) 
        {
            int early_day = 365;//max
            int early_time = 24 * 60 - 1;//max
            //先去碰见stu0的记录lev1_ht里找最早的时间
            foreach (string[] info in (List<string[]>)lev1_ht[name])
            {
                //从这个 stu所有的打卡记录中找到最早碰见stu0的时刻
                int state = int.Parse(info[1]);
                int day = int.Parse(info[2]);
                int s_time = int.Parse(info[3]);
                int e_time = int.Parse(info[4]);
                if (day > early_day)
                    continue;
                //如果是可能的答案，开始比对时间，查第day天 何时碰见stu0
                for (int i = s_time; i <= e_time; i++) {
                    if (pos[state,day,i] == 1) {
                        if (i < early_time&&day<early_day) {
                            early_time = i;
                            early_day = day;
                        }
                        break;
                    }
                }
            }//最早时间确定完成 分别为early_time和early_day;
            //将pos2填充
            //Debug.Log(name+"于"+early_day+"天"+early_time+"碰到stu0");
            //再去这个人的所有打卡记录travel_tb里找最早的时间
            foreach (string[] info in (List<string[]>)travel_tb[name])
            {
                int state = int.Parse(info[1]);
                int day = int.Parse(info[2]);
                int s_time = int.Parse(info[3]);
                int e_time = int.Parse(info[4]);
                //Debug.Log("地点->" + state + ",day->" + day + ",s->" + s_time + ",e->" + e_time + ",early_time" + early_time);
                if (day < early_day||(day==early_day&&e_time<early_time)){
                    //Debug.Log("时间不对，舍弃");
                    continue;
                }   
                else if (day == early_day) {
                    //碰过之后
                    if (s_time >= early_time) {
                        //Debug.Log("从" + s_time + "到->" + e_time + "都赋值," + "地点" + state);
                        for (int i = s_time; i <= e_time; i++)
                            pos2[state,day,i] = 1;
                    }
                    //在期间碰到stu0
                    else if (s_time < early_time && early_time< e_time  ) {
                        //Debug.Log("从" + early_time + "到->" + e_time + "都赋值," + "地点" + state);
                        for (int i = early_time; i <= e_time; i++)
                            pos2[state,day,i] = 1;
                    }
                }
                else if (day > early_day) {
                    for (int i = s_time; i <= e_time; i++)
                        pos2[state,day,i] = 1;
                }
            }
        }
    }
    public static int check_lev(string name) {

        if (tb1.ContainsKey(name))
            return 1;
        else if (tb2.ContainsKey(name))
            return 2;
        else
            return 0;
    }
    public static void ht_add(Hashtable ht, string key, string[] val)
    {
        if (ht.ContainsKey(key))
        {
            List<string[]> t = (List<string[]>)ht[key];
            t.Add((string[])val.Clone());
            ht[key] = t;
        }
        else
        {
            List<string[]> t = new List<string[]>();
            t.Add((string[])val.Clone());
            ht.Add(key, t);
        }
    }
}
