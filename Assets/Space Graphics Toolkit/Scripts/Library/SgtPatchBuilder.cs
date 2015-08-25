using UnityEngine;

public static class SgtPatchBuilder
{
	public static SgtPatch CreatePatch(string name, SgtTerrain terrain, SgtPatch parent, Vector3 pointBL, Vector3 pointBR, Vector3 pointTL, Vector3 pointTR, Vector3 coordBL, Vector3 coordBR, Vector3 coordTL, Vector3 coordTR, int depth)
	{
		if (terrain != null)
		{
			var parentTransform = parent != null ? parent.transform : terrain.transform;
			var patch           = SgtPatch.Create(name, terrain, parentTransform);
			
			patch.Parent  = parent;
			patch.Depth   = depth;
			patch.PointBL = pointBL;
			patch.PointBR = pointBR;
			patch.PointTL = pointTL;
			patch.PointTR = pointTR;
			patch.CoordBL = coordBL;
			patch.CoordBR = coordBR;
			patch.CoordTL = coordTL;
			patch.CoordTR = coordTR;
			
			patch.UpdateState();
			
#if UNITY_EDITOR
			if (Application.isPlaying == false)
			{
				patch.UpdateSplitMerge();
			}
#endif
			
			return patch;
		}
		
		return null;
	}
	
