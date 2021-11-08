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
    Hashtable ht = new Hashtable();
    float globalX = 0;
    bool[] used = new bool[20];

    // Start is called before the first frame update
    void Start()
    {
        ht.Add(1, obj1);
        ht.Add(2, obj2);
        ht.Add(3, obj3);
        Frame f = new Frame(new float[3] { 0, 0, 90 }, new float[3] { 0, 180, 0 }, new float[3] { 1, 1, 1 }, 1, "Pulling A Rope", true);

        q.Enqueue(f);
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
            if (line.Contains("兔子"))
            {
                Frame f = new Frame(new float[3] { 3, 0, 90 }, new float[3] { 0, 180, 0 }, new float[3] { 1, 1, 1 }, 2, "Pulling A Rope", true);
                q.Enqueue(f);
            }
            if (line.Contains("老鼠"))
            {
                Frame f = new Frame(new float[3] { 6, 0, 90 }, new float[3] { 0, 180, 0 }, new float[3] { 1, 1, 1 }, 3, "Pulling A Rope", true);
                q.Enqueue(f);
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
                //Set scale here
                //cur.GetComponent<Animation>().Play(f.content); //不加这句竟然也有动画
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

