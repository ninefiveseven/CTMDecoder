using System;

namespace _3mxImport
{
	public class MG1Decoder : RawDecoder
	{
		public static readonly int MG1_TAG = CtmReader.getTagInt("MG1\0");

	    public override CTMMesh meshDecode(MeshInfo minfo, CtmStream input)
	    {
	        CTMMesh m = base.meshDecode(minfo, input);
	        restoreIndices(minfo.getTriangleCount(), m.indices);
	        return m;
	    }
	
	    public override bool isSupported(int tag, int version)
	    {
	        return tag == MG1_TAG && version == RawDecoder.FORMAT_VERSION;
	    }
	
	    protected override float[] ReadFloats(CtmStream input, int count, int size)
	    {
	        return input.GetPackedFloats(count, size);
	    }
	
	    protected override int[] ReadInts(CtmStream input, int count, int size, bool signed)
	    {
	        return input.GetPackedInts(count, size, signed);
	    }
	
	    public void restoreIndices(int triangleCount, int[] indices)
	    {
	        for (int i = 0; i < triangleCount; ++i) {
	            if (i >= 1) {
	                indices[i * 3] += indices[(i - 1) * 3];
	            }
	            indices[i * 3 + 2] += indices[i * 3];
	            if ((i >= 1) && (indices[i * 3] == indices[(i - 1) * 3])) {
	                indices[i * 3 + 1] += indices[(i - 1) * 3 + 1];
	            } else {
	                indices[i * 3 + 1] += indices[i * 3];
	            }
	        }
	    }
	}
}

