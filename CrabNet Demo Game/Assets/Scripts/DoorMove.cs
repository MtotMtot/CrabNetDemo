using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMove : MonoBehaviour
{
    [SerializeField]
    private GameObject leftDoor;
    [SerializeField]
    private GameObject rightDoor;

    [SerializeField]
    private Transform leftEndPos;
    [SerializeField]
    private Transform rightEndPos;

    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        leftStartPos = leftDoor.transform.position;
        rightStartPos = rightDoor.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // open door fucntion referenced by logic manager
    public void OpenDoor()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveDoors());
        }
    }

    // move doors corotine, move doors to end position over time.
    private IEnumerator MoveDoors()
    {
        isMoving = true;
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            leftDoor.transform.position = Vector3.Lerp(leftStartPos, leftEndPos.position, t);
            rightDoor.transform.position = Vector3.Lerp(rightStartPos, rightEndPos.position, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure doors reach exact end positions
        leftDoor.transform.position = leftEndPos.position;
        rightDoor.transform.position = rightEndPos.position;
        isMoving = false;
    }
}
