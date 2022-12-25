using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float Speed = 10;
    public AudioClip SoundPlayerDie;
    public AudioClip SoundPushBlock;
    public AudioClip SoundBlockedBlock;

    Ground _ground;

    Vector3 _destination;

    int _x;
    int _z;

    bool _isDead = false;

    void Awake()
    {
        _ground = GameObject.FindObjectOfType(typeof(Ground)) as Ground;
    }

	// Use this for initialization
	void Start ()
    {
        ForcePosition(0, 0);
	}

    public void ForcePosition(int x, int z)
    {
        _x = x;
        _z = z;
        transform.position = _ground.CellPos(_x, _z);
        _destination = transform.position;
    }

	// Update is called once per frame
	void Update ()
    {
        if (_isDead)
            return;

        if (_ground.IsActive == false)
            return;

        if (transform.position == _destination)
        {

            int xx = 0;
            int zz = 0;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                zz = 1;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                zz = -1;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                xx = -1;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                xx = 1;
            }

            if (xx != 0 || zz != 0)
            {
                bool moveIsOK = (_ground.GetBlockID(_x + xx, _z + zz) == 0);

                // If it is a block
                if (_ground.GetBlockID(_x + xx, _z + zz) == 1)
                {
                    // And if there is an empty cell behind it
                    if (_ground.GetBlockID(_x + (xx * 2), _z + (zz * 2)) == 0)
                    {
                        // we move the block
                        Block block = _ground.GetBlock(_x + xx, _z + zz);
                        if (block != null)
                        {
                            GetComponent<AudioSource>().PlayOneShot(SoundPushBlock);

                            _ground.SetBlockID(_x + xx, _z + zz, 0);
                            _ground.SetBlockID(_x + (xx * 2), _z + (zz * 2), 1);
                            block.SetDestinationCell(_x + (xx * 2), _z + (zz * 2));
                            moveIsOK = true;

                            StartCoroutine(GroundRefreshOnOffAsync(_ground.CellSize / block.Speed));
                        }
                    }
                }

                if (moveIsOK)
                {
                    _x += xx;
                    _z += zz;
                    float angleY = (zz * 90) + (Mathf.Abs(90 * xx) + xx * 90);
                    transform.rotation = Quaternion.Euler(0, angleY, 0);
                    _destination = _ground.CellPos(_x, _z);
                }
                else
                {
                    GetComponent<AudioSource>().PlayOneShot(SoundBlockedBlock);
                }
            }
        }

	    if (transform.position != _destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destination, Time.deltaTime * Speed);
        }
	}

    IEnumerator GroundRefreshOnOffAsync(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // Refresh on the next frame
        _ground.RefreshOnOff();
    }

    void OnTriggerEnter(Collider other)
    {
        if (_isDead)
            return;

        if (other.name.Contains("BadGuy"))
        {
            Debug.Log(this.name + " killed by " + other.name);
            _isDead = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        GetComponent<AudioSource>().PlayOneShot(SoundPlayerDie);

        Vector3 rotation = new Vector3(-60, 180, 180);
        
        float duration = Time.time + 2;
        while (Time.time < duration)
        {
            transform.Rotate(rotation * Time.deltaTime, Space.World);
            //rotation = rotation - rotation * (0.9f * Time.deltaTime);
            transform.Translate(transform.up * 10 * Time.deltaTime, Space.World);
            yield return 0;
        }

        _ground.RestartLevel();

        yield return new WaitForSeconds(3);

        _ground.StartLevel();

        GetComponent<Collider>().enabled = true;
        _isDead = false;

        //Destroy(gameObject);
    }
}
