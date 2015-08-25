using UnityEngine;

// This component allows you to control a Camera component's depthTextureMode setting.
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Depth Texture Mode")]
public class SgtDepthTextureMode : MonoBehaviour
{
	public DepthTextureMode DepthMode = DepthTextureMode.None;
	
	private new Camera camera;
	
	protected virtual void Update()
	{
		if (camera == null) camera = GetComponent<Camera>();
		
		camera.depthTextureMode = DepthMode;
	}
}