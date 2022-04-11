using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
 
public class Meshes2Terrain : EditorWindow {
 
	[MenuItem("Terrain/Meshes To Terrain", false, 2000)] static void OpenWindow () {
		EditorWindow.GetWindow<Meshes2Terrain>(true);
	}
 
	private int resolution = 512;
 
	void OnGUI () {
 
		resolution = EditorGUILayout.IntField("Resolution", resolution);
		if(GUILayout.Button("Create Terrain")){
			if(Selection.activeGameObject == null){
				EditorUtility.DisplayDialog("No object selected", "Please select an object.", "Ok");
				return;
			}
			CreateTerrainByMesh();
		}
	}

	void CreateTerrainByMesh()
	{
		ShowProgressBar(0,100);
		TerrainData terrain = new TerrainData();
		terrain.heightmapResolution = resolution;
		GameObject terrainObject = Terrain.CreateTerrainGameObject(terrain);
		
		Undo.RegisterCreatedObjectUndo(terrainObject, "Object to Terrain");
		List<MeshCollider> cleans = new List<MeshCollider>();
		GameObject[] objects = Selection.gameObjects;
		Vector3 maxp = new Vector3(float.MinValue,float.MinValue,float.MinValue);
		Vector3 minp = new Vector3(float.MaxValue,float.MaxValue,float.MaxValue);
		foreach (var obj in objects)
		{
			MeshCollider col = obj.GetComponent<MeshCollider>();
			if(!col){
				col = obj.AddComponent<MeshCollider>();
				cleans.Add(col);
			}

			Bounds b = col.bounds;
			Vector3 pos = col.transform.position;
			Vector3 tmp = b.max;
			maxp = new Vector3(Mathf.Max(tmp.x, maxp.x), Mathf.Max(tmp.y, maxp.y), Mathf.Max(tmp.z, maxp.z));
			tmp = b.min;
			minp = new Vector3(Mathf.Min(tmp.x, minp.x), Mathf.Min(tmp.y, minp.y, 0), Mathf.Min(tmp.z, minp.z));
		}

		Vector3 objsSize = maxp - minp;
		float tExt = Mathf.Max(objsSize.x, objsSize.z);
		terrain.size = new Vector3(tExt, objsSize.y, tExt);

		float[,] heights = new float[terrain.heightmapResolution, terrain.heightmapResolution];	
		Ray ray = new Ray(new Vector3(minp.x, maxp.y * 2, minp.z), -Vector3.up);
		RaycastHit hit = new RaycastHit();
		Vector3 rayOrigin = ray.origin;
		int maxHeight = heights.GetLength(0);
		int maxLength = heights.GetLength(1);
		Vector2 stepXZ = new Vector2(terrain.size.x / maxLength, terrain.size.z / maxHeight);
		for(int zCount = 0; zCount < maxHeight; zCount++){
			for(int xCount = 0; xCount < maxLength; xCount++){
				float height = 0.0f;
				if (Physics.Raycast(ray, out hit, maxp.y * 3))
				{
					Collider col = hit.collider;
					Bounds b = col.bounds;
					float meshHeightInverse = 1 / terrain.size.y;
					height = Mathf.Max((hit.point.y - minp.y) * meshHeightInverse);
				}
				heights[zCount, xCount] = height;
				rayOrigin.x += stepXZ[0];
				ray.origin = rayOrigin;
			}
			rayOrigin.z += stepXZ[1];
			rayOrigin.x = minp.x;
			ray.origin = rayOrigin;
			ShowProgressBar(zCount,maxHeight);
		}
		terrain.SetHeights(0, 0, heights);
		foreach (var col in cleans)
		{
			DestroyImmediate(col);
		}
		EditorUtility.ClearProgressBar();
	}
	
	void ShowProgressBar(float progress, float maxProgress){
		float p = progress / maxProgress;
		EditorUtility.DisplayProgressBar("Creating Terrain...", Mathf.RoundToInt(p * 100f)+ " %", p);
	}
}