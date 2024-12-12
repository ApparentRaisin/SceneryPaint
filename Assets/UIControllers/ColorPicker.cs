using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [SerializeField] Button colorButton;
    [SerializeField] List<Color> colors = new List<Color>();

    RectTransform thisTrans;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisTrans = this.GetComponent<RectTransform>();
        
        //Sort Colours in list
        colors.Sort(delegate(Color a, Color b){
            return a.GetInt() - b.GetInt();
        });

        Paint.SetPaintColor(colors[0]);

        //Create buttons in menu according to sorted list
        for(int i = 0; i< colors.Count; i++){
            Button b = (Button)Instantiate(colorButton) as Button;
            b.GetComponent<RectTransform>().SetParent(this.transform, false);
            b.GetComponent<RectTransform>().anchoredPosition = new Vector2(20 + 40 * (i%7), 180 - 40*Mathf.FloorToInt(i/7));
            b.image.color = colors[i];
            b.onClick.AddListener( delegate {Paint.SetPaintColor(b.image.color); });
        }
    }

    Vector3 currentScale;

    public void Open(){
        currentScale = new Vector3(1,1,1);
    }

    public void Close(){
        currentScale = new Vector3(0,0,0);
    }
    // Update is called once per frame
    void Update()
    {
        thisTrans.localScale = Vector3.Lerp(thisTrans.localScale, currentScale, Time.deltaTime * 3.33f);
    }
}

//Setup color extension to support "sorting" colors
public static class ColorExtension{
    public static int GetInt(this Color color){
        return Mathf.FloorToInt(color.b * 255) * 255 * 255 + Mathf.FloorToInt(color.g * 255) * 255 + Mathf.FloorToInt(color.r * 255);
    }
}
