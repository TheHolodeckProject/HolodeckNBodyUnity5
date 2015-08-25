//using UnityEngine;
//using System.Collections;
//
//public class GenerateTube : MonoBehaviour
//{
//    //Defines how many times the tube will curve
//    public int walks = 1;
//    public int walklength = 1;
//    //Defines the radius of the circle
//    public float radius = 1f;
//    // Defines how many squares go around the middle of the sphere. Basically, the resolution of the sphere.
//    public int StimRes = 5;
//
//    //Defines the texture to be used for the sphere
//    //Texture stimTexture;
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
//        renderer.material.mainTexture = stimTexture;
//        renderer.material.SetColor("_SpecColor", Color.white);
//        //Initializes a vector3 array that will hold the vectors of the tube mesh
//        Vector3[] vertices = new Vector3[((StimRes + 1) * (walks + 1))];
//        //Debug.Log("Length of vertices is " + vertices.Length);
//        //Defines pi
//        float pi = Mathf.PI;
//        //For every walk
//        for (int walk = 0; walk <= walks; walk++)
//        {
//            //For every side of the tube
//            for (int face = 0; face <= StimRes; face++)
//            {
//                //Chops up a circle around the top of the circle into slices,then finds the Sin and Cos of each angle
//                float a1 = (2 * pi) * (float)(face == StimRes ? 0 : face) / StimRes;
//                float sin1 = Mathf.Sin(a1);
//                float cos1 = Mathf.Cos(a1);
//
//                //Defines which vertex we're defining
//                int vertexindex = face + walk * (StimRes + 1);
//                //Defines the base, unrotated Vector3
//                Vector3 newVec = new Vector3(cos1 * radius, walklength / (walk + 1), sin1 * radius);
//                //Adds the vertices of that triangle to the vertices array
//                vertices[vertexindex] = newVec;
//            }
//        }
//
//        //Creates the normals of the mesh. Normals are like poles sticking out of every vertex
//        //Initializes a new vector3 array to hold the normals
//        Vector3[] normales = new Vector3[vertices.Length];
//        //For every vertex
//        for (int n = 0; n < vertices.Length; n++)
//            //??? Not sure whty this is normalized
//            normales[n] = vertices[n].normalized;
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
//        //Initializes a counter
//        int i = 0;
//        //For every walk
//        for (int walk = 0; walk < walks; walk++)
//        {
//            //For every face of the tube
//            for (int face = 0; face < StimRes; face++)
//            {
//                int current = face + walk * (StimRes + 1);
//                int next = current + StimRes + 1;
//                //Defines the triangles
//                triangles[i++] = current;
//                triangles[i++] = current + 1;
//                triangles[i++] = next + 1;
//
//                triangles[i++] = current;
//                triangles[i++] = next + 1;
//                triangles[i++] = next;
//            }
//        }
//
//
//        //ADDED - Bends the tube
//        //Defines a Vector3 list containing the angles at which each ring of the tube will be displaced in every direction
//        //Vector3[] rotations;
//        //rotations = new Vector3[vertices.Length / (StimRes + 1)];
//        //Debug.Log("Vertices is " + vertices.Length + " long");
//        //Gives it a random rotation for Y
//        //rotations[1].x = 0.5f;
//        //rotations[1].y = 0.5f;
//        //rotations[1].z = 0.5f;
//        //For every walk
//        //for (int walk = 0; walk <= walks; walk++)
//        //{
//        //    //Checks if it's going to bend or not
//        //    if (rotations[walk].x != 0 | rotations[walk].y != 0 | rotations[walk].x != 0)
//        //        Debug.Log("Bending walk " + walk);
//        //    else
//        //        Debug.Log("Not Bending walk " + walk);
//        //    //For every face of the tube
//        //    for (int face = 0; face <= StimRes; face++)
//        //    {
//        //        //Defines which vertex we're changing
//        //        int vertexindex = face + walk * (StimRes+1) ;
//        //        //Checks if a vertex is actually being modified
//        //        if (vertices[vertexindex] != DoTwist(vertices[vertexindex], rotations[walk]))
//        //            Debug.Log("Vertex " + vertexindex + "Distorting from " + vertices[vertexindex] + " with " + DoTwist(vertices[vertexindex], rotations[walk]));
//        //        ////Calculates the transformed vector and normale
//        //        //vertices[vertexindex] = DoTwist(vertices[vertexindex], vertices[vertexindex].z * bend[walk]);
//        //        //vertices[vertexindex] = DoTwist(normales[vertexindex], vertices[vertexindex].z * bend[walk]);
//
//        //        vertices[vertexindex] = DoTwist(vertices[vertexindex], rotations[walk]);
//        //        vertices[vertexindex] = DoTwist(normales[vertexindex], rotations[walk]);
//
//        //    }
//        //}
//        //for (int vertex = 0; vertex < vertices.Length; vertex++)
//        //{
//        //    Debug.Log("Vertex " + vertex + " = " + vertices[vertex]);
//        //}
//
//        //ADDED
//        //Creates a list of Vector3 distortions
//        Vector3[] distortions;
//        distortions = new Vector3[vertices.Length];
//        //For every walk
//        for (int walk = 0; walk <= walks; walk++)
//        {
//                //For every face of the tube
//
//               for (int face = 0; face <= StimRes; face++)
//               {
//                   int vertexindex = face + walk * (StimRes + 1);
//                   //distortions[vertexindex].x;
//
//               }
//            }
//        //Assigns all the things we created to the mesh we created earlier
//        mesh.vertices = vertices;
//        mesh.normals = normales;
//        mesh.triangles = triangles;
//        mesh.RecalculateBounds();
//        mesh.Optimize();
//    }
//
//    //Vector3 DoTwist(Vector3 vec, Vector3 rotations)
//    //{
//        //float xTan = Mathf.Tan(rotations.x);
//        //float yTan = Mathf.Tan(rotations.y);
//        //float zTan = Mathf.Tan(rotations.z);
//        //Vector3 new_pos = Vector3.zero;
//       //new_pos.x = vec.x;
//        //new_pos.x = vec.x + vec.x * xTan * vec.x * xTan;
//        //new_pos.y = vec.y + vec.y * yTan * vec.y * yTan;
//        //new_pos.z = vec.z + vec.z * zTan * vec.z * zTan;
//    //    return new_pos;
//    //}
//
//}
