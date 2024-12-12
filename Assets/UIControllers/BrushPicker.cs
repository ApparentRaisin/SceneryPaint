using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrushPicker : MonoBehaviour
{
    [SerializeField] List<Texture2D> brushes = new List<Texture2D>();
    [SerializeField] Button brushButton;

    [SerializeField] RawImage brushImage;

    RectTransform thisTrans;

    private void Start() {
        thisTrans = this.GetComponent<RectTransform>();
        Paint.SetBrushTexture(brushes[0]);
        brushImage.texture = brushes[0];

        //Create buttons in menu according to sorted list
        for(int i = 0; i< brushes.Count; i++){
            Button b = (Button)Instantiate(brushButton) as Button;
            b.GetComponent<RectTransform>().SetParent(this.transform, false);
            b.GetComponent<RectTransform>().anchoredPosition = new Vector2(20 + 40 * (i%7), 180 - 40*Mathf.FloorToInt(i/7));
            b.GetComponentInChildren<RawImage>().texture = brushes[i];
            b.onClick.AddListener( delegate {
                Paint.SetBrushTexture(b.GetComponentInChildren<RawImage>().texture); 
                brushImage.texture = b.GetComponentInChildren<RawImage>().texture; 
            });
        }
    }

    Vector3 currentScale;

    public void Open(){
        currentScale = new Vector3(1,1,1);
    }

    public void Close(){
        currentScale = new Vector3(0,0,0);
    }
    
    void Update()
    {
        thisTrans.localScale = Vector3.Lerp(thisTrans.localScale, currentScale, Time.deltaTime * 3.33f);
    }
}
