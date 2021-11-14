using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("物体")]
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    public GameObject obj4;
    public GameObject obj5;
    public GameObject radish;
    class Frame
    {
        public float[] position;
        public float[] rotation;
        public float[] scale;
        public int type;
        public string content;
        public bool useGlobalX;

        public Frame() { }
        public Frame(float[] p, float[] r, float[] s, int t, string c, bool uX)
        {
            position = p;
            rotation = r;
            scale = s;
            type = t;
            content = c;
            useGlobalX = uX;
        }
    }
    Queue q = new Queue();
    Queue characterQueue = new Queue();
    Hashtable ht = new Hashtable();
    float globalX = 0;
    int curCount = 0, radishGetoutNum = 5;
    bool[] used = new bool[20];
    String radishSignal = "萝卜越长越大";
    String comingSignal = "快来帮忙拔萝卜";
    String comingWord = "来了";

    // Start is called before the first frame update
    void Start()
    {
        radish.GetComponent<Animator>().SetTrigger("away_canvas");
        ht.Add(1, obj1);
        ht.Add(2, obj2);
        ht.Add(3, obj3);
        ht.Add(4, obj4);
        ht.Add(5, obj5);
        ht.Add(0, radish);

        readFile();
    }
    int finishedLineNumber = 0;
    void readFile()
    {
        //Debug.Log("Read Here!");
        StreamReader sr = new StreamReader("E:\\pycharm\\real_time_audio\\a.txt", Encoding.UTF8);
        String line;
        for (int i = 0; i < finishedLineNumber; i++)
        {
            line = sr.ReadLine();
        }
        while ((line = sr.ReadLine()) != null)
        {
            Debug.Log(line);
            if (line.Contains("老公公就去拔萝卜"))
            {
                Frame f = new Frame(new float[3] { 3, 0, 90 }, new float[3] { 0, 180, 0 }, new float[3] { 1, 1, 1 }, 1, "Pulling A Rope", true);
                q.Enqueue(f);
            }
            if (line.Contains("老婆婆" + comingSignal) || line.Contains("老婆婆，" + comingSignal))
            {
                characterQueue.Enqueue(2);
            }
            if (line.Contains("小姑娘" + comingSignal) || line.Contains("小姑娘，" + comingSignal))
            {
                characterQueue.Enqueue(3);
            }
            if (line.Contains("小花狗" + comingSignal) || line.Contains("小花狗，" + comingSignal))
            {
                characterQueue.Enqueue(4);
            }
            if (line.Contains("小花猫" + comingSignal) || line.Contains("小花猫，" + comingSignal))
            {
                characterQueue.Enqueue(5);
            }
            if (line.Contains(comingWord + comingWord) || line.Contains(comingWord + "，" + comingWord))
            {
                Frame f = new Frame(new float[3] { 3, 0, 90 }, new float[3] { 0, 180, 0 }, new float[3] { 1, 1, 1 }, (int)characterQueue.Dequeue(), "Pulling A Rope", true);
                q.Enqueue(f);
            }
            if (line.Contains(radishSignal))
            {
                //Frame f = new Frame(new float[3] { -2.3F, 0.67F, 90 }, new float[3] { 0, 0, 0 }, new float[3] { 1, 1, 1 },0, "", false);
                //q.Enqueue(f);
                radish.GetComponent<Animator>().SetTrigger("new_idle");//萝卜首次出现
            }
            finishedLineNumber += 1;
        }
        sr.Close();

    }
    // Update is called once per frame
    float lastTime = 0, time = 0;
    void FixedUpdate()
    {
        time += Time.deltaTime;
        //Debug.Log(time - lastTime);
        if (time - lastTime > 3)
        {
            lastTime = time;
            readFile();
        }

        //Get an element from the queue
        if (q.Count > 0)
        {
            Frame f = (Frame)q.Dequeue();
            if (!used[f.type])
            {
                if (f.useGlobalX)
                {
                    f.position[0] = globalX;
                    globalX += 3;
                }
                GameObject cur = (GameObject)ht[f.type];
                cur.transform.position = new Vector3(f.position[0], f.position[1], f.position[2]);
                cur.transform.rotation = Quaternion.Euler(f.rotation[0], f.rotation[1], f.rotation[2]);
                used[f.type] = true;
                curCount++;
                if (curCount == radishGetoutNum)
                {
                    radish.GetComponent<Animator>().SetTrigger("new_getout");
                }
                //Set scale here
                //cur.GetComponent<Animation>().Play(f.content); //播放动画用trigger
            }

        }
        if (Input.GetKey("d"))
        {
            Frame f = new Frame(new float[3] { 3, 0, 90 }, new float[3] { 0, 180, 0 }, new float[3] { 1, 1, 1 }, 2, "Pulling A Rope", true);
            q.Enqueue(f);

        }
        if (Input.GetKey("f"))
        {
            Frame f = new Frame(new float[3] { 6, 0, 90 }, new float[3] { 0, 180, 0 }, new float[3] { 1, 1, 1 }, 3, "Pulling A Rope", true);
            q.Enqueue(f);
        }
    }
}

