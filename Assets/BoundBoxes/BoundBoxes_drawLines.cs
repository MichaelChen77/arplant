using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BoundBoxes_drawLines : MonoBehaviour {
	public Material lineMaterial;
	Dictionary<int, Vector3[,]> outlines = new Dictionary<int, Vector3[,]> ();
	Dictionary<int, Color> colors = new Dictionary<int, Color>();

	int linesID = 0;

	void CreateLineMaterial()
	{
	    if( !lineMaterial ) {
	        lineMaterial = new Material(Shader.Find("Custom/GizmoShader"));
	    }
	}

	void OnPostRender() {
		if(outlines==null) return;
	    CreateLineMaterial();
	    lineMaterial.SetPass(0);
		GL.Begin( GL.LINE_STRIP );
		foreach(KeyValuePair<int, Color> c in colors)
		{
			GL.Color (c.Value);
			for (int i=0; i<outlines[c.Key].GetLength(0); i++) {
				GL.Vertex(outlines[c.Key][i,0]);
				GL.Vertex(outlines[c.Key][i,1]);
			}
		}
		GL.End();
	}

	public int AddOutlines(Vector3[,] newOutlines, Color newcolor)
	{
		if (newOutlines.GetLength (0) > 0) {
			linesID++;
			outlines[linesID] = newOutlines;
			colors[linesID] = newcolor;
		}
		return linesID;
	}
		
	public void SetOutlines(int index,Vector3[,] newOutlines, Color newcolor) {
		if(newOutlines.GetLength(0)>0)	{
			outlines [index] = newOutlines;
			colors [index] = newcolor;
		}
	}

	public void DeleteOutlines(int index)
	{
		if(outlines.ContainsKey(index))
		{
			linesID = 0;
			outlines.Remove (index);
			colors.Remove(index);
		}
	}

	public void Reset()
	{
		outlines.Clear ();
		colors.Clear ();
	}
}
