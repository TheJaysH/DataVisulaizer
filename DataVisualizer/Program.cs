using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace DataVisualizer
{
    class Program
    {
        private static bool Reverse = true;     // read from top/bottom of array
        private static bool Random = true;
        private static readonly int Mode = 0;            // Pixel calculation mode [0 = byte for byte, 1 = greyscale, 2 = HSV]
        private static readonly int Width = 2;           // width unit (aspect ratio)
        private static readonly int Height = 2;         // height unit (aspect ratio)

        static void Main(string[] args)
        {
            //if (Random)
            //{
            //    Gener
            //}


            //Console.WriteLine("Number Of Logical Processors: {0}", Environment.ProcessorCount);
            // Exit if no args have been passed
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide file/folder as argument");
                Console.ReadKey();
                Environment.Exit(1);
            }

            ParseArgs(args);
        }

        private static void ProcessFile(object param)
        {
            string file = (string)param;

            ProcessFile(file);
        }

        private static void ParseArgs(string[] args)
        {
            var file = new FileInfo(args[0]);
            var folder = new DirectoryInfo(args[0]);

            if (folder.Exists)
            {
                var files = folder.EnumerateFiles();
                //Parallel.ForEach(files, (f) => ProcessFile(f.FullName));
                foreach (var f in files)
                {
                    ProcessFile(f.FullName);
                }
            }
            else if (file.Exists)
            {
                var output = ProcessFile(file.FullName);
                Process.Start(output);
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

        public static byte[] GetRandomBytes()
        {
            var ran = new Random().Next(10000,1000000);
            var bytes = new List<byte>();

            for (int i = 0; i < ran; i++)
            {
                var b = new Random().Next(0,255);
                bytes.AddRange(BitConverter.GetBytes(b));

            }

            return bytes.ToArray();

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

        private static byte[] ConstructFileHeader(byte[] bytes,int _width, int _height)
        {
            var sqrt = Math.Sqrt(bytes.Length);
            var t = (int)Math.Round(sqrt, 0);

            var p = (t / (_height + _width));
            var h = p * _height;
            var w = p * _width;

            //Debug.WriteLine(p);

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
            List<byte> bytes = new List<byte>(File.ReadAllBytes(filePath));
            //List<byte> bytes = new List<byte>(GetRandomBytes());
            List<byte> byteList = new List<byte>();

            if (Reverse) bytes.Reverse();

            byte[] header = ConstructFileHeader(bytes.ToArray(), Width, Height);

            for (int i = 0; i < header.Length; i++)
            {
                byteList.Add(header[i]);
            }
    


            for (int i = 0; i < bytes.Count; i++)
            {
                var value = int.Parse(bytes[i].ToString(), System.Globalization.NumberStyles.HexNumber);
                //PlayBeep((ushort)(value * 50), 7);
                //Debug.WriteLine(bytes[i]);



                var sine20Seconds = new SignalGenerator()
                {
                    Gain = 0.2,
                    Frequency = 10 * value,
                    Type = SignalGeneratorType.Sin
                }
                .Take(TimeSpan.FromMilliseconds(1));
                using (var wo = new WaveOutEvent())
                {
                    wo.Init(sine20Seconds);
                    wo.Play();
                    while (wo.PlaybackState == PlaybackState.Playing)
                    {
                        //Thread.Sleep(1);
                    }
                }

                switch (Mode)
                {
                    case 0:
                        //byteList.Add(bytes[i - 1]);

                        byteList.Add(bytes[i]);
                        //byteList.Add(bytes[i-2]);
                        //byteList.Add(bytes[i-3]);
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

        public static void PlayBeep(UInt16 frequency, int msDuration, UInt16 volume = 16383)
        {
            var mStrm = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(mStrm);

            const double TAU = 2 * Math.PI;
            int formatChunkSize = 16;
            int headerSize = 8;
            short formatType = 1;
            short tracks = 1;
            int samplesPerSecond = 44100;
            short bitsPerSample = 16;
            short frameSize = (short)(tracks * ((bitsPerSample + 7) / 8));
            int bytesPerSecond = samplesPerSecond * frameSize;
            int waveSize = 4;
            int samples = (int)((decimal)samplesPerSecond * msDuration / 1000);
            int dataChunkSize = samples * frameSize;
            int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;
            // var encoding = new System.Text.UTF8Encoding();
            writer.Write(0x46464952); // = encoding.GetBytes("RIFF")
            writer.Write(fileSize);
            writer.Write(0x45564157); // = encoding.GetBytes("WAVE")
            writer.Write(0x20746D66); // = encoding.GetBytes("fmt ")
            writer.Write(formatChunkSize);
            writer.Write(formatType);
            writer.Write(tracks);
            writer.Write(samplesPerSecond);
            writer.Write(bytesPerSecond);
            writer.Write(frameSize);
            writer.Write(bitsPerSample);
            writer.Write(0x61746164); // = encoding.GetBytes("data")
            writer.Write(dataChunkSize);
            {
                double theta = frequency * TAU / (double)samplesPerSecond;
                // 'volume' is UInt16 with range 0 thru Uint16.MaxValue ( = 65 535)
                // we need 'amp' to have the range of 0 thru Int16.MaxValue ( = 32 767)
                double amp = volume >> 2; // so we simply set amp = volume / 2
                for (int step = 0; step < samples; step++)
                {
                    short s = (short)(amp * Math.Sin(theta * (double)step));
                    writer.Write(s);
                }
            }

            mStrm.Seek(0, SeekOrigin.Begin);
            new System.Media.SoundPlayer(mStrm).Play();
            writer.Close();
            mStrm.Close();
            mStrm.Dispose();
        }


        /// <summary>
        /// Takes Hue, Saturation and Vibrance. returns an RGB value.
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
        private static RGB HsvToRgb(double H, double S, double V)
        {
            double R, G, B;
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
                    case 6:
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
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.
                    default:
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
