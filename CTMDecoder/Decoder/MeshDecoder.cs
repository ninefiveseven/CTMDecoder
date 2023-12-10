using System;

namespace _3mxImport
{
	public abstract class MeshDecoder
	{
		public static readonly int INDX = CtmReader.getTagInt("INDX");
	    public static readonly int VERT = CtmReader.getTagInt("VERT");
	    public static readonly int NORM = CtmReader.getTagInt("NORM");
	    public static readonly int TEXC = CtmReader.getTagInt("TEXC");
	    public static readonly int ATTR = CtmReader.getTagInt("ATTR");
	
	    public abstract CTMMesh meshDecode(MeshInfo minfo, CtmStream input);
	
	    public abstract bool isSupported(int tag, int version);

	}
}

