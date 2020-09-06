using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

  [SerializeField]
  private int xSize;

  [SerializeField]
  private int ySize;

  public Material mat;
  
  private Mesh mesh;

  private Vector3[] verts;
  private int[] tris;
  //vertex count = xsize + 1 * zsize + 1

  // Start is called before the first frame update
  void Start()
  {
    mesh = new Mesh();
    GetComponent<MeshFilter>().mesh = mesh;

    StartCoroutine(CreateShape());
  }

  private void Update()
  {
    UpdateMesh();
  }

  public IEnumerator CreateShape()
  {
    verts = new Vector3[(xSize + 1) * (ySize + 1)];

    for (int i = 0, y = 0; y <= ySize; y++)
    {
      for (int x = 0; x <= xSize; x++)
      {
        float newy = Mathf.PerlinNoise(x * .3f, y * .3f) * 2f;
        verts[i] = new Vector3(newy,y,0);
        i++;
      }
    }

    tris = new int[xSize * ySize * 6];

    int vert = 0;
    int tri = 0;

    for (int y = 0; y < ySize; y++)
    {
      for (int x = 0; x < xSize; x++)
      {

        tris[tri + 0] = vert + 0;
        tris[tri + 1] = vert + xSize + 1;
        tris[tri + 2] = vert + 1;
        tris[tri + 3] = vert + 1;
        tris[tri + 4] = vert + xSize + 1;
        tris[tri + 5] = vert + xSize + 2;

        vert++;
        tri += 6;

        yield return new WaitForSeconds(.01f);
      }

      vert++;
    }

  }

  public void UpdateMesh()
  {
    mesh.Clear();
    
    mesh.vertices = verts;
    mesh.triangles = tris;
    
    mesh.RecalculateNormals();
  }

  public void OnDrawGizmos()
  {
    if (verts == null)
      return;

    for (int i = 0; i < verts.Length; i++)
    {
      Gizmos.DrawSphere(verts[i], .1f);
    }
  }

}
