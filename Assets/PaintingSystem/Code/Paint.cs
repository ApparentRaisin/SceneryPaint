using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public class Paint : MonoBehaviour
{
    Camera thisCamera;

    //scale of frustrum collect
    public static float scale = 100;

    //screen space brush size
    public float brushSize = 10f;
    RenderTexture screenSpaceTexture, growScreen;
    Material screenSpaceMat, drawMat, growMat;
    Camera screenPositionCamera;
    Collider[] colliders;
    public static Color paintColor;
    public static Texture2D paintTex;

    
    void Start()
    {
        //Cache important Data
        thisCamera = this.GetComponent<Camera>();
        screenPositionCamera = (Camera)Instantiate(Resources.Load<Camera>("ScreenPositionCamera")) as Camera;
        drawMat = new Material(Shader.Find("Hidden/Draw"));
        growMat = new Material(Shader.Find("Hidden/Grow"));
        screenSpaceMat = new Material(Shader.Find("Unlit/ScreenSpace"));

        //setup screen pos texture and camera
        screenSpaceTexture = new RenderTexture(2048, 2048, 16);
        screenSpaceTexture.format = RenderTextureFormat.ARGB64;
        screenPositionCamera.targetTexture = screenSpaceTexture;
        screenPositionCamera.transform.position = Vector3.zero;
        screenPositionCamera.transform.rotation = Quaternion.identity;
        screenPositionCamera.transform.localScale = Vector3.one;

        //Cannot use a temporary texture for margin grow texture because of resolution issues
        growScreen = new RenderTexture(2048, 2048, 16);
        growScreen.format = RenderTextureFormat.ARGB64;
        
    }

    public static void SetPaintColor(Color color){
        paintColor = color;
    }

    public static void SetBrushTexture(Texture brush){
        paintTex = (Texture2D)brush as Texture2D;
    }

    bool paint = false;
    // Update is called once per frame
    void Update()
    {
        //Collect all colliders that mouse is over while clicking
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            colliders = FrustrumCollect.colliders.ToArray();//.transform;
            paint = true;
        }


    }
    Material mat;
    Vector2 lastMouse, dir;
    
    //Keep drawing functions in OnRenderImage as running them in update creates weird artifacts, is just safer this way
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //set draw material informatio nfor this pass of the draw function
        Vector2 mPos = Input.mousePosition;
        drawMat.SetVector("_MousePosition", new Vector4(mPos.x, mPos.y, dir.x, dir.y));

        //setting direction info to rotate brush
        dir = Vector2.Lerp(dir, (lastMouse-mPos).normalized, Time.deltaTime);
        if(Vector2.Distance(lastMouse,mPos) > 1)
        {
            lastMouse = mPos;
        }
            
        //Run the paint "loop" sending each collected collider to the draw function    
        if (paint) {
            foreach (Collider collider in colliders)
            {
                DrawMeshTex(collider);
            }
            paint = false;
        }
        
        //Draw the screen as normal
        Graphics.Blit(source, destination);

    }

    private void OnDisable()
    {
        screenSpaceTexture.Release();
        growScreen.Release();
    }
    void DrawMeshTex(Collider current)
    {
        //Early exit from objects that dont have renders
        //TODO: early exit from objects that are completly occluded AABB test?
        if(current.GetComponent<MeshRenderer>() == null){
            return;
        }
        //create paint texture cache if none exists
        if (current.TryGetComponent<PaintCache>(out PaintCache cache) != true)
        {
            cache = current.AddComponent<PaintCache>();
        }
        
        //collect mesh/transforms and render to custom texture for screen space 
        Mesh m = current.GetComponent<MeshFilter>().sharedMesh;
        Matrix4x4 mat = current.transform.localToWorldMatrix;
        
        RenderTexture.active = screenSpaceTexture;
        screenSpaceMat.SetVector("_CameraDirection", thisCamera.transform.forward);
        screenSpaceMat.SetPass(0);
        screenPositionCamera.ResetProjectionMatrix();
        screenPositionCamera.ResetWorldToCameraMatrix();
        screenSpaceMat.SetMatrix("_WorldToCameraO", screenPositionCamera.worldToCameraMatrix);
        Graphics.DrawMeshNow(m, mat);
        RenderTexture.active = null;

        //Grow positionTexture margins
        Graphics.Blit(screenSpaceTexture, growScreen, growMat);

        //Send screen space texture to drawing shader and blit into cached Render Texture
        drawMat.SetTexture("_PositionTex", growScreen);
        drawMat.SetColor("_Color", paintColor);
        drawMat.SetTexture("_PaintTex",paintTex);
        drawMat.SetFloat("_BrushTexScale", scale);
        RenderTexture t = RenderTexture.GetTemporary(screenSpaceTexture.width, screenSpaceTexture.height);
        Graphics.Blit(cache.texture, t, drawMat);
        Graphics.Blit(t, cache.texture);
        RenderTexture.ReleaseTemporary(t);

        //Clear Color
        Graphics.Blit(Texture2D.blackTexture, screenSpaceTexture);
        Graphics.Blit(Texture2D.blackTexture, growScreen);
        
    }
}
