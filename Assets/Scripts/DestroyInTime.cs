using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInTime : MonoBehaviour
{
    public bool singleFrame = false;
    public float destroyTime = 1f;
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(DestroyRoutine());
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    IEnumerator DestroyRoutine()
    {
        if (singleFrame)
        {
            yield return null;
            yield return new WaitForEndOfFrame();
        }
        else
        {
            yield return new WaitForSeconds(destroyTime);
        }
        Destroy(gameObject);
    }
}
