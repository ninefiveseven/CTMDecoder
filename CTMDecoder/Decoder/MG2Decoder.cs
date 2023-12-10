using System;

namespace _3mxImport
{
	public class MG2Decoder : MG1Decoder
	{
		public static readonly int MG2_Tag = CtmReader.getTagInt("MG2\0");
	    public static readonly int MG2_HEADER_TAG = CtmReader.getTagInt("MG2H");
	    public static readonly int GIDX = CtmReader.getTagInt("GIDX");
	
	    public override bool isSupported(int tag, int version) {
	        return tag == MG2_Tag && version == RawDecoder.FORMAT_VERSION;
	    }
	
	    public override CTMMesh meshDecode(MeshInfo minfo, CtmStream input){
	        int vc = minfo.getVertexCount();
	
	        checkTag(input.GetInt(), MG2_HEADER_TAG);
	        float vertexPrecision = input.GetFloat();
	        float normalPrecision = input.GetFloat();
	
	        CTMGrid grid = CTMGrid.fromStream(input);
	        if(!grid.check()) {
	            throw new Exception("The vertex size grid is corrupt!");
	        }
	
	        float[] vertices = readVertices(input, grid, vc, vertexPrecision);
	
	        int[] indices = readIndices(input, minfo.getTriangleCount(), vc);
	
	        float[] normals = null;
	        if (minfo.hasNormals()) {
	            normals = readNormals(input, vertices, indices, normalPrecision, vc);
	        }
	
	        V2Data[] uvData = new V2Data[minfo.getUvMapCount()];
	        for (int i = 0; i < uvData.Length; i++) {
	            uvData[i] = readUvData(input, vc);
	        }
	
	        V2Data[] attributs = new V2Data[minfo.getAttrCount()];
	        for (int i = 0; i < attributs.Length; i++) {
	            attributs[i] = readAttribute(input, vc);
	        }
	
	        return new CTMMesh(vertices, normals, indices, uvData, attributs);
	    }
	
	    private float[] readVertices(CtmStream input, CTMGrid grid, int vcount, float precision){
	        checkTag(input.GetInt(), VERT);
	        int[] intVertices = input.GetPackedInts(vcount, CTMMesh.CTM_POSITION_ELEMENT_COUNT, false);
	
	        checkTag(input.GetInt(), GIDX);
	        int[] gridIndices = input.GetPackedInts(vcount, 1, false);
	        for (int i = 1; i < vcount; i++) {
	            gridIndices[i] += gridIndices[i - 1];
	        }
	
	        return Algorithm.revertVertices(intVertices, gridIndices, grid, precision);
	    }
	
	    private int[] readIndices(CtmStream input, int triCount, int vcount){
	        checkTag(input.GetInt(), INDX);
	        int[] indices = input.GetPackedInts(triCount, 3, false);
	        restoreIndices(triCount, indices);
	        foreach(int i in indices) {
	            if (i > vcount) {
	                throw new Exception("One element of the indice array "
	                                               + "points to a none existing vertex(id: " + i + ")");
	            }
	        }
	        return indices;
	    }
	
	    private float[] readNormals(CtmStream input, float[] vertices, int[] indices,
	                                float normalPrecision, int vcount){
	        checkTag(input.GetInt(), NORM);
	        int[] intNormals = input.GetPackedInts(vcount, CTMMesh.CTM_NORMAL_ELEMENT_COUNT, false);
	        return restoreNormals(intNormals, vertices, indices, normalPrecision);
	    }
	
	    private V2Data readUvData(CtmStream input, int vcount){
	        checkTag(input.GetInt(), TEXC);
	        String name = input.GetString();
	        String material = input.GetString();
	        float precision = input.GetFloat();
	        if (precision <= 0f) {
	            throw new Exception("A uv precision value <= 0.0 was read");
	        }
	
	        int[] intCoords = input.GetPackedInts(vcount, CTMMesh.CTM_UV_ELEMENT_COUNT, true);
	        float[] data = restoreUVCoords(precision, intCoords);
	
	        return new V2Data(data);
	    }
	
