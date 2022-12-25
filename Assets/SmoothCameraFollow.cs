using UnityEngine;
using System.Collections;

public class SmoothCameraFollow : MonoBehaviour
{
    public Transform Target;
    public float Speed = 2;

    Vector3 _delta;

	void Awake ()
    {
        _delta = transform.position - Target.position;
	}

    void Start()
    {
        transform.position = Target.position + _delta;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target.position + _delta, Time.deltaTime * Speed);
	}
}
