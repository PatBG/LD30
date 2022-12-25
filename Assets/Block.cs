using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
    public GameObject BadGuy = null;
    public float BadGuyDelay = 5;
    public Material MaterialGoofy;
    public Material MaterialRegular;
    public AudioClip SoundSpawnBadGuy;

    public Material MaterialOn;
    public Material MaterialOff;

    public float Speed = 10;

    Vector3 _destination;
    Ground _ground;

    public bool IsOn = false;

    bool _isDead = false;

    int _x;
    int _z;

    void Awake()
    {
        _ground = GameObject.FindObjectOfType(typeof(Ground)) as Ground;
    }

	// Use this for initialization
	void Start ()
    {
        if (BadGuy != null)
        {
            StartCoroutine(BagGuyWarning());
        }
	}

    IEnumerator BagGuyWarning()
    {
        while (_ground.TimeStartLevel > Time.time)
        {
            yield return 0;
        }

        BadGuy1 badGuy1 = BadGuy.GetComponent<BadGuy1>();
        Material materialBadGuy = badGuy1.IsRegular ? MaterialRegular : MaterialGoofy;

        float beat = 1; // BadGuyDelay / 25;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        while (_ground.TimeStartLevel + (2 * beat) * (BadGuyDelay / 5) > Time.time)
        {
            Material initialMaterial = IsOn ? MaterialOn : MaterialOff;
            meshRenderer.material.Lerp(initialMaterial, materialBadGuy, Mathf.PingPong(Time.time - _ground.TimeStartLevel, beat));
            yield return 0;
        }

        yield return new WaitForSeconds((_ground.TimeStartLevel + BadGuyDelay - (1.5f * beat)) - Time.time);

        while (_ground.TimeStartLevel + BadGuyDelay > Time.time)
        {
            Material initialMaterial = IsOn ? MaterialOn : MaterialOff;
            meshRenderer.material.Lerp(initialMaterial, materialBadGuy, Mathf.PingPong(Time.time - _ground.TimeStartLevel, beat / 2f));
            yield return 0;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (_isDead)
            return;

        if (_ground.IsActive == false)
            return;

        if (transform.position != _destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destination, Time.deltaTime * Speed);
        }
        else
        {
            if (BadGuy != null)
            {
                if (_ground.TimeStartLevel + BadGuyDelay <= Time.time)
                {
                    GameObject cellObject = Instantiate(BadGuy) as GameObject;
                    cellObject.transform.parent = transform.parent;
                    BadGuy1 badGuy1 = cellObject.GetComponent<BadGuy1>();
                    badGuy1.ForcePosition(_x, _z);

                    if (badGuy1.IsRegular)
                    {
                        if (_ground.GetBlockID(_x, _z + 1) == 0)
                        {
                            badGuy1.ForceRotation(90);
                            if (_ground.GetBlockID(_x + 1, _z) == 0)
                            {
                                badGuy1.ForceRotation(180);
                                if (_ground.GetBlockID(_x, _z - 1) == 0)
                                {
                                    badGuy1.ForceRotation(270);
                                    if (_ground.GetBlockID(_x - 1, _z) == 0)
                                    {
                                        Debug.Log(cellObject.name + " turn on place (no blocks)");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_ground.GetBlockID(_x, _z - 1) == 0)
                        {
                            badGuy1.ForceRotation(90);
                            if (_ground.GetBlockID(_x - 1, _z) == 0)
                            {
                                badGuy1.ForceRotation(180);
                                if (_ground.GetBlockID(_x, _z + 1) == 0)
                                {
                                    badGuy1.ForceRotation(270);
                                    if (_ground.GetBlockID(_x + 1, _z) == 0)
                                    {
                                        Debug.Log(cellObject.name + " turn on place (no blocks)");
                                    }
                                }
                            }
                        }
                    }

                    GetComponent<AudioSource>().PlayOneShot(SoundSpawnBadGuy);
                    Debug.Log(this.name + " has spawned " + cellObject.name);
                    _ground.SetBlockID(_x, _z, 0);
                    _ground.RefreshOnOff();
                    _isDead = true;
                    GetComponent<Collider>().enabled = false;
                    StartCoroutine(Die());
                }
            }
        }
	}

    public void SetDestinationCell(int x, int z)
    {
        _x = x;
        _z = z;
        _destination = _ground.CellPos(x, z);
    }

    public void RefreshOnOff()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = IsOn ? MaterialOn : MaterialOff;
    }

    IEnumerator Die()
    {
        Vector3 rotation = new Vector3(0, 90, 0);
        float end = Time.time + 3;

        while (Time.time < end)
        {
            transform.Rotate(rotation * Time.deltaTime, Space.World);
            rotation = rotation - rotation * (0.9f * Time.deltaTime);
            transform.Translate(transform.up * 10 * Time.deltaTime, Space.World);
            yield return 0;
        }

        Destroy(gameObject);
    }
}
