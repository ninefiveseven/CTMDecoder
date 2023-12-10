using System;

namespace _3mxImport
{
	public class CTMGrid
	{
		/**
	     * Axis-aligned bounding box for the grid
	     */
	    private readonly float[] min, max;
	    /**
	     * How many divisions per axis (minimum 1).
	     */
	    private readonly int[] division;
	
	    public static CTMGrid fromStream(CtmStream input){
	        return new CTMGrid(input.readFloatArray(3),
	                        input.readFloatArray(3),
	                        input.readIntArray(3));
	    }
	
	    public CTMGrid(float[] min, float[] max, int[] division) {
	        this.min = min;
	        this.max = max;
	        this.division = division;
	    }
	
	
	    public bool check() {
	        if (min.Length != 3) {
	            return false;
	        }
	        if (max.Length != 3) {
	            return false;
	        }
	        if (division.Length != 3) {
	            return false;
	        }
	
	        foreach(int d in division) {
	            if (d < 1) {
	                return false;
	            }
	        }
	        for (int i = 0; i < 3; i++) {
	            if (max[i] < min[i]) {
	                return false;
	            }
	        }
	        return true;
	    }
	
	    public float[] getMin() {
	        return min;
	    }
	
	    public float[] getMax() {
	        return max;
	    }
	
	    public int[] getDivision() {
	        return division;
	    }
	
	    public float[] getSize() {
	        float[] size = new float[3];
	        for (int i = 0; i < 3; i++) {
	            size[i] = (max[i] - min[i]) / division[i];
	        }
	        return size;
	    }
	}
}

