using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

namespace _3mxImport
{
    public class CtmReader
    {
        public static readonly int OCTM = getTagInt("OCTM");        

        private static readonly MeshDecoder[] DECODER = new MeshDecoder[]{
            new RawDecoder(),
            new MG1Decoder(),
            new MG2Decoder()
        };

        private String comment;
        private readonly CtmStream input;
        //private readonly CtmInputStream input=new CtmInputStream();
        private bool decoded;

        //public static Stopwatch sw = new Stopwatch();

        public CtmReader(Stream source)
        {
            input = new CtmStream(source);
        }

        public CTMMesh decode()
        {

            if (decoded)
            {
                throw new Exception("Ctm File got already decoded");
            }
            decoded = true;

            if (input.GetInt() != OCTM)
            {
                throw new Exception("The CTM file doesn't start with the OCTM tag!");
            }


            int formatVersion = input.GetInt();
            int methodTag = input.GetInt();

            MeshInfo mi = new MeshInfo(input.GetInt(),//vertex count
                    input.GetInt(), //triangle count
                    input.GetInt(), //uvmap count
                    input.GetInt(), //attribute count
                    input.GetInt());                  //flags

            comment = input.GetString();


            // Uncompress from stream
            CTMMesh ctmMesh = null;
            foreach (MeshDecoder md in DECODER)
            {
                if (md.isSupported(methodTag, formatVersion))
                {              
                    ctmMesh = md.meshDecode(mi, input);               
                    break;
                }
            }

            if (ctmMesh == null)
            {
                throw new IOException("No sutible decoder found for Mesh of compression type: " + unpack(methodTag) + ", version " + formatVersion);
            }

            return ctmMesh;
        }

        public static String unpack(int tag)
        {
            byte[] chars = new byte[4];
            chars[0] = (byte)(tag & 0xff);
            chars[1] = (byte)((tag >> 8) & 0xff);
            chars[2] = (byte)((tag >> 16) & 0xff);
            chars[3] = (byte)((tag >> 24) & 0xff);
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(chars);
        }

        /**
	     * before calling this method the first time, the decode method has to be
	     * called.
	     * <p/>
	     * @throws RuntimeExceptio- if the file wasn't decoded before.
	     */
        public String getFileComment()
        {
            if (!decoded)
            {
                throw new Exception("The CTM file is not decoded yet.");
            }
            return comment;
        }

        public static int getTagInt(String tag)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] chars = enc.GetBytes(tag);
            if (chars.Length != 4)
                throw new Exception("A tag has to be constructed out of 4 characters!");
            return chars[0] | (chars[1] << 8) | (chars[2] << 16) | (chars[3] << 24);
        }
    }
}

