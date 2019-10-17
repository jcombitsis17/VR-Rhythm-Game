using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCircle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterAnimation(1f));
    }

    IEnumerator DestroyAfterAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
