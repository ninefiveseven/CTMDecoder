using System;

namespace _3mxImport
{
	public class V2Data
	{
		public static readonly float STANDARD_UV_PRECISION = 1f / 4096f;
	    public static readonly float STANDARD_PRECISION = 1f / 256;
	    public readonly float[] values;   // Attribute/UV coordinate values (per vertex)
	
	    public V2Data(float[] values) {
	        this.values = values;
	    }
	
	 
	}
}

