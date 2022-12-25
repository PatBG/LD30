using UnityEngine;
using System.Collections;

public class BadGuy1 : MonoBehaviour
{
    public bool IsRegular = true;
    public float Speed = 2;
    public float RotationSpeed = 360;
    public AudioClip SoundBadGuyDie;

    Ground _ground;
    Vector3 _destinationPosition;
    float _destinationAngleY;

    int _x;
    int _z;

    bool _isDead = false;

    void Awake()
    {
        _ground = GameObject.FindObjectOfType(typeof(Ground)) as Ground;
    }

    // Use this for initialization
    void Start()
    {
    }

    public void ForcePosition(int x, int z)
    {
        _x = x;
        _z = z;
        transform.position = _ground.CellPos(_x, _z);
        _destinationPosition = transform.position;
        _destinationAngleY = 0;
    }

    public void ForceRotation(int angleY)
    {
        _destinationAngleY = angleY;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, angleY, transform.rotation.eulerAngles.z);
    }
	
	// Update is called once per frame
    void Update()
    {
        if (_isDead)
            return;

        if (transform.position == _destinationPosition)
        {
            int sideX = 0;
            int sideZ = 0;
            int frontX = 0;
            int frontZ = 0;

            if (Mathf.Approximately(_destinationAngleY, 90))    // Up
            {
                sideX = IsRegular ? 1 : - 1;
                frontZ = 1;
            }
            else if (Mathf.Approximately(_destinationAngleY, 270))    // Down
            {
                sideX = IsRegular ? -1 : 1;
                frontZ = -1;
            }
            else if (Mathf.Approximately(_destinationAngleY, 0))    // Left
            {
                sideZ = IsRegular ? 1 : -1;
                frontX = -1;
            }
            else if (Mathf.Approximately(_destinationAngleY, 180))    // Right
            {
                sideZ = IsRegular ? -1 : 1;
                frontX = 1;
            }
            else
            {
                Debug.LogError("_destinationAngleY = " + _destinationAngleY + " transform.rotation.eulerAngles = " + transform.rotation.eulerAngles);
            }

            bool sideIsEmpty = (_ground.GetBlockID(_x + sideX, _z + sideZ) == 0);
            bool frontIsEmpty = (_ground.GetBlockID(_x + frontX, _z + frontZ) == 0);

            //Debug.Log("sideIsEmpty = " + sideIsEmpty + ", frontIsEmpty = " + frontIsEmpty);

            if (sideIsEmpty == true)
            {
                // Turn to side
                _destinationAngleY += IsRegular ? 90 : -90;

                // And move to side
                _x += sideX;
                _z += sideZ;
                _destinationPosition = _ground.CellPos(_x, _z);
            }
            else
            {
                if (frontIsEmpty == true)
                {
                    // Move front
                    _x += frontX;
                    _z += frontZ;
                    _destinationPosition = _ground.CellPos(_x, _z);
                }
                else
                {
                    // Turn to other side
                    _destinationAngleY += IsRegular ? -90 : 90;
                }
            }
            _destinationAngleY = (Mathf.Round(_destinationAngleY) + 360) % 360;
        }

        if (transform.rotation.eulerAngles.y != _destinationAngleY)
        {
            float angleY = transform.rotation.eulerAngles.y;
            float destinationAngleY = _destinationAngleY;
            if (Mathf.Abs(angleY - destinationAngleY) > 180)
            {
                if (angleY < destinationAngleY)
                    angleY += 360;
                else
                    destinationAngleY += 360;
            }
            angleY = Mathf.MoveTowards(angleY, destinationAngleY, Time.deltaTime * RotationSpeed);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, angleY, transform.rotation.eulerAngles.z);
        }

        if (transform.position != _destinationPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destinationPosition, Time.deltaTime * Speed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (_isDead)
            return;

        if (other.name.Contains("BadGuy"))
        {
            // TODO : implement behaviour for collision between bad guys
        }
        else if (other.name.Contains("Block"))
        {
            Debug.Log(this.name + " was crunched by a block");
            OnDie();
        }
    }

    public void OnDie()
    {
        _isDead = true;
        GetComponent<Collider>().enabled = false;
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        GetComponent<AudioSource>().PlayOneShot(SoundBadGuyDie);

        Vector3 rotation = new Vector3(0, 0, IsRegular ? 60 : -60);
        float end = Time.time + 5;

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
