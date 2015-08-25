using UnityEngine;
using System.Collections.Generic;

public abstract class SgtTerrain : MonoBehaviour
{
	public static List<SgtTerrain> AllTerrains = new List<SgtTerrain>();
	
	public int Resolution = 5;
	
	#pragma warning disable 414
	public int MaxSplitsInEditMode = 0;
	
	public int MaxColliderDepth = 0;
	
	public float[] SplitDistances = new float[0];
	
	[SgtRangeAttribute(0.0f, 1.0f)]
	public float SkirtThickness = 0.1f;
	
	public Material Material;
	
	public SgtCorona Corona;
	
	[SerializeField]
	private SgtPatch positiveX;
	
	[SerializeField]
	private SgtPatch positiveY;
	
	[SerializeField]
	private SgtPatch positiveZ;
	
	[SerializeField]
	private SgtPatch negativeX;
	
	[SerializeField]
	private SgtPatch negativeY;
	
	[SerializeField]
	private SgtPatch negativeZ;
	
	[System.NonSerialized]
	public Vector3[] Positions = new Vector3[0];
	
	[System.NonSerialized]
	public Vector2[] Coords1 = new Vector2[0];
	
	[System.NonSerialized]
	public Vector2[] Coords2 = new Vector2[0];
	
	[System.NonSerialized]
	public Vector3[] Normals = new Vector3[0];
	
	[System.NonSerialized]
	public Vector4[] Tangents = new Vector4[0];
	
	[System.NonSerialized]
	public Vector3[] QuadPoints = new Vector3[0];
	
	[System.NonSerialized]
	public Vector3[] QuadNormals = new Vector3[0];
	
	[System.NonSerialized]
	public Vector3[] QuadTangents = new Vector3[0];
	
	[System.NonSerialized]
	public int[] Indices = new int[0];
	
	[System.NonSerialized]
	private bool stateDirty;
	
	[System.NonSerialized]
	private bool meshDirty;
	
	[System.NonSerialized]
	private bool materialDirty;
	
	[System.NonSerialized]
	private Material expectedCoronaMaterial;
	
	[System.NonSerialized]
	private bool expectedCoronaMaterialSet;
	
	public Material CoronaMaterial
	{
		get
		{
			if (Corona != null)
			{
				return Corona.InnerMaterial;
			}
			
			return null;
		}
	}
	
	public void MarkStateAsDirty()
	{
#if UNITY_EDITOR
		SgtHelper.SetDirty(this);
#endif
		stateDirty = true;
	}
	
	public void MarkMeshAsDirty()
	{
#if UNITY_EDITOR
		SgtHelper.SetDirty(this);
#endif
		meshDirty = true;
	}
	
	public void MarkMaterialAsDirty()
	{
#if UNITY_EDITOR
		SgtHelper.SetDirty(this);
#endif
		materialDirty = true;
	}
	
	public void UpdateSplitDistances()
	{
		if (SplitDistances.Length > 0)
		{
			if (SplitDistances[0] <= 0.0f)
			{
				SplitDistances[0] = 1.0f;
			}
			
			for (var i = 1; i < SplitDistances.Length; i++)
			{
				var p = SplitDistances[i - 1];
				var c = SplitDistances[i];
				
				if (c <= 0.0f || c >= p)
				{
					SplitDistances[i] = p * 0.5f;
				}
			}
		}
	}
	
	public abstract float GetHeight(Vector3 point);
	
	protected virtual void OnEnable()
	{
		AllTerrains.Add(this);
		
		MarkMaterialAsDirty();
	}
	
	protected virtual void OnDisable()
	{
		AllTerrains.Remove(this);
	}
	
	protected virtual void Update()
	{
		UpdateSplitDistances();
		UpdateDirtyState();
		UpdateDirtyMesh();
		UpdatePatches();
	}
	
	protected virtual void LateUpdate()
	{
		UpdateDirtyMaterials();
	}
	
