
using UnityEngine;
using System.Collections;
public class CameraMover : MonoBehaviour
{
    public GameObject target;
    private Vector3 distance;
    // Update is called once per frame
    private void Start()
    {
        distance = transform.position - target.transform.position;
    }

    void Update()
    {
        transform.position = target.transform.position + distance;
    }
    IEnumerator Shaking(float duration, AnimationCurve curve){
        float startTime = Time.time;
        while(Time.time - startTime <= duration){
            float power = curve.Evaluate((Time.time - startTime)/duration);
            transform.position += Random.insideUnitSphere*power;
            yield return null;
        }
    }
    public void ScreenShake(float duration, AnimationCurve curve){
        StartCoroutine(Shaking(duration, curve));
    }

}
