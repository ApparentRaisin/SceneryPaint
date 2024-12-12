using UnityEngine;
using UnityEngine.Rendering;

//Ouline shader making using of extra camera using replacement shader to collect screen space normals
[ExecuteAlways, ImageEffectAllowedInSceneView]
public class OutlineReplacement : MonoBehaviour
{
    Camera cam;
    [SerializeField] Camera cameraNormals;

    Material outLineMaterial;
    Shader normalsReplacement;
    RenderTexture normalsTex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = this.GetComponent<Camera>();
        normalsReplacement = Shader.Find("Unlit/NormalsReplacement");
        cam.depthTextureMode = DepthTextureMode.Depth;
        cameraNormals.SetReplacementShader(normalsReplacement, "RenderType");
        
    }
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if(outLineMaterial == null){
            outLineMaterial = new Material(Shader.Find("Hidden/OutlineReplacement"));
        }
        
        //Set prerender camera to capture screen space normals - this is done to enable editor scene view outlines
        cameraNormals.transform.position = Camera.current.transform.position;
        cameraNormals.transform.rotation = Camera.current.transform.rotation;
        cameraNormals.fieldOfView = Camera.current.fieldOfView;
        cameraNormals.farClipPlane = Camera.current.farClipPlane;
        
        RenderTexture tex = RenderTexture.GetTemporary(Camera.current.activeTexture.width,Camera.current.activeTexture.height);
        tex.format = RenderTextureFormat.ARGB64;

        cameraNormals.targetTexture = tex;
        cameraNormals.RenderWithShader(normalsReplacement, "RenderType");
        cameraNormals.targetTexture = null;
        
        //Get screen view direction
        Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(this.GetComponent<Camera>().projectionMatrix, false).inverse;
        outLineMaterial.SetMatrix("_InvProjectionMatrix", clipToView);
        outLineMaterial.SetTexture("_NormalsTex", tex);

        //Final blit for outline
        Graphics.Blit(src,dest,outLineMaterial);

        RenderTexture.ReleaseTemporary(tex);
    }

    private void OnDisable() {
        //normalsTex.Release();
    }
}