	    private V2Data readAttribute(CtmStream input, int vc){
	        checkTag(input.GetInt(), ATTR);
	
	        String name = input.GetString();
	        float precision = input.GetFloat();
	        if (precision <= 0f) {
	            throw new Exception("An attribute precision value <= 0.0 was read");
	        }
	
	        int[] intData = input.GetPackedInts(vc, CTMMesh.CTM_ATTR_ELEMENT_COUNT, true);
	        float[] data = restoreAttribs(precision, intData);
	
	        return new V2Data( data);
	    }
	    private float[] restoreAttribs(float precision, int[] intAttribs) {
	        int ae = CTMMesh.CTM_ATTR_ELEMENT_COUNT;
	        int vc = intAttribs.Length / ae;
	        float[] values = new float[intAttribs.Length];
	        int[] prev = new int[ae];
	        for (int i = 0; i < vc; ++i) {
	            for (int j = 0; j < ae; ++j) {
	                int value = intAttribs[i * ae + j] + prev[j];
	                values[i * ae + j] = value * precision;
	                prev[j] = value;
	            }
	        }
	        return values;
	    }
	    private float[] restoreUVCoords(float precision, int[] intUVCoords) {
	        int vc = intUVCoords.Length / CTMMesh.CTM_UV_ELEMENT_COUNT;
	        float[] values = new float[intUVCoords.Length];
	        int prevU = 0, prevV = 0;
	        for (int i = 0; i < vc; ++i) {
	            int u = intUVCoords[i * CTMMesh.CTM_UV_ELEMENT_COUNT] + prevU;
	            int v = intUVCoords[i * CTMMesh.CTM_UV_ELEMENT_COUNT + 1] + prevV;
	            values[i * CTMMesh.CTM_UV_ELEMENT_COUNT] = u * precision;
	            values[i * CTMMesh.CTM_UV_ELEMENT_COUNT + 1] = v * precision;
	
	            prevU = u;
	            prevV = v;
	        }
	        return values;
	    }
	    private float[] restoreNormals(int[] intNormals, float[] vertices, int[] indices, float normalPrecision) {
	        float[] smoothNormals = Algorithm.GetSmoothNormals(vertices, indices);
	        float[] normals = new float[vertices.Length];
	
	        int vc = vertices.Length / CTMMesh.CTM_POSITION_ELEMENT_COUNT;
	        int ne = CTMMesh.CTM_NORMAL_ELEMENT_COUNT;
	
	        for (int i = 0; i < vc; ++i) {
	            float magn = intNormals[i * ne] * normalPrecision;
	            double thetaScale, theta;
	            int intPhi = intNormals[i * ne + 1];
	            double phi = intPhi * (0.5 * Math.PI) * normalPrecision;
	            if (intPhi == 0) {
	                thetaScale = 0.0f;
	            } else if (intPhi <= 4) {
	                thetaScale = Math.PI / 2.0f;
	            } else {
	                thetaScale = (2.0f * Math.PI) / intPhi;
	            }
	            theta = intNormals[i * ne + 2] * thetaScale - Math.PI;  
	            double[] n2 = new double[3];
	            n2[0] = Math.Sin(phi) * Math.Cos(theta);
	            n2[1] = Math.Sin(phi) * Math.Sin(theta);
	            n2[2] = Math.Cos(phi);
	            float[] basisAxes = Algorithm.NormalCoordSys(smoothNormals, i * ne);
	            double[] n = new double[3];
	            for (int j = 0; j < 3; ++j) {
	                n[j] = basisAxes[j] * n2[0]
	                       + basisAxes[3 + j] * n2[1]
	                       + basisAxes[6 + j] * n2[2];
	            }
	            for (int j = 0; j < 3; ++j) {
	                normals[i * ne + j] = (float) (n[j] * magn);
	            }
	        }
	
	        return normals;
	    }
	}
}

