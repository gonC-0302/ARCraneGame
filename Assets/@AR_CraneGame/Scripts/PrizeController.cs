using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeController : MonoBehaviour
{
    private Vector3 _originPos;

    void Start()
    {
        _originPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            StartCoroutine(Reset());
        }
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(3);
        gameObject.transform.position = _originPos;
        gameObject.transform.rotation = Quaternion.identity;
    }
}
