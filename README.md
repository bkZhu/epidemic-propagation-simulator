# epidemic-propagation-simulator

this is an infector contact tracking system based on unity3d engine.

here is the base project：[Virus-School](https://github.com/YunxiuXu/Virus-School)

+ I redesign the character behavior and the scene.
+ I add the functions of data collection and storage.
+ At the last day the system will find the related contact people.

## Scene design
There are four place in this system :

![image](https://github.com/bkZhu/epidemic-propagation-simulator/blob/main/images/scenes.png)

## the types of people:

![image](https://github.com/bkZhu/epidemic-propagation-simulator/blob/main/images/type.png)

+ first level contacted person means : the person who direct contact with infectors
+ second level contacted person means : the person who indirect contact with infectors

About the movement of each person, to facilitate debugging, in the system, the movement of the first infector will be relatively small, the rest of the characters will move between places at different time points every day, and at 21:00 every day, all the characters will return to the residence until the beginning of the next day

this is the initial state:

![image](https://github.com/bkZhu/epidemic-propagation-simulator/blob/main/images/init_state.png)

and we start the system, some people will move:

![image](https://github.com/bkZhu/epidemic-propagation-simulator/blob/main/images/move_state.png)

we can see that the infector goes to the shopping market and one healthy person stays with him.
After serval minutes, the healthy person goes out. Meanwhile, the system stores a string of "student5(name),1(place_id),0(days),168(in_time),251(out_time)" in List<string>.

![image](https://github.com/bkZhu/epidemic-propagation-simulator/blob/main/images/move_state2.png)
![image](https://github.com/bkZhu/epidemic-propagation-simulator/blob/main/images/info.png)

After three days:

![image](https://github.com/bkZhu/epidemic-propagation-simulator/blob/main/images/last_state.png)

Finally, the system will find out all the first-level person (red) and second-level person (yellow). The specific list is shown:

![image](https://github.com/bkZhu/epidemic-propagation-simulator/blob/main/images/track_info.png)

## Details
Here, I use a HashMap<UserID,Info> to store the track information.
Each place has a trigger, when people go into the place, the trigger will write a message to the HashMap.
At the end of the last day, the system will first build risk tables that represent the risk time of each place using the first infector track information.
Then, the system will check each person to find out whether he is the potential infector.
