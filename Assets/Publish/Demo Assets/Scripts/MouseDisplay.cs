using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Kit.Extend;

public class MouseDisplay : MonoBehaviour
{
    public Image ImageUp;
    public Image ImageDown;
    public Image ImageLeft;
    public Image ImageRight;
    public Image ImageWheel;
    public float Threshold = 0.1f;

	Color On = Color.white;
	Color Off = new Color(1f, 1f, 1f, .3f);

    float mouseHorizontal;
    float mouseVertical;
    float mouseScrollWheel;
    void OnEnable()
    {
        ImageUp.color = Off;
        ImageDown.color = Off;
        ImageLeft.color = Off;
        ImageRight.color = Off;
        ImageWheel.color = Off;
        StartCoroutine(MouseScroll());
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void Update ()
    {
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");
        mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

        // Left/Right
        if (Mathf.Abs(mouseHorizontal) > Threshold)
        {
            ImageRight.color = (mouseHorizontal > 0f) ?
                On :
                Off;
            ImageLeft.color = (mouseHorizontal < 0f) ?
                On :
                Off;
        }
        else
        {
            ImageRight.color = Off;
            ImageLeft.color = Off;
        }

        // Up/Down
        if (Mathf.Abs(mouseVertical) > Threshold)
        {
            ImageUp.color = (mouseVertical > 0f) ?
                On :
                Off;
            ImageDown.color = (mouseVertical < 0f) ?
                On :
                Off;
        }
        else
        {
            ImageUp.color = Off;
            ImageDown.color = Off;
        }

	}

    // Wheel
    private IEnumerator MouseScroll()
    {
        float ScrollDegree = 0f;
        float currDegree = ScrollDegree = 0f;
        while(true)
        {
            if(Mathf.Abs(mouseScrollWheel) > 0f)
            {
                ImageWheel.color = On;
                ScrollDegree = (mouseScrollWheel > 0f) ? 10f : -10f;
                currDegree = mouseScrollWheel = 0f;
                while (!currDegree.EqualRoughly(ScrollDegree, .1f))
                {
                    currDegree = Mathf.Lerp(currDegree, ScrollDegree, 0.1f);
                    ImageWheel.rectTransform.Rotate(Vector3.forward, currDegree);
                    yield return new WaitForEndOfFrame();
                }
            }
            ImageWheel.color = Off;
            yield return new WaitForEndOfFrame();
        }
    }
}
