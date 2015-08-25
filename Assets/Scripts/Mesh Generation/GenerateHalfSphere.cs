//using UnityEngine;
//using System.Collections;
//
//public class GenerateHalfSphere : MonoBehaviour
//{
//    //Defines the radius of the circle
//    public float radius = 1f;
//    // Defines how many squares go around the middle of the sphere. Basically, the resolution of the sphere.
//    public int StimRes = 30;
//
//	//Defines the texture to be used for the sphere
//	//Texture stimTexture;
//
//    void Start()
//    {
//        //Adds a mesh filter and renderer to the empty gameobject
//        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
//        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
//        //??? Creates a mesh from the meshfilter?
//        Mesh mesh = filter.mesh;
//        //??? Clears the mesh?
//        mesh.Clear();
//        //Assigns the texture to the mesh
//        //renderer.material.mainTexture = stimTexture;
//		renderer.material.SetColor("_SpecColor", Color.white);
//        //Initializes a vector3 array that will hold the vectors of this sphere
//        //Divides by 2 because we're only making a half-sphere
//        Vector3[] vertices = new Vector3[((StimRes + 1) * (StimRes/2 + 1) + 1)];
//        //Debug.Log("Length of vertices is " + vertices.Length);
//        //Defines pi
//        float pi = Mathf.PI; 
//        //Defines the first vertex as the top of the sphere
//        vertices[0] = Vector3.up * radius;
//        //For every horizontal slice of the half sphere
//        for (int lat = 0; lat <= StimRes/2; lat++)
//        {
//            //Chops up a circle around the top of the circle into slices,then finds the Sin and Cos of each angle
//            float a1 = pi * (float)(lat + 1) / (StimRes + 1);
//            float sin1 = Mathf.Sin(a1);
//            float cos1 = Mathf.Cos(a1);
//            //For every vertical slice
//            for (int lon = 0; lon <= StimRes; lon++)
//            {
//				//??? Figures out the other angle of that triangle
//                float a2 = (2 * pi) * (float)(lon == StimRes ? 0 : lon) / StimRes;
//                float sin2 = Mathf.Sin(a2);
//                float cos2 = Mathf.Cos(a2);
//				//Defines which vertex we're defining
//				int vertexindex = lon + lat * (StimRes+1) + 1;
//				//Debug.Log ("Trying to define vertex # " + vertexindex);
//				//Adds the vertices of that triangle to the vertices array
//                vertices[vertexindex] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;
//            }
//        }
//        //Creates the normals of the mesh. Normals are like poles sticking out of every vertex
//        //Initializes a new vector3 array to hold the normals
//        Vector3[] normales = new Vector3[vertices.Length];
//        //For every vertex
//        for (int n = 0; n < vertices.Length; n++)
//            //??? Not sure whty this is normalized
//            normales[n] = vertices[n].normalized;
//
//        //Creates the triangles
//        //Defines the number of faces
//        int nbFaces = vertices.Length;
//        //Defines the number of triangles there'll be
//        int nbTriangles = nbFaces * 2;
//        //Defines the number of indexes there will be
//        int nbIndexes = nbTriangles * 3;
//        //??? Not sure what this is doing
//        int[] triangles = new int[nbIndexes];
//  
//		//Top Cap
//        int i = 0;
//        //??? Not sure what this is doing
//        for (int lon = 0; lon < StimRes; lon++)
//          //for (int lon = 0; lon < 1; lon++)
//        {
//            triangles[i++] = lon + 2;
//            triangles[i++] = lon + 1;
//            triangles[i++] = 0;
//        }
//          //Middle
//          //For every horizontal slice of the circle
//          for (int lat = 0; lat <= StimRes/2 - 1; lat++)
//          //for (int lat = 0; lat < 1; lat++)
//          {
//              //For every vrtical slice of the circle
//              for (int lon = 0; lon < StimRes; lon++)
//              //for (int lon = 0; lon < 1; lon++)
//              {
//				int current = lon + lat * (StimRes + 1) + 1;
//				int next = current + StimRes + 1;
//                  //??? Not sure what this is doing
//                  triangles[i++] = current;
//                  triangles[i++] = current + 1;
//                  triangles[i++] = next + 1;
//
//                  triangles[i++] = current;
//                  triangles[i++] = next + 1;
//                  triangles[i++] = next;
//              }
//          }
//		//Assigns all the things we created to the mesh we created earlier
//        mesh.vertices = vertices;
//        mesh.normals = normales;
//        mesh.triangles = triangles;
//        mesh.RecalculateBounds();
//        mesh.Optimize();
//		//Applies a mesh collider
//		//gameObject.AddComponent (MeshCollider);
//    }
//}