	protected virtual void OnDestroy()
	{
		negativeX = SgtPatch.MarkForDestruction(negativeX);
		negativeY = SgtPatch.MarkForDestruction(negativeY);
		negativeZ = SgtPatch.MarkForDestruction(negativeZ);
		positiveX = SgtPatch.MarkForDestruction(positiveX);
		positiveY = SgtPatch.MarkForDestruction(positiveY);
		positiveZ = SgtPatch.MarkForDestruction(positiveZ);
	}
	
	private void UpdatePatches()
	{
		if (negativeX == null) negativeX = CreatePatch("Negative X", Quaternion.Euler(  0.0f,  90.0f, 0.0f));
		if (negativeY == null) negativeY = CreatePatch("Negative Y", Quaternion.Euler( 90.0f,   0.0f, 0.0f));
		if (negativeZ == null) negativeZ = CreatePatch("Negative Z", Quaternion.Euler(  0.0f, 180.0f, 0.0f));
		if (positiveX == null) positiveX = CreatePatch("Positive X", Quaternion.Euler(  0.0f, 270.0f, 0.0f));
		if (positiveY == null) positiveY = CreatePatch("Positive Y", Quaternion.Euler(270.0f,   0.0f, 0.0f));
		if (positiveZ == null) positiveZ = CreatePatch("Positive Z", Quaternion.Euler(  0.0f,   0.0f, 0.0f));
	}
	
	private void UpdateDirtyState()
	{
		if (stateDirty == true)
		{
			stateDirty = false;
			
			if (positiveX != null) positiveX.UpdateStates();
			if (positiveY != null) positiveY.UpdateStates();
			if (positiveZ != null) positiveZ.UpdateStates();
			if (negativeX != null) negativeX.UpdateStates();
			if (negativeY != null) negativeY.UpdateStates();
			if (negativeZ != null) negativeZ.UpdateStates();
		}
	}
	
	private void UpdateDirtyMesh()
	{
		if (meshDirty == true)
		{
			meshDirty = false;
			
			if (positiveX != null) positiveX.RegenerateMeshes();
			if (positiveY != null) positiveY.RegenerateMeshes();
			if (positiveZ != null) positiveZ.RegenerateMeshes();
			if (negativeX != null) negativeX.RegenerateMeshes();
			if (negativeY != null) negativeY.RegenerateMeshes();
			if (negativeZ != null) negativeZ.RegenerateMeshes();
		}
	}
	
	private void UpdateDirtyMaterials()
	{
		if (expectedCoronaMaterial != CoronaMaterial || (expectedCoronaMaterialSet == true && expectedCoronaMaterial == null))
		{
			expectedCoronaMaterial    = CoronaMaterial;
			expectedCoronaMaterialSet = CoronaMaterial != null;
			materialDirty             = true;
		}
		
		if (materialDirty == true)
		{
			materialDirty = false;
			
			if (negativeX != null) negativeX.UpdateStates();
			if (negativeY != null) negativeY.UpdateStates();
			if (negativeZ != null) negativeZ.UpdateStates();
			if (positiveX != null) positiveX.UpdateStates();
			if (positiveY != null) positiveY.UpdateStates();
			if (positiveZ != null) positiveZ.UpdateStates();
		}
	}
	
	private SgtPatch CreatePatch(string name, Quaternion rotation)
	{
		var pointBL = rotation * new Vector3(-1.0f, -1.0f, 1.0f);
		var pointBR = rotation * new Vector3( 1.0f, -1.0f, 1.0f);
		var pointTL = rotation * new Vector3(-1.0f,  1.0f, 1.0f);
		var pointTR = rotation * new Vector3( 1.0f,  1.0f, 1.0f);
		var coordBL = new Vector2(1.0f, 0.0f);
		var coordBR = new Vector2(0.0f, 0.0f);
		var coordTL = new Vector2(1.0f, 1.0f);
		var coordTR = new Vector2(0.0f, 1.0f);
		
		return SgtPatchBuilder.CreatePatch(name, this, null, pointBL, pointBR, pointTL, pointTR, coordBL, coordBR, coordTL, coordTR, 0);
	}
}