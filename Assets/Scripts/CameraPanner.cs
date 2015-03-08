using UnityEngine;
using System.Collections;

public class CameraPanner : MonoBehaviour
{
    public void PanCamera(Vector3 newPos, float speed = 0.0f)
    {
        StopAllCoroutines();

        if (speed == 0.0f)
        {
            transform.position = newPos;
            return;
        }

        StartCoroutine(PanCameraRoutine(newPos, speed));
    }

	private IEnumerator PanCameraRoutine(Vector3 targetPos, float speed)
    {
        Vector3 originalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        float timer = speed;

        while (timer > 0.0f)
        {
            timer -= Time.deltaTime;

            transform.position = Vector3.Lerp(originalPos, targetPos, 1.0f - (timer / speed));
            yield return new WaitForEndOfFrame();
        }

        transform.position = targetPos;
        yield return null;
    }
}
