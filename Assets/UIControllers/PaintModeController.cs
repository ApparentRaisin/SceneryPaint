using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PaintModeController : MonoBehaviour
{
    [SerializeField] KeyCode paintKey;
    public static bool isPainting = false;
    [SerializeField] RawImage brush;
    Paint paintScript;
    FrustrumCollect meshCollect;
    [SerializeField] GameObject paintCanvas;

    CanvasScaler scaler;
    [SerializeField]float maxSize;

    public static UnityEvent paintingToggle = new UnityEvent();

    //Listed framerates for target framerate
    [SerializeField] List<int> frameRate = new List<int>();
    
    [SerializeField] int selectedFramerate = 0;
    int currentFramerate =-1;

    void Start()
    {
        paintCanvas = this.transform.GetChild(0).gameObject;
        scaler = paintCanvas.GetComponent<CanvasScaler>();
        paintScript = this.GetComponent<Paint>();
        paintingToggle.AddListener(SetPaint);
        meshCollect = this.GetComponent<FrustrumCollect>();
    }

    //painting toggle
    void SetPaint(){
        paintCanvas.SetActive(isPainting);
        paintScript.enabled = isPainting;
        meshCollect.enabled = isPainting;
    }

    void Update()
    {
        //Set framerate
        if(currentFramerate != selectedFramerate)
        {
            Application.targetFrameRate = frameRate[selectedFramerate];
            currentFramerate = selectedFramerate;
        }

        //toggle painting mode
        if(Input.GetKeyDown(paintKey)){
            isPainting = !isPainting;
            paintingToggle.Invoke();
        }

        if(isPainting){
            //set brush scale
            Paint.scale += Input.mouseScrollDelta.y*8;
            Paint.scale = Mathf.Clamp(Paint.scale,1,400);
            Vector2 screen = new Vector2(800,(9f/16f)*800);
            brush.rectTransform.anchoredPosition = new Vector2((Input.mousePosition.x/Screen.width)*screen.x,(Input.mousePosition.y/Screen.height)*screen.y);
            brush.rectTransform.sizeDelta = (Paint.scale*Vector2.one);

            //set brush color
            brush.color = Paint.paintColor;
        }
        
    }
}
