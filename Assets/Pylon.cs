using UnityEngine;
using System.Collections;

public class Pylon : MonoBehaviour 
{
    public Material MaterialOn;
    public Material MaterialOff;

    public bool IsOn = false;

	// Use this for initialization
	void Start ()
    {	
	}
	
	// Update is called once per frame
	void Update ()
    {	
	}

    public void RefreshOnOff()
    {
        foreach (MeshRenderer meshRenderer2 in GetComponentsInChildren<MeshRenderer>())
        {
            meshRenderer2.material = IsOn ? MaterialOn : MaterialOff;
        }
    }

    public void ExtendTip(float heightMax, float heightPerSec)
    {
        StartCoroutine(ExtendTipAsync(heightMax, heightPerSec));
    }

    /// <summary>
    /// Extend the tip of the pylon
    /// </summary>
    /// <param name="heightMax"></param>
    /// <param name="heightPerSec"></param>
    /// <returns></returns>
    IEnumerator ExtendTipAsync(float heightMax, float heightPerSec)
    {
        Transform tip = transform.Find("Tip");
        if (tip != null)
        {
            // Substract the height of the base of the pylon
            heightMax -= 2;
            while (tip.transform.localScale.y < heightMax)
            {
                Vector3 scale = tip.transform.localScale;
                scale.y = Mathf.MoveTowards(scale.y, heightMax, heightPerSec * Time.deltaTime);
                tip.transform.localScale = scale;
                yield return 0;
            }
        }
    }
}
