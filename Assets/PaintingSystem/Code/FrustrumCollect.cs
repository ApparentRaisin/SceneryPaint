using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustrumCollect : MonoBehaviour
{
    Camera cam;
    Mesh cube;

    MeshCollider meshCollider;

    List<Vector3> verts;

    public static List<Collider> colliders = new List<Collider>();
    Rigidbody thisRigid;
    // Start is called before the first frame update
    void Start()
    {
        cam = this.GetComponentInParent<Camera>();

        //collider and rigidbody used for collision events
        meshCollider = this.GetComponent<MeshCollider>();
        thisRigid = this.GetComponent<Rigidbody>();

        //Create frustrum mesh to be used to collect colliders
        cube = new Mesh();

        Vector3[] corners = new Vector3[4];
        verts = new List<Vector3>();

        
        cam.CalculateFrustumCorners(new Rect(0.5f, 0.5f, 0.1f, 0.1f), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, corners);
        
        foreach (Vector3 v in corners)
        {
            verts.Add(Vector3.zero);
            verts.Add(v);
        }

        int[] tris = new int[36]
        {
            0,1,2,1,2,3,
            2,3,4,4,5,3,
            4,5,6,6,7,5,
            6,7,0,0,1,7,
            1,3,5,5,7,1,
            0,2,4,4,6,0
        };
        
        cube.SetVertices(verts);
        cube.SetTriangles(tris, 0);
        cube.RecalculateBounds();
        cube.RecalculateNormals();

        meshCollider.sharedMesh = cube;
        
    }

    // Update is called once per frame
    void Update()
    {
        //Recalculate frustrum mesh corners each frame to move with mouse
        Vector3[] corners = new Vector3[4];
        verts = new List<Vector3>();

        Vector3 mPos = cam.ScreenToViewportPoint(Input.mousePosition);

        float width = Paint.scale / Screen.width, height = Paint.scale / Screen.height;

        cam.CalculateFrustumCorners(new Rect(mPos.x - width, mPos.y - height, width*2, height*2), cam.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, corners);

        foreach (Vector3 v in corners)
        {
            verts.Add(Vector3.zero);
            verts.Add(v);
        }
        cube.SetVertices(verts);
        cube.RecalculateBounds();

        meshCollider.sharedMesh = cube;
        
    }

    private void FixedUpdate()
    {
        colliders.Clear();
    }


    private void OnTriggerEnter(Collider other)
    {
        //Collect all colliders in collision mesh
        colliders.Add(other);      
    }
}
