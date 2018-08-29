//#define BACKWARDS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DataVisualizer
{
    class Program
    {
        private static int Mode = 0;        // Pixel calculation mode
        private static int Direction = 0;   // read from top/bottom of array
        private static int Width = 3;       // width unit
        private static int Height = 12;     // height unit
        private static int Scale = 1;       // scale unit

        static void Main(string[] args)
        {
            // Exit if no args have been passed
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide file/folder as argument");
                Console.ReadKey();
                Environment.Exit(1);
            }
            
            ParseArgs(args);            
        }

        private static void ParseArgs(string[] args)
        {
            var file = new FileInfo(args[0]);
            var folder = new DirectoryInfo(args[0]);

            if (folder.Exists)
            {
                foreach (var item in folder.EnumerateFiles())
                {
                    ProcessFile(item.FullName);
                }
            }
            else if (file.Exists)
            {
                Process.Start(ProcessFile(file.FullName));
            }
            else
            {
                throw new Exception("Argument provided is neither a filer or a folder");
            }
        }


        private static string ProcessFile(string file)
        {
            Console.WriteLine($"Processing: {file}");

            FileInfo item = new FileInfo(file);

            byte[] bytes = GetByteArray(item.FullName);

            var outputDir = $"{item.DirectoryName}\\output";
            var outputFile = $"{outputDir}\\{item.Name}.bmp";

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            AppendFile(bytes, outputFile);

            return outputFile;
        }

        private static byte[] GetBytes(int value, int length = 4)
        {
            byte[] bytes = new byte[length];
            byte[] val = BitConverter.GetBytes(value);

            for (int i = 0; i < length; i++)
            {
                try
                {
                    bytes[i] = val[i];
                }
                catch (IndexOutOfRangeException)
                {
                    bytes[i] = 0x00;
                }
            }

            return bytes;
        }

        private static byte[] ConstructFileHeader(byte[] bytes,int _width = 5, int _height = 5,int scale = 1)
        {
            var sqrt = Math.Sqrt(bytes.Length);
            var t = (int)Math.Round(sqrt, 0);

            var p = (t / (_height + _width) * scale);
            var h = p * _height;
            var w = p * _width;

            Debug.WriteLine(p);

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
            header[2] = fileSize[0]; // 02
            header[3] = fileSize[1]; // 03
            header[4] = fileSize[2]; // 04
            header[5] = fileSize[3]; // 05

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
            header[18] = width[0];  // 12
            header[19] = width[1];  // 13        
            header[20] = width[2];  // 14
            header[21] = width[3];  // 15

            // Height of the bitmap in pixels. Positive for bottom to top pixel order
            header[22] = height[0]; // 16
            header[23] = height[1]; // 17
            header[24] = height[2]; // 18  
            header[25] = height[3]; // 19

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
            header[34] = rawSize[0]; // 22
            header[35] = rawSize[1]; // 23
            header[36] = rawSize[2]; // 24
            header[37] = rawSize[3]; // 25

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

            // Number of colors in the palette
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

            return header;
        }

        private static void AppendFile(byte[] bytes, string file)
        {
            try
            {
                File.WriteAllBytes(file, bytes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static byte[] GetByteArray(string filePath)
        {
            var byteList = new List<byte>();

            byte[] bytes = File.ReadAllBytes(filePath);
            byte[] header = ConstructFileHeader(bytes, Width, Height, Scale);

            for (int i = 0; i < header.Length; i++)
            {
                byteList.Add(header[i]);
            }

            #region ForLoop conditons

            int iterator = Direction == 0 ? 0 : bytes.Length - 1;
            int right = Direction == 0 ? bytes.Length : 0;

            void increment(ref int i)
            {
                switch (Direction)
                {
                    case 0:
                        i++;
                        break;
                    case 1:
                        i--;
                        break;
                    default:
                        break;
                }
            }

            bool condition(ref int i,  int j)
            {
                switch (Direction)
                {
                    case 0:
                        return i < j;
                    case 1:
                        return i > j;        
                    default:
                        return false;                        
                }
            }

            #endregion

            for (int i = iterator; condition(ref i, right); increment(ref i))
            {
                Debug.WriteLine(i);
                switch (Mode)
                {
                    case 0:
                        byteList.Add(bytes[i]);
                        break;
                    case 1:
                        byteList.Add(bytes[i]);
                        byteList.Add(bytes[i]);
                        byteList.Add(bytes[i]);
                        break;
                    case 2:
                        int h = int.Parse(bytes[i].ToString(), System.Globalization.NumberStyles.HexNumber);
                        var colour = HsvToRgb(h, 1, 1);
                        byteList.Add(BitConverter.GetBytes(colour.R)[0]);
                        byteList.Add(BitConverter.GetBytes(colour.G)[0]);
                        byteList.Add(BitConverter.GetBytes(colour.B)[0]);
                        break;
                    default:
                        break;
                }
          
            }

            return byteList.ToArray();
        }

        /// <summary>
        /// Takes Hue, Saturation and Vibrance. returns an RGB value
        /// ========================================================
        /// This is not my code, however i have modified it as best i can to clean it up.
        /// Im still new to colour conversion. and im sure there is an internal method of doing this.
        /// I will refactor in the future.
        /// 
        /// I have added an RGB class to return from this, it originally used out params.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="S"></param>
        /// <param name="V"></param>
        /// <returns></returns>
        private static RGB HsvToRgb(double h, double S, double V)
        {
            double R, G, B;
            double H = h;
            int r, g, b;

            while (H < 0)
            {
                H += 360;
            }

            while (H >= 360)
            {
                H -= 360;
            }
            
            if (V <= 0)
            {
                R = G = B = 0;
            }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));

                switch (i)
                {
                    // Red is the dominant color
                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    // Green is the dominant color
                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;
                    // Blue is the dominant color
                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;
                    // Red is the dominant color
                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;
                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.
                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }

            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));

            RGB rgb = new RGB(r, g, b);

            return rgb;
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        private static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

        public class RGB
        {
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }    
            
            public RGB(int r, int g, int b)
            {
                R = r;
                G = g;
                B = b;
            }
        }

    }
}