	public static void GenerateMesh(SgtPatch patch)
	{
		var terrain = patch.Terrain;
		
		if (terrain.Resolution > 0)
		{
			var positions    = terrain.Positions;
			var coords1      = terrain.Coords1;
			var coords2      = terrain.Coords2;
			var normals      = terrain.Normals;
			var tangents     = terrain.Tangents;
			var quadPoints   = terrain.QuadPoints;
			var quadNormals  = terrain.QuadNormals;
			var quadTangents = terrain.QuadTangents;
			var indices      = terrain.Indices;
			var res          = terrain.Resolution;
			var resAdd1      = res + 1;
			var resAdd2      = res + 2;
			var resAdd3      = res + 3;
			var resRecip     = SgtHelper.Reciprocal(res);
			var mainVerts    = resAdd1 * resAdd1;
			var skirtVerts   = resAdd1 * 4;
			var mainIndices  = res * res * 6;
			var skirtIndices = res * 24;
			var vertex       = default(int);
			var vertex2      = default(int);
			var index        = default(int);
			
			if (    positions.Length != mainVerts + skirtVerts ) terrain.Positions    = positions    = new Vector3[mainVerts + skirtVerts];
			if (      coords1.Length != mainVerts + skirtVerts ) terrain.Coords1      = coords1      = new Vector2[mainVerts + skirtVerts];
			if (      coords2.Length != mainVerts + skirtVerts ) terrain.Coords2      = coords2      = new Vector2[mainVerts + skirtVerts];
			if (      normals.Length != mainVerts + skirtVerts ) terrain.Normals      = normals      = new Vector3[mainVerts + skirtVerts];
			if (     tangents.Length != mainVerts + skirtVerts ) terrain.Tangents     = tangents     = new Vector4[mainVerts + skirtVerts];
			if (   quadPoints.Length != resAdd3 * resAdd3) terrain.QuadPoints   = quadPoints   = new Vector3[resAdd3 * resAdd3];
			if (  quadNormals.Length != resAdd2 * resAdd2              ) terrain.QuadNormals  = quadNormals  = new Vector3[resAdd2 * resAdd2];
			if ( quadTangents.Length != resAdd2 * resAdd2              ) terrain.QuadTangents = quadTangents = new Vector3[resAdd2 * resAdd2];
			
			// Go through all vertices, but extend the borders by one
			for (var y = -1; y < resAdd2; y++)
			{
				for (var x = -1; x < resAdd2; x++)
				{
					var u      = x * resRecip;
					var v      = y * resRecip;
					var pointB = Lerp3(patch.PointBL, patch.PointBR, u);
					var pointT = Lerp3(patch.PointTL, patch.PointTR, u);
					var point  = Lerp3(pointB, pointT, v).normalized; point *= terrain.GetHeight(point);
					
					index = x + 1 + (y + 1) * resAdd3;
					
					quadPoints[index] = point;
					
					// Is this a main vertex?
					if (x >= 0 && x < resAdd1 && y >= 0 && y < resAdd1)
					{
						var coordB = Lerp2(patch.CoordBL, patch.CoordBR, u);
						var coordT = Lerp2(patch.CoordTL, patch.CoordTR, u);
						var coord1 = Lerp2(coordB, coordT, v);
						
						vertex = x + y * resAdd1;
						
						positions[vertex] = point;
						
						coords1[vertex] = coord1;
						
						coords2[vertex] = new Vector2(u, v);
					}
				}
			}
			
			// Quad normals & tangents
			for (var y = 0; y < resAdd2; y++)
			{
				for (var x = 0; x < resAdd2; x++)
				{
					var bl = x + y * resAdd3;
					var br = x + 1 + y * resAdd3;
					var tl = x + (y + 1) * resAdd3;
					var tr = x + 1 + (y + 1) * resAdd3;
					
					var b = quadPoints[bl] - quadPoints[br];
					var t = quadPoints[tl] - quadPoints[tr];
					var l = quadPoints[bl] - quadPoints[tl];
					var r = quadPoints[br] - quadPoints[tr];
					
					var h = (b + t).normalized;
					var v = (l + r).normalized;
					var i = x + y * resAdd2;
					
					quadNormals[i] = Vector3.Cross(h, v);
					
					quadTangents[i] = v;
				}
			}
			
			// Normals & Tangents
			for (var y = 0; y < resAdd1; y++)
			{
				for (var x = 0; x < resAdd1; x++)
				{
					var bl = x + y * resAdd2;
					var br = x + 1 + y * resAdd2;
					var tl = x + (y + 1) * resAdd2;
					var tr = x + 1 + (y + 1) * resAdd2;
					
					var n = quadNormals[bl] + quadNormals[br] + quadNormals[tl] + quadNormals[tr];
					var t = quadTangents[bl] + quadTangents[br] + quadTangents[tl] + quadTangents[tr];
					var i = x + y * resAdd1;
					
					normals[i] = n.normalized;
					//normals[i] = positions[i].normalized;
					
					tangents[i] = SgtHelper.NewVector4(t.normalized, 1.0f);
				}
			}
			
			// Skirt vertices
			var scale = 1.0f - SgtHelper.Divide(terrain.SkirtThickness * Mathf.Pow(0.5f, patch.Depth), 1.0f);
			
			for (var i = 0; i < resAdd1; i++)
			{
				// Bottom
				vertex  = mainVerts + i;
				vertex2 = i;
				
				positions[vertex] = positions[vertex2] * scale; coords1[vertex] = coords1[vertex2]; coords2[vertex] = coords2[vertex2]; normals[vertex] = normals[vertex2]; tangents[vertex] = tangents[vertex2];
				
				// Top
				vertex  = mainVerts + i + resAdd1;
				vertex2 = resAdd1 * res + i;
				
				positions[vertex] = positions[vertex2] * scale; coords1[vertex] = coords1[vertex2]; coords2[vertex] = coords2[vertex2]; normals[vertex] = normals[vertex2]; tangents[vertex] = tangents[vertex2];
				
				// Left
				vertex  = mainVerts + i + resAdd1 + resAdd1;
				vertex2 = resAdd1 * i;
				
				positions[vertex] = positions[vertex2] * scale; coords1[vertex] = coords1[vertex2]; coords2[vertex] = coords2[vertex2]; normals[vertex] = normals[vertex2]; tangents[vertex] = tangents[vertex2];
				
				// Right
				vertex  = mainVerts + i + resAdd1 + resAdd1 + resAdd1;
				vertex2 = resAdd1 * i + res;
				
				positions[vertex] = positions[vertex2] * scale; coords1[vertex] = coords1[vertex2]; coords2[vertex] = coords2[vertex2]; normals[vertex] = normals[vertex2]; tangents[vertex] = tangents[vertex2];
			}
			
			// Indices
			if (indices.Length != mainIndices + skirtIndices)
			{
				terrain.Indices = indices = new int[mainIndices + skirtIndices];
				
				// Main
				for (var y = 0; y < res; y++)
				{
					for (var x = 0; x < res; x++)
					{
						index  = (x + y * res) * 6;
						vertex = x + y * resAdd1;
						
						indices[index + 0] = vertex;
						indices[index + 1] = vertex + 1;
						indices[index + 2] = vertex + resAdd1;
						indices[index + 3] = vertex + resAdd1 + 1;
						indices[index + 4] = vertex + resAdd1;
						indices[index + 5] = vertex + 1;
					}
				}
				
				// Skirt
				for (var i = 0; i < res; i++)
				{
					// Bottom
					index   = mainIndices + (res * 0 + i) * 6;
					vertex  = mainVerts + i;
					vertex2 = i;
					
					indices[index + 0] = vertex;
					indices[index + 1] = vertex + 1;
					indices[index + 2] = vertex2;
					indices[index + 3] = vertex2 + 1;
					indices[index + 4] = vertex2;
					indices[index + 5] = vertex + 1;
					
					// Top
					index   = mainIndices + (res * 1 + i) * 6;
					vertex  = mainVerts + i + resAdd1;
					vertex2 = res * resAdd1 + i;
					
					indices[index + 0] = vertex2;
					indices[index + 1] = vertex2 + 1;
					indices[index + 2] = vertex;
					indices[index + 3] = vertex + 1;
					indices[index + 4] = vertex;
					indices[index + 5] = vertex2 + 1;
					
					// Left
					index   = mainIndices + (res * 2 + i) * 6;
					vertex  = mainVerts + i + resAdd1 + resAdd1;
					vertex2 = i * resAdd1;
					
					indices[index + 0] = vertex;
					indices[index + 1] = vertex2;
					indices[index + 2] = vertex + 1;
					indices[index + 3] = vertex2 + resAdd1;
					indices[index + 4] = vertex + 1;
					indices[index + 5] = vertex2;
					
					// Right
					index   = mainIndices + (res * 3 + i) * 6;
					vertex  = mainVerts + i + resAdd1 + resAdd1 + resAdd1;
					vertex2 = i * resAdd1 + res;
					
					indices[index + 0] = vertex2;
					indices[index + 1] = vertex;
					indices[index + 2] = vertex2 + resAdd1;
					indices[index + 3] = vertex + 1;
					indices[index + 4] = vertex2 + resAdd1;
					indices[index + 5] = vertex;
				}
			}
			
			if (patch.Mesh != null)
			{
				patch.Mesh.Clear();
			}
			else
			{
				patch.Mesh = SgtObjectPool<Mesh>.Pop() ?? new Mesh();
				patch.Mesh.hideFlags = HideFlags.DontSave;
			}
			
			patch.Mesh.vertices  = positions;
			patch.Mesh.uv        = coords1;
			patch.Mesh.uv2       = coords2;
			patch.Mesh.normals   = normals;
			patch.Mesh.tangents  = tangents;
			patch.Mesh.triangles = indices;
			patch.Mesh.RecalculateBounds();
			
			patch.MeshCenter = patch.Mesh.bounds.center;
		}
	}
	
	// Excludes clamp
	private static Vector2 Lerp2(Vector2 a, Vector2 b, float t)
	{
		a.x += (b.x - a.x) * t;
		a.y += (b.y - a.y) * t;
		
		return a;
	}
	
	private static Vector3 Lerp3(Vector3 a, Vector3 b, float t)
	{
		a.x += (b.x - a.x) * t;
		a.y += (b.y - a.y) * t;
		a.z += (b.z - a.z) * t;
		
		return a;
	}
}