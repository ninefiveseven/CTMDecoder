using System;
using System.Diagnostics;

namespace _3mxImport
{
	public class CTMMesh
	{
		public const int CTM_ATTR_ELEMENT_COUNT = 4;
	    public const int CTM_NORMAL_ELEMENT_COUNT = 3;
	    public const int CTM_POSITION_ELEMENT_COUNT = 3;
	    public const int CTM_UV_ELEMENT_COUNT = 2;
	    //
	    public readonly float[] vertices, normals;
	    public readonly int[] indices;
	    // Multiple sets of UV coordinate maps (optional)
	    public readonly V2Data[] texcoordinates;
	    // Multiple sets of custom vertex attribute maps (optional)
	    public readonly V2Data[] attributs;
	
	    public CTMMesh(float[] vertices, float[] normals, int[] indices, V2Data[] texcoordinates, V2Data[] attributs) {
	        Debug.Assert(vertices != null);
			Debug.Assert(indices != null);
			Debug.Assert(texcoordinates != null);
			Debug.Assert(attributs != null);
			
			this.vertices = vertices;
	        this.normals = normals;
	        this.indices = indices;
	        this.texcoordinates = texcoordinates;
	        this.attributs = attributs;
	    }
	
	    public int getVertexLength() {
	        return vertices.Length / CTM_POSITION_ELEMENT_COUNT;
	    }
	
	    public int getUVLength() {
	        return texcoordinates.Length;
	    }
	
	    public int getAttrLength() {
	        return attributs.Length;
	    }
	
	    public int getTriangleLength() {
	        return indices.Length / 3;
	    }
	
	    public bool hasNormals() {
	        return normals != null;
	    }
	
	 
	}
}

