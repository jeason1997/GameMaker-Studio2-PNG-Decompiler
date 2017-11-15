using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace gms_assets
{
    internal class Program
    {
        private static string Unzip(string fName)
        {
            FileInfo fileInfo = new FileInfo(fName);
            if (fileInfo.Exists)
            {
                string folderName = fileInfo.Name.Split('.')[0];
                if (!Directory.Exists(folderName))
                {
                    ZipFile.ExtractToDirectory(fileInfo.FullName, folderName);
                }
                return folderName;
            }
            else
            {
                return null;
            }
        }

        private static void UnpackPng(string fName, bool trim)
        {
            Random rnd = new Random();
            UnpackPng("data.win", fName, trim);
        }

        private static void UnpackPng(string fName, string folderName, bool trim)
        {
            string winPath = folderName + "/" + fName;
            List<byte> bytes = new List<byte>();
            ;
            if (File.Exists(winPath))
            {
                Stopwatch sw = Stopwatch.StartNew();
                
                byte[] fBytes = File.ReadAllBytes(winPath);
                Random random = new Random();
                string fileName = null;
                for (int i = 0; i < fBytes.Length; i++)
                {
                    if (fBytes[i] == 0x89 && fBytes[i + 1] == 0x50
                        && fBytes[i + 2] == 0x4e && fBytes[i + 3] == 0x47)
                    {
                        fileName = folderName + "/" + (random.Next(0, 9999)).ToString() + ".png";
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Catched {fileName}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (fileName != null)
                    {
                        bytes.Add(fBytes[i]);
                    }

                    if (fBytes[i] == 0x42 && fBytes[i + 1] == 0x60
                        && fBytes[i + 2] == 0x82 && fileName != null)
                    {
                        using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Append)))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"Writing {fileName}");
                            Console.ForegroundColor = ConsoleColor.White;
                            writer.Write(bytes.ToArray());
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{fileName} Writed");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        if(trim)
                            TrimImage(fileName);
                        fileName = null;
                        bytes.Clear();
                    }
                }
                
                sw.Stop();
                Console.WriteLine((sw.ElapsedMilliseconds / 1000).ToString() + "s Passed.");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Completed");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("File not detected.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void Main(string[] args)
        {
            string folderName = null;
            string fileName = "data.win";
            bool trim = false;
            bool[] argTaken = new bool[2] {false, false};
            for (int i = 0; i < args.Length; i++)
            {
                string _arg = args[i].ToLower();
                if (args.Length > i)
                {
                    if (_arg.Equals("-z"))
                    {
                        folderName = Unzip(args[i + 1]);
                        if (folderName == null)
                        {
                            Console.WriteLine("Can't find zip file.");
                            return;
                        }
                        argTaken[0] = true;
                        Console.WriteLine($"Processing {folderName}");
                    }
                    else if (_arg.Equals("-f"))
                    {
                        argTaken[1] = true;
                        fileName = args[i + 1];
                        Console.WriteLine($"Processing {fileName}");
                    } else if (_arg.Equals("-t"))
                    {
                        trim = true;
                    }
                }
            }

            if (argTaken[0])
            {
                Console.WriteLine($"Unzipping {folderName}");
                UnpackPng(folderName, trim);
            }
            else if (argTaken[1])
            {
                Console.WriteLine($"Unpacking {fileName}");
                UnpackPng(fileName, trim);
            }
            else if (argTaken[0] && argTaken[1])
            {
                Console.WriteLine($"Unzipping {folderName}");
                UnpackPng(folderName, fileName, trim);
            }
            else
            {
                Console.WriteLine("gms_assets [-z zip name*] [-f filename] [-t trim image]\r\n" +
                                  "\t*required");
            }
        }
        
        
        private static void TrimImage(string fileName)
        {
            int finalX = 0;
            int finalY = 0;
            using (var fs = File.OpenRead(fileName))
            {
                Bitmap bmp = new Bitmap(fs);
                Console.WriteLine($"Original size {bmp.Width}x{bmp.Height}");
                for (int x = 0; x < bmp.Width; x += 1)
                {
                    Color clr = bmp.GetPixel(x, 0);
                    if (clr.A == 0)
                    {
                        finalX = x;
                        break;
                    }
                }
                
                for (int y = 0; y < bmp.Height; y += 1)
                {
                    Color clr = bmp.GetPixel(0, y);
                    if (clr.A == 0)
                    {
                        finalY = y;
                        break;
                    }
                }
                
                if (finalX.Equals(0)) finalX = bmp.Width;
                if (finalY.Equals(0)) finalY = bmp.Height;
                
                Console.WriteLine($"Trimmed Image {finalX}x{finalY}");
                if (finalX.Equals(bmp.Width) && finalY.Equals(bmp.Height))
                {
                    Console.WriteLine("Trimmed image equal with original image, skipping.");
                    return;
                }
                
                Bitmap bmpTarget = new Bitmap(finalX, finalY);
                
                using(Graphics g = Graphics.FromImage(bmpTarget))
                {
                    g.DrawImage(bmp, new Rectangle(0, 0, bmpTarget.Width, bmpTarget.Height),
                        new Rectangle(0, 0, finalX, finalY),
                        GraphicsUnit.Pixel);
                    g.Save();
                }
                bmpTarget.Save(fileName.Split('.')[0] + "_trim.png", ImageFormat.Png);
            }
        }
    }
}