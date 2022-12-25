using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum CellType
{
    Empty,
    Block,
    Obstacle,
    StartPylon,
    EndPylon
};

public class Ground : MonoBehaviour
{
    public float CellSize = 1;

    public GameObject CellBlock;
    public GameObject CellObstacle;
    public GameObject CellPylonOn;
    public GameObject CellPylonOff;
    public GameObject CellGround;
    public GameObject CellBorder;
    public GameObject BadGuy1Goofy;
    public GameObject BadGuy1Regular;

    public AudioClip SoundWin;

    int _sizeX;
    int _sizeZ;

    int _startPylonX;
    int _startPylonZ;

    int _endPylonX;
    int _endPylonZ;

    public float TimeStartLevel = float.MaxValue;

    public bool IsActive { get { return TimeStartLevel != float.MaxValue; } }
    /// <summary>
    /// 0 = empty
    /// 1 = block (for link)
    /// 2 = obstacle
    /// 3 = start pylon
    /// 4 = end pylon
    /// </summary>
    int[,] Level1 = new int[10, 10]
    {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 3, 0, 1, 0, 1, 1, 0, 1, 0},
        {0, 0, 0, 0, 0, 1, 0, 0, 1, 0},
        {0, 1, 1, 1, 1, 1, 0, 1, 1, 0},
        {0, 1, 0, 0, 0, 1, 0, 0, 0, 0},
        {0, 1, 0, 1, 0, 0, 0, 1, 1, 0},
        {0, 0, 0, 1, 1, 1, 0, 0, 1, 0},
        {0, 1, 0, 1, 0, 0, 0, 0, 1, 0},
        {0, 1, 0, 1, 0, 1, 1, 0, 4, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    int[,] Level2 = new int[12, 12]
    {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 4, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0},
        {0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0},
        {0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0},
        {0, 1, 0, 0, 0, 2, 0, 0, 0, 0, 1, 0},
        {0, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 0},
        {0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 1, 0},
        {0, 1, 0, 1, 0, 0, 0, 0, 1, 1, 1, 0},
        {0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0},
        {0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 3, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };

    int[,] Level3 = new int[14, 14]
    {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 3, 2, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0},
        {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0},
        {0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
        {0, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0},
        {0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0},
        {0, 1, 0, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0},
        {0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
        {0, 1, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0},
        {0, 1, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0},
        {0, 1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0},
        {0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 4, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    };

    int[,] Level4 = new int[16, 16]
    {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 4, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 0},
        {0, 2, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0},
        {0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 0},
        {0, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0},
        {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 0},
        {0, 1, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 1, 1, 1, 0},
        {0, 1, 1, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0},
        {0, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 0},
        {0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0},
        {0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0},
        {0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0},
        {0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1, 0},
        {0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 2, 0},
        {0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 3, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    };

    int[][,] _levels;
    int _currentLevel;

    int[,] _level;

    bool _levelIsWin = false;

    public static Transform LevelRoot = null;

    void Awake()
    {
        _levels = new int[][,]
        {
            Level1,
            Level2,
            Level3,
            Level4,
        };

        _currentLevel = 0;
        InitLevel();
    }

	// Use this for initialization
	void Start () 
    {
        StartLevel();
    }

    public void StartLevel()
    {
        TimeStartLevel = Time.time;
    }

    public Vector3 CellPos(int x, int z)
    {
        return LevelRoot.position + new Vector3(x * CellSize, 0, z * CellSize);
    }

    public int GetBlockID(int x, int z)
    {
        if (x < 0 || x >= _sizeX || z < 0 || z >= _sizeZ)
        {
            return -1;
        }

        return _level[x, z];
    }

    public void SetBlockID(int x, int z, int id)
    {
        if (x < 0 || x >= _sizeX || z < 0 || z >= _sizeZ)
        {
            Debug.LogError("invalid coordinates [" + x + "," + z + "]");
        }

        _level[x, z] = id;
    }

    public Block GetBlock(int x, int z)
    {
        if (GetBlockID(x, z) <= 0)
        {
            return null;
        }

        Vector3 cellPos = CellPos(x, z);

        foreach(Block block in LevelRoot.GetComponentsInChildren<Block>())
        {
            if (block.transform.position == cellPos)
            {
                return block;
            }
        }

        return null;
    }

    void InitLevel()
    {
        TimeStartLevel = float.MaxValue;

        float positionY = _currentLevel * 20;
        _level = (int[,])_levels[_currentLevel % _levels.Length].Clone();

        LevelRoot = new GameObject("LevelRoot" + _currentLevel).transform;
        LevelRoot.transform.parent = transform;
        Vector3 position = LevelRoot.position;
        position.y = positionY;
        LevelRoot.position = position;

        _sizeX = _level.GetLength(1);
        _sizeZ = _level.GetLength(0);

        GameObject cellObject;
        cellObject = Instantiate(CellGround, CellPos(_sizeX / 2, _sizeZ / 2) + new Vector3(-CellSize / 2, -CellSize, -CellSize / 2), Quaternion.identity) as GameObject;
        cellObject.transform.localScale = new Vector3(_sizeX, 1, _sizeZ);
        cellObject.transform.parent = LevelRoot;

        cellObject = Instantiate(CellBorder, CellPos(_sizeX / 2, -1) + new Vector3(-CellSize / 2, 0, 0), Quaternion.identity) as GameObject;
        cellObject.transform.localScale = new Vector3(_sizeX + 2, 1, 1);
        cellObject.transform.parent = LevelRoot;

        cellObject = Instantiate(CellBorder, CellPos(_sizeX / 2, _sizeZ) + new Vector3(-CellSize / 2, 0, 0), Quaternion.identity) as GameObject;
        cellObject.transform.localScale = new Vector3(_sizeX + 2, 1, 1);
        cellObject.transform.parent = LevelRoot;

        cellObject = Instantiate(CellBorder, CellPos(-1, _sizeZ / 2) + new Vector3(0, 0, -CellSize / 2), Quaternion.identity) as GameObject;
        cellObject.transform.localScale = new Vector3(1, 1, _sizeZ + 2);
        cellObject.transform.parent = LevelRoot;

        cellObject = Instantiate(CellBorder, CellPos(_sizeZ, _sizeZ / 2) + new Vector3(0, 0, -CellSize / 2), Quaternion.identity) as GameObject;
        cellObject.transform.localScale = new Vector3(1, 1, _sizeZ + 2);
        cellObject.transform.parent = LevelRoot;

        Random.InitState(_currentLevel + 666);
        for (int x = 0; x < _sizeX; x++)
        {
            for (int z = 0; z < _sizeZ; z++)
            {
                switch ((CellType)_level[x, z])
                {
                    case CellType.Empty :
                        cellObject = null;
                        break;
                    case CellType.Block :
                        cellObject = CellBlock;
                        break;
                    case CellType.Obstacle:
                        cellObject = CellObstacle;
                        break;
                    case CellType.StartPylon:
                        _startPylonX = x;
                        _startPylonZ = z;
                        cellObject = CellPylonOn;
                        break;
                    case CellType.EndPylon:
                        _endPylonX = x;
                        _endPylonZ = z;
                        cellObject = CellPylonOff;
                        break;
                    default:
                        Debug.LogError("Unknow cell value _level[" + x + "," + z + "] = "+ _level[x, z]);
                        cellObject = null;
                        break;
                }

                if (cellObject != null)
                {
                    cellObject = Instantiate(cellObject, CellPos(x, z), Quaternion.identity) as GameObject;
                    Block block = cellObject.GetComponent<Block>();
                    if (block != null)
                    {
                        // initialize internal block grid position
                        block.SetDestinationCell(x, z);

                        if (Random.Range(0, 100) < Mathf.Min(5 + _currentLevel, 50))
                        {
                            block.BadGuy = (Random.Range(0, 100) < 50) ? BadGuy1Regular : BadGuy1Goofy;
                            block.BadGuyDelay = 5 * Random.Range(1, 5); //Mathf.Min(2, 100 - _currentLevel));
                        }
                    }
                    cellObject.transform.parent = LevelRoot;
                }
            }
        }

        // Move the player to the starting position
        Player player = transform.Find("/Player").GetComponent<Player>();
        if (_currentLevel % 2 == 0)
            player.ForcePosition(0, 0);
        else
            player.ForcePosition(_sizeX - 1, _sizeZ - 1);
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

        RefreshOnOff();
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    public void RefreshOnOff()
    {
        // Initialize all blocks to OFF
        for (int x = 0; x < _sizeX; x++)
        {
            for (int z = 0; z < _sizeZ; z++)
            {
                Block block = GetBlock(x, z);
                if (block != null)
                     block.IsOn = false;
            }
        }

        Pylon endPylon = LevelRoot.GetComponentInChildren<Pylon>();
        endPylon.IsOn = false;

        _levelIsWin = false;

        // Set recursively block to ON from the start pylon
        SetOnRecursively(_startPylonX, _startPylonZ);

            // Initialize all blocks to OFF
        for (int x = 0; x < _sizeX; x++)
        {
            for (int z = 0; z < _sizeZ; z++)
            {
                Block block= GetBlock(x, z);
                if (block != null)
                     block.RefreshOnOff();
            }
        }

        endPylon.RefreshOnOff();

        if (_levelIsWin)
        {
            StartCoroutine(WinLevel());
        }
    }

    void SetOnRecursively(int x, int z)
    {
        SetOnRecursivelySub(x - 1, z);
        SetOnRecursivelySub(x + 1, z);
        SetOnRecursivelySub(x, z - 1);
        SetOnRecursivelySub(x, z + 1);
    }

    void SetOnRecursivelySub(int x, int z)
    {
        if (x == _endPylonX && z == _endPylonZ)
        {
            Pylon endPylon = LevelRoot.GetComponentInChildren<Pylon>();
            endPylon.IsOn = true;
            _levelIsWin = true;
            SetOnRecursively(x, z);
        }
        else
        {
            Block block = GetBlock(x, z);
            if (block != null && block.IsOn == false)
            {
                block.IsOn = true;
                SetOnRecursively(x, z);
            }
        }
    }

    IEnumerator WinLevel()
    {
        Debug.Log("Win level!!!!!!!!!!!!!!");
        TimeStartLevel = float.MaxValue;
        
        Pylon endPylon = LevelRoot.GetComponentInChildren<Pylon>();
        endPylon.ExtendTip(20, 3);

        transform.Find("/Player").GetComponent<Player>().ForcePosition(_endPylonX, _endPylonZ);

        //LevelRoot.BroadcastMessage("OnDie", SendMessageOptions.DontRequireReceiver);
        foreach (BadGuy1 badGuy1 in LevelRoot.GetComponentsInChildren<BadGuy1>())
        {
            Destroy(badGuy1.gameObject);
        }

        GetComponent<AudioSource>().PlayOneShot(SoundWin, 1);
        yield return new WaitForSeconds(SoundWin.length);
        GetComponent<AudioSource>().PlayOneShot(SoundWin, 0.9f);
        yield return new WaitForSeconds(SoundWin.length);
        GetComponent<AudioSource>().PlayOneShot(SoundWin, 0.8f);
        yield return new WaitForSeconds(SoundWin.length);
        GetComponent<AudioSource>().PlayOneShot(SoundWin, 0.7f);
        yield return new WaitForSeconds(SoundWin.length);

        Transform previousLevelRoot = LevelRoot;
        _currentLevel++;
        InitLevel();

        yield return new WaitForSeconds(6);

        Destroy(previousLevelRoot.gameObject);

        StartLevel();
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 140, 80), "Level " + (_currentLevel + 1));

        if (GUI.Button(new Rect(20, 35, 120, 20), "Restart at level 1"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (GUI.Button(new Rect(20, 60, 120, 20), "Restart this level"))
        {
            RestartLevel();
            StartCoroutine(StartLevelAsync(2));
        }
    }

    public void RestartLevel()
    {
        Destroy(LevelRoot.gameObject);
        InitLevel();
    }

    IEnumerator StartLevelAsync(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartLevel();
    }
}
