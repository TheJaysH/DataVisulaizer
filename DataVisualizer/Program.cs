﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DataVisualizer
{
    class Program
    {
       
        private static List<byte> ByteList = new List<byte>();

     
        private static string FileOutput { get; set; }
        private static string File_Input { get; set; }
        private static string Input_Directory { get; set; }

        /// <summary>
        /// Main entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            Input_Directory = @"C:\Temp\Image";

            try
            {
                Input_Directory = args[0];
            }
            catch
            {
                Console.WriteLine($"Input Directory not defined. Generating Random Image... ");
            }
            
            // No comand line args supplied. generate random image in working dir
            if (string.IsNullOrEmpty(Input_Directory))
            {
                byte[] bytes = ByteArray();
                FileOutput = $@"{Environment.CurrentDirectory}\Random.bmp";

                // then append it to a file
                AppendFile(bytes);
            }
            else
            {
                // Directory of Files to convert
                DirectoryInfo dir = new DirectoryInfo(Input_Directory);

                // create output directory
                if (!Directory.Exists($@"{dir.FullName}\output"))
                    Directory.CreateDirectory($@"{dir.FullName}\output");

                // Iterate over each file
                foreach (var item in dir.EnumerateFiles())
                {
                    Console.WriteLine($"Processing: {item.Name}");

                    // clear out list
                    ByteList.Clear();

                    FileOutput = $@"{dir.FullName}\output\{item.Name.Substring(0, item.Name.IndexOf("."))}.bmp";

                    // first lets generate the byte array
                    byte[] bytes = ByteArray(item.FullName);

                    // then append it to a file
                    AppendFile(bytes);
                }
            }
            
        }

        private static byte[] GetBytes(int value, int length = 4)
        {
            byte[] bytes = new byte[length];
            byte[] tmp = BitConverter.GetBytes(value);

            for (int i = 0; i < length; i++)
            {
                try
                {
                    bytes[i] = tmp[i];
                }
                catch (IndexOutOfRangeException)
                {
                    bytes[i] = 0x00;
                }

                Debug.WriteLine(i + " " + bytes[i]);
            }

            return bytes;
        }

        private static byte[] ConstructFileHeader(byte[] bytes)
        {
            var sqrt = Math.Sqrt(bytes.Length);  
            
            var t = (int)Math.Floor(sqrt);
            var h = t / 2;
            var w = t / 2; 

            byte[] width = GetBytes(w);
            byte[] height = GetBytes(h);

            byte[] fullSize = GetBytes(bytes.Length + 54);
            byte[] rawSize = GetBytes(bytes.Length);
           
            return ConstructFileHeader(fullSize, width, height, rawSize);
        }

        private static byte[] ConstructFileHeader(byte[] fileSize, byte[] width, byte[] height, byte[] rawSize)
        {
            var header = new byte[54];

            #region BMP header
            // ID field (0x42 = B, 0x4D = M)
            header[0] = 0x42;   // 00
            header[1] = 0x4D;   // 01

            // the size of the BMP file in bytes
            header[2] = fileSize[0];//0x46;   // 02
            header[3] = fileSize[1];//0x00;   // 03
            header[4] = fileSize[2];//0x00;   // 04
            header[5] = fileSize[3];//0x00;   // 05

            // Reserved; actual value depends on the application that creates the image
            header[6] = 0x00;   // 06
            header[7] = 0x00;   // 07
            header[8] = 0x00;   // 08
            header[9] = 0x00;   // 09

            // The offset, i.e.starting address, of the byte where the bitmap image data(pixel array) can be found.
            header[10] = 0x36;  // 0A
            header[11] = 0x00;  // 0B
            header[12] = 0x00;  // 0C
            header[13] = 0x00;  // 0D
            #endregion

            #region DIB header
            // Number of bytes in the DIB header (from this point)
            header[14] = 0x28;  // 0E
            header[15] = 0x00;  // 0F
            header[16] = 0x00;  // 10
            header[17] = 0x00;  // 11

            // Width of the bitmap in pixels
            header[18] = width[0]; // 0xFF;  // 12
            header[19] = width[1]; // 0x02;  // 13        
            header[20] = width[2]; // 0x00;  // 14
            header[21] = width[3]; // 0x00;  // 15

            // Height of the bitmap in pixels. Positive for bottom to top pixel order
            header[22] = height[0]; //0xFF;  // 16
            header[23] = height[1]; //0x02;  // 17
            header[24] = height[2]; //0x00;  // 18  
            header[25] = height[3]; //0x00;  // 19

            // Number of color planes being used
            header[26] = 0x01;  // 1A     
            header[27] = 0x00;  // 1B

            // Number of bits per pixel
            header[28] = 0x18;  // 1C
            header[29] = 0x00;  // 1D

            // BI_RGB, no pixel array compression used
            header[30] = 0x00;  // 1E
            header[31] = 0x00;  // 1F
            header[32] = 0x00;  // 20
            header[33] = 0x00;  // 21

            // Size of the raw bitmap data (including padding)
            header[34] = rawSize[0];//0xff;  // 22
            header[35] = rawSize[1];//0x00;  // 23
            header[36] = rawSize[2];//0x00;  // 24
            header[37] = rawSize[3];//0x00;  // 25

            #region Print resolution of the image
            header[38] = 0x00;  // 26
            header[39] = 0x00;  // 27
            header[40] = 0x00;  // 28
            header[41] = 0x00;  // 29

            header[42] = 0x00;  // 2A
            header[43] = 0x00;  // 2B
            header[44] = 0x00;  // 2C
            header[45] = 0x00;  // 2D
            #endregion

            // 	Number of colors in the palette
            header[46] = 0x00;  // 2E
            header[47] = 0x00;  // 2F
            header[48] = 0x00;  // 30
            header[49] = 0x00;  // 31

            // 0 means all colors are important
            header[50] = 0x00;  // 32
            header[51] = 0x00;  // 33
            header[52] = 0x00;  // 34
            header[53] = 0x00;  // 35
            #endregion

            #region 2x2 Test Pixels
            //header[54] = 0x00;  // 36
            //header[55] = 0x00;  // 37
            //header[56] = 0xFF;  // 38

            //header[57] = 0xFF;  // 39
            //header[58] = 0xFF;  // 3A
            //header[59] = 0xFF;  // 3B

            //header[60] = 0x00;  // 3C
            //header[61] = 0x00;  // 3D

            //header[62] = 0xFF;  // 3E
            //header[63] = 0x00;  // 3F
            //header[64] = 0x00;  // 40

            //header[65] = 0x00;  // 41
            //header[66] = 0xFF;  // 42
            //header[67] = 0x00;  // 43

            //header[68] = 0x00;  // 3C
            //header[69] = 0x00;  // 3D
            #endregion

            return header;
        }

        private static void AppendFile(byte[] bytes)
        {
            // delete the file
            File.Delete(FileOutput);

            // append our bytes
            File.WriteAllBytes(FileOutput, bytes);
        }

        private static byte[] ByteArray(string file = null)
        {
            byte[] bytes = File.ReadAllBytes(file);
            byte[] header = ConstructFileHeader(bytes);

            for (int i = 0; i < header.Length; i++)
            {
                ByteList.Add(header[i]);
            }
            
            for (int i = 0; i < bytes.Length; i++)
            {
                ByteList.Add(bytes[i]);
            }

            // cool our list is full... now return it as an array to be appened to a file
            return ByteList.ToArray();
        }
      
    }
}
