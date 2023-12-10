using System;
using System.Collections.Generic;
using UnityEngine;

namespace _3mxImport
{
	public class RawDecoder : MeshDecoder
	{
		public static readonly int RAW_TAG = CtmReader.getTagInt("RAW\0");
	    public const int FORMAT_VERSION = 5;
	    public override CTMMesh meshDecode(MeshInfo minfo, CtmStream input)
	    {
            int vc = minfo.getVertexCount();
            V2Data[] tex = new V2Data[minfo.getUvMapCount()];
            V2Data[] att = new V2Data[minfo.getAttrCount()];
            checkTag(input.GetInt(), INDX);
            int[] indices = ReadInts(input, minfo.getTriangleCount(), 3, false);

            checkTag(input.GetInt(), VERT);
	        float[] vertices = ReadFloats(input, vc * CTMMesh.CTM_POSITION_ELEMENT_COUNT, 1);
            float[] normals = null;
	        if (minfo.hasNormals()) {
	            checkTag(input.GetInt(), NORM);
	            normals = ReadFloats(input, vc, CTMMesh.CTM_NORMAL_ELEMENT_COUNT);
	        }

            for (int i = 0; i < tex.Length; ++i)
            {
                checkTag(input.GetInt(), TEXC);
                tex[i] = readUV(vc, input);
            }

            return new CTMMesh(vertices, normals, indices, tex, att);
	    }
	
	    protected void checkTag(int readTag, int expectedTag)
	    {
	        if (readTag != expectedTag) {
	            throw new Exception("Instead of the expected data tag(\"" + CtmReader.unpack(expectedTag)
	                    + "\") the tag(\"" + CtmReader.unpack(readTag) + "\") was read!");
	        }
	    }

	    protected virtual int[] ReadInts(CtmStream input, int count, int size, bool signed)
	    {

            return input.readIntArray(count*size);
        }
        protected virtual float[] ReadFloats(CtmStream input, int count, int size)
	    {

            return input.readFloatArray(count * size);
        }


	    private V2Data readUV(int vertCount, CtmStream input)
	    {
	        string name = input.GetString();
	        string matname = input.GetString();
            float[] UVdataArray = ReadFloats(input, vertCount, CTMMesh.CTM_UV_ELEMENT_COUNT);
	
	        return new V2Data(UVdataArray);
	    }	
	    public override bool isSupported(int tag, int version)
	    {
	        return tag == RAW_TAG && version == FORMAT_VERSION;
	    }
	}
}

