using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class background : MonoBehaviour
{
    public Transform backgroundLeft;
    public Transform backgroundRight;
    public Transform backgroundNew;
    public Canvas uiCanvas;
    public startgame SQ;

    public float splitDistance = 480f;  
    public float speed = 2f;

    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    void Start()
    {
        leftStartPos = backgroundLeft.position;
        rightStartPos = backgroundRight.position;

        backgroundNew.gameObject.SetActive(true);
    }

    public void StartSplit()
    {
        if (uiCanvas != null)
            uiCanvas.gameObject.SetActive(false);

        StartCoroutine(SplitAndReveal());
    }

    IEnumerator SplitAndReveal()
    {
        float t = 0;
        float duration = 1f / speed;

        Vector3 leftEnd = leftStartPos + Vector3.left * splitDistance;
        Vector3 rightEnd = rightStartPos + Vector3.right * splitDistance;

        while (t < duration)
        {
            t += Time.deltaTime;
            float smooth = Mathf.SmoothStep(0, 1, t / duration);

            backgroundLeft.position = Vector3.Lerp(leftStartPos, leftEnd, smooth);
            backgroundRight.position = Vector3.Lerp(rightStartPos, rightEnd, smooth);

            yield return null;
        }

        Destroy(backgroundLeft.gameObject);
        Destroy(backgroundRight.gameObject);

        SQ.PlayGame();
    }
}
