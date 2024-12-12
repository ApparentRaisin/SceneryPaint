using UnityEngine;

public class PaintCache : MonoBehaviour
{
    //Cached Data
    Material[] thisMat;
    public RenderTexture texture;
    void OnEnable()
    {
        //Cache all important materials and texture per-object to drecreate the number of .GetTexture calls
        thisMat = GetComponent<MeshRenderer>().materials;
        Texture2D cTex = thisMat[0].GetTexture("_MainTex") as Texture2D;
        
        texture = new RenderTexture(2048,2048,0);
        Graphics.Blit(cTex, texture);
        
        for(int i = 0; i < thisMat.Length; i++){
            thisMat[i].SetTexture("_MainTex", texture);
        }

        //Destroy(cTex);
    }

    private void OnDisable()
    {
        texture.Release();
    }

    //Save the texture to a file
    //TODO: use this to make save file for scene
    public static Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        return tex;
    }
}
