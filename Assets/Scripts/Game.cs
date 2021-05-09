using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Game : MonoBehaviour
{
    private const int SCREEN_WIDTH = 240;
    private const int SCREEN_HEIGHT = 135;
    private ConcurrentDictionary<int, GameObject> cellContainer = new ConcurrentDictionary<int, GameObject>();
    private float[] CamSizeOptions = { 5.625f, 11.25f, 22.5f, 33.75f, 45.0f, 67.5f };
    private int currentLevelCamSize = 5;
    private Vector3 ResetCamera;
    private Vector3 Origin;
    private Vector3 Diference;

    public bool isPlaying = false;
    public float speed = 0.5f;
    public float timer = 0.0f;
    public GameObject btnSettings;
    public GameObject btnSpeedUp;
    public GameObject btnSpeedDown;
    public GameObject btnDrag;
    public GameObject btnPlay;
    public GameObject btnPause;
    public GameObject btnSave;
    public GameObject btnLoad;
    public bool isDrag = false;
    public Camera cam;
    public HUD hud;

    private static string[] txt = File.ReadAllLines(Environment.CurrentDirectory + @"\Assets\Inputs\Output.txt");
    static Graph graph = new Graph(importFile(txt));
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("SavePattern", SavePattern);
        EventManager.StartListening("LoadPattern", LoadPattern);
        Camera.main.orthographicSize = CamSizeOptions[currentLevelCamSize];
        ResetCamera = cam.transform.position;
        #region CreateButton
        Button _btnSettings = btnSettings.GetComponent<Button>();
        _btnSettings.onClick.AddListener(btnSettingsOnClick);
        Button _btnSpeedUp = btnSpeedUp.GetComponent<Button>();
        _btnSpeedUp.onClick.AddListener(btnSpeedUpOnClick);
        Button _btnSpeedDown = btnSpeedDown.GetComponent<Button>();
        _btnSpeedDown.onClick.AddListener(btnSpeedDownOnClick);
        Button _btnDrag = btnDrag.GetComponent<Button>();
        _btnDrag.onClick.AddListener(btnDragOnClick);
        Button _btnPlay = btnPlay.GetComponent<Button>();
        _btnPlay.onClick.AddListener(btnPlayOnClick);
        Button _btnPause = btnPause.GetComponent<Button>();
        btnPause.SetActive(false);
        _btnPause.onClick.AddListener(btnPauseOnClick);
        Button _btnSave = btnSave.GetComponent<Button>();
        _btnSave.onClick.AddListener(btnSaveOnClick);
        Button _btnLoad = btnLoad.GetComponent<Button>();
        _btnLoad.onClick.AddListener(btnLoadOnClick);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            #region Debug
            //Dictionary<int, bool> deathNote = status(3072);
            //PopulationControl(deathNote, 3072);
            //isPlaying = false;
            //foreach (KeyValuePair<int, GameObject> kvp in cellContainer)
            //{
            //    Debug.Log("Key = " + kvp.Key/* +";    Value = "+kvp.Value*/);
            //}
            #endregion
            if (timer >= speed)
            {
                Stopwatch stopWatch = new Stopwatch();//kiểm tra thời gian chạy của thuật toán (có thể bỏ qua).
                stopWatch.Start();
                timer = 0f;
                ConcurrentDictionary<int, bool> deathNote = handlingWithTask(SCREEN_WIDTH * SCREEN_HEIGHT);
                _PopulationControl(deathNote, SCREEN_WIDTH * SCREEN_HEIGHT);
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = "Min: " + ts.Minutes + "Sec: " + ts.Seconds + "MiliSec: " + ts.Milliseconds;
                UnityEngine.Debug.Log(elapsedTime);
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        UserInput();
    }

    #region Button Events
    void btnSettingsOnClick()//tạm thời hàm này là hàm sinh test
    {
        for (int x = 0; x < SCREEN_WIDTH; x++)
        {
            for (int y = 0; y < SCREEN_HEIGHT; y++)
            {
                int value = x + y * SCREEN_WIDTH + 1;
                if (randomAliveCell())
                {
                    GameObject addGO = Instantiate(Resources.Load("Prefabs/Cell"), new Vector2(x, y), Quaternion.identity) as GameObject;
                    cellContainer.TryAdd(value, addGO);
                }
            }
        }
    }

    void btnSpeedUpOnClick()
    {
        if (speed > 0.2f)
        {
            speed -= 0.1f;
        }
    }

    void btnSpeedDownOnClick()
    {
        speed += 0.1f;
    }

    void btnDragOnClick()
    {
        isDrag = !isDrag;
    }

    void btnPlayOnClick()
    {
        btnPlay.SetActive(false);
        btnPause.SetActive(true);
        isPlaying = true;
    }

    void btnPauseOnClick()
    {
        btnPause.SetActive(false);
        btnPlay.SetActive(true);
        isPlaying = false;
    }

    void btnSaveOnClick()
    {
        isPlaying = false;
        hud.setSaveDialogActive();
    }

    void btnLoadOnClick()
    {
        isPlaying = false;
        hud.setLoadDialogActive();
    }
    #endregion

    #region Drag Camera
    void LateUpdate()
    {
        if (isDrag && currentLevelCamSize < 5)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Origin = MousePos();
            }
            if (Input.GetMouseButton(0))
            {
                Diference = MousePos() - cam.transform.position;
                Vector3 tmp = Origin - Diference;
                //xử lý vấn đề tua cam bay khỏi game fields
                if (currentLevelCamSize == 4)
                {
                    UnityEngine.Debug.Log("is 4");
                    if (tmp.x >= 79 && tmp.x <= 159 && tmp.y >= 44 && tmp.y <= 89)
                    {
                        cam.transform.position = tmp;
                    }
                }
                if(currentLevelCamSize == 3)
                {
                    UnityEngine.Debug.Log("is 3");
                    if (tmp.x >= 59 && tmp.x <= 179 && tmp.y >= 33 && tmp.y <= 101)
                    {
                        cam.transform.position = tmp;
                    }
                }
                if (currentLevelCamSize == 2)
                {
                    UnityEngine.Debug.Log("is 2");
                    if (tmp.x >= 39 && tmp.x <= 199 && tmp.y >= 21 && tmp.y <= 112)
                    {
                        cam.transform.position = tmp;
                    }
                }
                if (currentLevelCamSize == 1)
                {
                    UnityEngine.Debug.Log("is 1");
                    if (tmp.x >= 19 && tmp.x <= 219 && tmp.y >= 10 && tmp.y <= 123)
                    {
                        cam.transform.position = tmp;
                    }
                }
                if (currentLevelCamSize == 0)
                {
                    UnityEngine.Debug.Log("is 0");
                    if (tmp.x >= 9 && tmp.x <= 229 && tmp.y >= 5 && tmp.y <= 129)
                    {
                        cam.transform.position = tmp;
                    }
                }
            }
            if (Input.GetMouseButton(1)) // reset camera to original position
            {
                cam.transform.position = ResetCamera;
            }
        }
    }
    Vector3 MousePos()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }
    #endregion

    void UserInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())//kiểm tra có đang click vào các element thuộc UI không
            {
                UnityEngine.Debug.Log("Clicked on the UI");
                return;
            }
            if (isDrag) return;
            UnityEngine.Debug.Log(isDrag);
            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //đổi từ value của cell sang tọa độ x, y trong hệ tọa độ 2 chiều
            int x = Mathf.RoundToInt(mousePoint.x);
            int y = Mathf.RoundToInt(mousePoint.y);
            int value = x + y * SCREEN_WIDTH + 1;
            UnityEngine.Debug.Log("x = " + x + "; y = " + y + "; value = " + value);
            if (!cellContainer.ContainsKey(value))
            {
                //Debug.Log("yes");
                GameObject addGO = Instantiate(Resources.Load("Prefabs/Cell"), new Vector2(x, y), Quaternion.identity) as GameObject;
                cellContainer.TryAdd(value, addGO);
            }
            else
            {
                //Debug.Log("no");
                GameObject tmp = cellContainer[value];
                Destroy(tmp);
                GameObject deleteGO = cellContainer[value];
                cellContainer.TryRemove(value, out deleteGO);
            }
            return;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) //phóng to
        {
            if (currentLevelCamSize > 0)
            {
                currentLevelCamSize--;
                UnityEngine.Debug.Log("zoom in");
                Camera.main.orthographicSize = CamSizeOptions[currentLevelCamSize];
            }
            return;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) //thu bé
        {
            if (currentLevelCamSize < 5)
            {
                currentLevelCamSize++;
                UnityEngine.Debug.Log("zoom out");
                Vector3 CamCurrentPos = cam.transform.position;
                //xử lý vấn đề từ camSize level cao drag ra góc rồi thu bé lại sẽ out gamefields
                if (currentLevelCamSize == 5)
                {
                    CamCurrentPos.x = 119.5f;
                    CamCurrentPos.y = 67f;
                }
                if (currentLevelCamSize == 4)
                {
                    if (CamCurrentPos.x < 79) CamCurrentPos.x = 79;
                    if (CamCurrentPos.x > 159) CamCurrentPos.x = 159;
                    if (CamCurrentPos.y < 44) CamCurrentPos.y = 44;
                    if (CamCurrentPos.y > 89) CamCurrentPos.y = 89;
                }
                if (currentLevelCamSize == 3)
                {
                    if (CamCurrentPos.x < 59) CamCurrentPos.x = 59;
                    if (CamCurrentPos.x > 179) CamCurrentPos.x = 179;
                    if (CamCurrentPos.y < 33) CamCurrentPos.y = 33;
                    if (CamCurrentPos.y > 101) CamCurrentPos.y = 101;
                }
                if (currentLevelCamSize == 2)
                {
                    if (CamCurrentPos.x < 39) CamCurrentPos.x = 39;
                    if (CamCurrentPos.x > 199) CamCurrentPos.x = 199;
                    if (CamCurrentPos.y < 21) CamCurrentPos.y = 21;
                    if (CamCurrentPos.y > 112) CamCurrentPos.y = 112;
                }
                if (currentLevelCamSize == 1)
                {
                    if (CamCurrentPos.x < 19) CamCurrentPos.x = 19;
                    if (CamCurrentPos.x > 219) CamCurrentPos.x = 219;
                    if (CamCurrentPos.y < 10) CamCurrentPos.y = 10;
                    if (CamCurrentPos.y > 123) CamCurrentPos.y = 123;
                }
                cam.transform.position = CamCurrentPos;
                Camera.main.orthographicSize = CamSizeOptions[currentLevelCamSize];
            }
            return;
        }
    }
    void SavePattern()
    {
        string path = "Patterns";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string _patternString = "";
        foreach(KeyValuePair<int,GameObject> kvp in cellContainer)
        {
            _patternString += kvp.Key.ToString();
            _patternString += "|";
        }
        //kiểm tra tính hợp lệ của chuỗi nhập vào
        string filePath = Environment.CurrentDirectory + @"\Patterns\" + hud.saveDialog.patternName.text + ".txt";
        UnityEngine.Debug.Log(filePath);
        if (File.Exists(filePath))
        {
            UnityEngine.Debug.Log("Nope");
        }
        else
        {
            File.WriteAllText(path + "/" + hud.saveDialog.patternName.text + ".txt", _patternString);
        }
        
    }
    void LoadPattern()
    {
        string path = "Patterns";
        if (!Directory.Exists(path))
        {
            return;
        }
        //xóa hết cell ở trên fields
        foreach (KeyValuePair<int, GameObject> kvp in cellContainer)
        {
            GameObject tmp = kvp.Value;
            Destroy(tmp);
            GameObject deleteGO = kvp.Value;
            cellContainer.TryRemove(kvp.Key, out deleteGO);
        }
        string patternName = hud.loadDialog.patternName.options[hud.loadDialog.patternName.value].text;
        string patternString = File.ReadAllText(path + "/" + patternName + ".txt");
        int StringLength = patternString.Length;
        string currentValue = "";
        for(int i = 0; i < StringLength; i++)
        {
            if(patternString[i] == '|')
            {
                int CellValue = int.Parse(currentValue);
                currentValue = "";
                int y = (CellValue - 1) / SCREEN_WIDTH;
                int x = (CellValue - 1) % SCREEN_WIDTH;
                GameObject addGO = Instantiate(Resources.Load("Prefabs/Cell"), new Vector2(x, y), Quaternion.identity) as GameObject;
                cellContainer.TryAdd(CellValue, addGO);
            }
            else
            {
                currentValue += patternString[i];
            }
        }
    }
    #region Debug
    static void printGraph(LinkedList<int>[] adj, int len)
    {
        for (int i = 0; i <= len; i++)
        {
            Console.WriteLine("\nAdjacency list of vertex " + i);
            Console.Write("head");

            foreach (var item in adj[i])
            {
                Console.Write(" -> " + item);
            }
            Console.WriteLine();
        }
    }
    bool randomAliveCell()
    {
        int rand = UnityEngine.Random.Range(0, 100);
        if (rand >= 65)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region Graph
    static void addEdge(LinkedList<int>[] adj, int u, int v)
    {
        adj[u].AddLast(v);
        adj[v].AddLast(u);
    }
    static LinkedList<int>[] importFile(string[] src)
    {
        LinkedList<int>[] adj = new LinkedList<int>[SCREEN_HEIGHT * SCREEN_WIDTH + 1];
        for (int i = 0; i < SCREEN_HEIGHT * SCREEN_WIDTH + 1; i++)
        {
            adj[i] = new LinkedList<int>();
        }
        int maxValue = -1;
        foreach (string line in src)
        {
            int u = 0;
            int v = 0;
            string value = "";
            int breakpoint = 0;
            line.Trim();
            //Console.Write(line+": ");
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\t' && breakpoint == 0)
                {
                    breakpoint++;
                    u = int.Parse(value);
                    if (u > maxValue) maxValue = u;
                    value = "";
                    continue;
                }
                if (line[i] == '\t' && breakpoint == 1)
                //if (i == line.Length - 1) //new cond
                {
                    value += line[i];
                    v = int.Parse(value);
                    value = "";
                    if (v > maxValue) maxValue = v;
                    break;
                }
                value += line[i];
            }
            //Console.WriteLine("u = " + u + ";v = " + v);
            addEdge(adj, u, v);
        }
        return adj;
    }
    #endregion

    bool isAlive(LinkedList<int> node, bool isThisAlive)
    {
        int neighborAlive = 0;
        foreach (var item in node)
        {
            if (cellContainer.ContainsKey(item))
            {
                neighborAlive++;
            }
            if (neighborAlive == 4) return false;
        }
        if (isThisAlive && neighborAlive < 2) return false;
        if (isThisAlive) return true;
        if (!isThisAlive && neighborAlive == 3) return true;
        return false;
    }

    #region Sync
    Dictionary<int, bool> status(int len)
    {
        Dictionary<int, bool> deathNote = new Dictionary<int, bool>();
        for (int i = 1; i <= len; i++)
        {
            //int y = (i - 1) / SCREEN_WIDTH;
            //int x = (i - 1) % SCREEN_WIDTH;
            bool isThisAlive;
            if (cellContainer.ContainsKey(i))
            {
                isThisAlive = true;
            }
            else
            {
                isThisAlive = false;
            }
            LinkedList<int> neighbor = graph.adj[i];
            deathNote.Add(i, isAlive(neighbor, isThisAlive));
        }
        return deathNote;
    }

    void PopulationControl(Dictionary<int, bool> deathNote, int len)
    {
        for (int i = 1; i <= len; i++)
        {
            int y = (i - 1) / SCREEN_WIDTH;
            int x = (i - 1) % SCREEN_WIDTH;
            //grid[x, y].setAlive(deathNote[i]);
            if (deathNote[i])
            {
                if (!cellContainer.ContainsKey(i))
                {
                    GameObject addGO = Instantiate(Resources.Load("Prefabs/Cell"), new Vector2(x, y), Quaternion.identity) as GameObject;
                    cellContainer.TryAdd(i, addGO);
                }
            }
            else
            {
                if (cellContainer.ContainsKey(i))
                {
                    GameObject tmp = cellContainer[i];
                    Destroy(tmp);
                    GameObject deleteGO = cellContainer[i];
                    cellContainer.TryRemove(i, out deleteGO);
                }
            }
        }
    }
    #endregion

    #region Async 
    ConcurrentDictionary<int, bool> handlingWithTask(int len)
    {
        ConcurrentDictionary<int, bool> deathNote = new ConcurrentDictionary<int, bool>();//dict chứa trạng thái của ô
        List<Task> tasks = new List<Task>();
        //tạo các task xử lý song song
        for (int i = 1; i <= len; i++)
        {
            int currentValue = i;
            #region Old Code
            ////lấy x, y tương ứng với giá trị của ô
            ////VD: ô 1 sẽ có tọa độ 0,0
            //// y = (1 - 1) / 64 = 0;
            //// x = (1 - 1) % 48 = 0
            //int y = (i - 1) / SCREEN_WIDTH;
            //int x = (i - 1) % SCREEN_WIDTH;
            //LinkedList<int> neighbor = grid[x, y].neighbor;
            //bool isThisAlive = grid[x, y].getAlive();
            #endregion
            LinkedList<int> neighbor = graph.adj[currentValue];
            bool isThisAlive;
            if (cellContainer.ContainsKey(currentValue))
            {
                isThisAlive = true;
            }
            else
            {
                isThisAlive = false;
            }
            var task = Task.Run(() => deathNote.TryAdd(currentValue, isAlive(neighbor, isThisAlive)));
            tasks.Add(task);
        }
        //đồng bộ các task
        Task.WaitAll(tasks.ToArray());
        return deathNote;
    }
    void _PopulationControl(ConcurrentDictionary<int, bool> deathNote, int len)
    {
        for (int i = 1; i <= len; i++)
        {
            int y = (i - 1) / SCREEN_WIDTH;
            int x = (i - 1) % SCREEN_WIDTH;
            //grid[x, y].setAlive(deathNote[i]);
            if (deathNote[i])
            {
                if (!cellContainer.ContainsKey(i))
                {
                    GameObject addGO = Instantiate(Resources.Load("Prefabs/Cell"), new Vector2(x, y), Quaternion.identity) as GameObject;
                    cellContainer.TryAdd(i, addGO);
                }
            }
            else
            {
                if (cellContainer.ContainsKey(i))
                {
                    GameObject tmp = cellContainer[i];
                    Destroy(tmp);
                    GameObject deleteGO = cellContainer[i];
                    cellContainer.TryRemove(i, out deleteGO);
                }
            }
        }
    }
    #endregion
}
