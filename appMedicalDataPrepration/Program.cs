using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wrpMedicalImageIO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.IO;
using System.Data.SqlClient;
using System.Collections;
using System.Xml;
using System.Drawing.Imaging;
using System.Data.OleDb;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using System.Xml.Linq;



namespace appMedicalDataPrepration
{
    class Program
    {
        Boolean Imageisnull = true;

        
        static void Main(string[] args)
        {


            var xdoc = new XmlDocument();
            xdoc.Load("configAppMedDataPrepration.xml");
            var xnde = xdoc.SelectSingleNode("config/Root");
            var Root = xnde.InnerText;
            xnde = xdoc.SelectSingleNode("config/dataFolder");
            var dataFolder = xnde.InnerText;
            xnde = xdoc.SelectSingleNode("config/OutputFolder");
            var destFolder = xnde.InnerText;
            var outputImagesize = 500;
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string data = "";

            data=Console.ReadLine();

            dataFolder = data;
            //BratsPrepration();


            #region survivaldata
            //string SVfile = @"Y:\PhD-research\DataSet\Survival\Brats17TrainingData\survival_data.csv";
                //var reader = new StreamReader(File.OpenRead(SVfile));
                //List<string> listImg = new List<string>();
                //while (!reader.EndOfStream)
                //{
                //    var line = reader.ReadLine();
                //    var values = line.Split(',');
                //    var strfldr = values[0];

                //    string[] allfolders = Directory.GetDirectories(dataFolder);
                //    foreach (var folder in allfolders)
                //    {

                //        string fldrName = Path.GetFileNameWithoutExtension(folder);

                //        if (strfldr.CompareTo(fldrName) == 0)
                //        {
                //            var allfile = Directory.GetFiles(folder, "*nii.gz");// var allfile = Directory.GetFiles(dataFolder, "*seg*");
                //            foreach (var file in allfile)
                //            {
                //                var fl = file;
                //                Console.WriteLine(fl);
                //                ConvertCase(fl, destFolder, outputImagesize);

                //            }
                //        }
                //   }

            //}


            #endregion


            string[] allfolders = Directory.GetDirectories(dataFolder);
            var allfile = Directory.GetFiles(dataFolder, "*nii.gz");
            foreach (var file in allfile)
            {
                var fl = file;
                Console.WriteLine(fl);
                ConvertCase(fl, destFolder, outputImagesize);

            }

            #region convert to nifti

            //foreach (var file in allfolders)
            //{
            //    var fl = file + "\\";
            //    Console.WriteLine(fl);

            //    convert2dtonifti(fl);

            //}

            //convert2dtoniftiCT(dataFolder);
            #endregion

        }

        static void ConvertCase(string fl, string destFolder,int outputImagesize)
        {
            Console.WriteLine("{0}", Path.GetFileNameWithoutExtension(fl));
            var img = MedicalIO.ReadImage(fl);

            Bitmap bmp;
            ROI Output; Output = new ROI();
            RangeLevel valuerange; valuerange = new RangeLevel();

            #region nx

            if (fl.Contains("seg"))
            {
                ////////printing GT.png*
                for (int i = 0; i < img.nx; i++)
                {
                    bmp = img.CoronalPlaneReconstruction(i);
                    string Name = Path.GetFileNameWithoutExtension(fl);
                    Name = Path.GetFileNameWithoutExtension(Name);
                    Name = Name + ".nx." + i.ToString() + ".png";
                    string savename = destFolder + Name;
                    savename = destFolder + Name;
                    bmp = printGTSlides(bmp, Name, savename);
                    bmp.Save(savename);

                }

            }
            else
            {
                ////////////for printing all stack of MRI in BRATS
                for (int i = 0; i < img.nx; i++)
                {
                    bmp = img.CoronalPlaneReconstruction(i);
                    string Name = Path.GetFileNameWithoutExtension(fl);
                    Name = Path.GetFileNameWithoutExtension(Name);
                    Name = Name + ".nx." + i.ToString() + ".png";
                    valuerange = Findrange(bmp);
                    int min_level = valuerange.min_level;
                    int max_level = valuerange.max_level;
                    if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                        bmp = ConvertTo24bppRgb(bmp, min_level, max_level);
                    string savename = destFolder + Name;

                    //if (!isImagEmpty(bmp))
                    //{ bmp.Save(savename); }

                    bmp.Save(savename);
                }

            }

        

             #endregion

            #region ny

            if (fl.Contains("label"))
            {
                //////////printing GT.png*
                for (int i = 0; i < img.ny; i++)
                {
                    bmp = img.SagittalPlaneReconstruction(i);
                    string Name = Path.GetFileNameWithoutExtension(fl);
                    Name = Path.GetFileNameWithoutExtension(Name);
                    Name = Name + ".ny." + i.ToString() + ".png";
                    string savename = destFolder + Name;
                    savename = destFolder + Name;
                    bmp = printGTSlides(bmp, Name, savename);
                    bmp.Save(savename);

                }

            }
            else
            {
                //////////////for printing all stack of MRI in BRATS
                for (int i = 0; i < img.ny; i++)
                {

                    bmp = img.SagittalPlaneReconstruction(i);
                    string Name = Path.GetFileNameWithoutExtension(fl);
                    Name = Path.GetFileNameWithoutExtension(Name);
                    Name = Name + ".ny." + i.ToString() + ".png";
                    valuerange = Findrange(bmp);
                    int min_level = valuerange.min_level;
                    int max_level = valuerange.max_level;
                    if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                        bmp = ConvertTo24bppRgb(bmp, min_level, max_level);
                    string savename = destFolder + Name;
                    //if (!isImagEmpty(bmp))
                    //{ bmp.Save(savename); }

                    bmp.Save(savename);
                }

            }


            #endregion

            #region nz


            if (fl.Contains("seg"))
            {
                //////////printing GT.png*
                for (int i = 0; i < img.nz; i++)
                {
                    bmp = img.TransversalPlaneReconstruction(i);
                    string Name = Path.GetFileNameWithoutExtension(fl);
                    Name = Path.GetFileNameWithoutExtension(Name);
                    Name = Name + ".nz." + i.ToString() + ".png"; 
                    string savename = destFolder + Name;
                    savename = destFolder + Name;
                    bmp = printGTSlides(bmp, Name, savename);
             

                }

            }
            else 
            {
                ////////////for printing all stack of MRI in BRATS
                for (int i = 0; i < img.nz; i++)
                {
                    bmp = img.TransversalPlaneReconstruction(i);
                    string fldr = Path.GetDirectoryName(fl);
                    string strfldr = Path.GetFileNameWithoutExtension(fldr);
                    string Name = Path.GetFileNameWithoutExtension(fl);
                    string fldrName = Path.GetFileNameWithoutExtension(Name);
                    Name = fldrName + ".nz." + i.ToString() + ".png";
                    valuerange = Findrange(bmp);
                    int min_level = valuerange.min_level;
                    int max_level = valuerange.max_level;

                    string savefldr = destFolder + strfldr + "\\";
                    if (!Directory.Exists(savefldr))
                    {
                        Directory.CreateDirectory(savefldr);

                        if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                            bmp = ConvertTo24bppRgb(bmp, min_level, max_level);


                        string savename = savefldr + Name;

                        bmp.Save(savename);
                    }
                }


            }



            #endregion


            Console.WriteLine("finished");
        }
  
        static void convert2dtonifti(string datafolder)
        {
            string fl = datafolder; 
            Console.WriteLine("{0}", Path.GetFileNameWithoutExtension(fl));

            var Allpng = Directory.GetFiles(fl, "*.png");
            var fixedname =  Path.GetFileName(Path.GetDirectoryName(fl)) + "_out.nz."; // "Brats17_CBICA_AQY_1_contest.nz.";

            System.String savefilename = fl + Path.GetFileName(Path.GetDirectoryName(fl))+ ".nii.gz";

            Bitmap bmpone = new Bitmap(Allpng[0]);
            int width = bmpone.Width; 
            int height = bmpone.Height;
            int numberofslides = Allpng.Length;
            int numberbyteper = 1; 

            var med3img = new MedicalImage(width,height,numberofslides,numberbyteper);

            for (int i = 1; i <= numberofslides; i++)
            {
        
               //var fname = Allpng[i];
                var fname = fl + fixedname + (i-1) + ".png";
                Console.Write(i);
                Console.Write("    ");
                Console.WriteLine(fname, i);

               Bitmap bmp = new Bitmap(fname);
               med3img.Setsclides(bmp, i-1);

             }
            string saveout = "Y:\\PhD-research\\DataSet\\braindatabase\\BRATS041017\\Out1\\Refine\\" + Path.GetFileName(Path.GetDirectoryName(fl)) + ".nii.gz";
            savefilename = saveout;
            med3img.SavetoNifti( savefilename);
        }

        static void convert2dtoniftiCT(string datafolder)
        {
            string fl = datafolder; //@"D:\code-nifti\volume-0.nz.48\";
            Console.WriteLine("{0}", Path.GetFileNameWithoutExtension(fl));

            var Allpng = Directory.GetFiles(fl, "*.png");
            var fixedname = Path.GetFileName(Path.GetDirectoryName(fl)) + ".nz."; // "volume-0.nz.";

            System.String savefilename = fl + Path.GetFileName(Path.GetDirectoryName(fl)) + ".nii.gz";

            Bitmap bmpone = new Bitmap(Allpng[0]);
            int width = bmpone.Width;
            int height = bmpone.Height;
            int numberofslides = Allpng.Length;
            int numberbyteper = 1;

            var med3img = new MedicalImage(width, height, numberofslides, numberbyteper);

            for (int i = 0; i < numberofslides ; i++)
            {

                //var fname = Allpng[i];
                var fname = fl + fixedname + (i +45) + ".png";
                Console.Write(i);
                Console.Write("    ");
                Console.WriteLine(fname, i);

                Bitmap bmp = new Bitmap(fname);

                med3img.Setsclides(bmp, i);

            }
            string saveout = "Y:\\PhD-research\\DataSet\\LiTS\\Konstantin-cGAN.101102017\\trainval_output\\volume-0\\" + Path.GetFileName(Path.GetDirectoryName(fl)) + ".nii.gz";
            savefilename = saveout;
            med3img.SavetoNifti(savefilename);
        }

        public static System.Drawing.Bitmap creatvectors2d(Bitmap bmp)
        {
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {

                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new byte[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[3 * npixels];

                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    double y = Math.Floor(Convert.ToDouble(it) / Convert.ToDouble(bmp.Width));
                    double x = it - (y * Convert.ToDouble(bmp.Width));
                }
            }

            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new byte[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[1 * npixels];

                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    double y = Math.Floor(Convert.ToDouble(it) / Convert.ToDouble(bmp.Width));
                    double x = it - (y * Convert.ToDouble(bmp.Width));
                }
            }

            return bmp;
        } 

        public static RangeLevel Findrange(Bitmap bmp)
        {
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new short[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                int min_val=5000;int max_val=0;
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    var val = Convert.ToInt32(inval);
                    if (val < min_val )
                    {
                      min_val = val;
                    }
                    if (val > max_val)
                    {
                        max_val = val;
                    }
               
                 }
         
                RangeLevel result; result = new RangeLevel();
                result.max_level = max_val + 200; /////for all images is : max_val+200;
                result.min_level = min_val ; /////for all images is : min_val;
                return result;
          }



            else if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new float[npixels];
                //var values = new byte[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                int min_val = 5000; int max_val = 0;
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    var val = Convert.ToInt32(inval);
                    if (val < min_val)
                    {
                        min_val = val;
                    }
                    if (val > max_val)
                    {
                        max_val = val;
                    }

                }

                RangeLevel result; result = new RangeLevel();
                result.max_level = max_val + 200; /////for all images is : max_val+200;
                result.min_level = min_val; /////for all images is : min_val;
                return result;
            }

            else if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format64bppArgb)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new double[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                int min_val = 5000; int max_val = 0;
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];

                    var val = Convert.ToInt32(inval);

                    if (val < min_val)
                    {
                        min_val = val;
                    }
                    if (val > max_val)
                    {
                        max_val = val;
                    }

                }

                RangeLevel result; result = new RangeLevel();
                result.max_level = max_val + 200; /////for all images is : max_val+200;
                result.min_level = min_val; /////for all images is : min_val;
                return result;
            }

            else
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new byte[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                int min_val = 5000; int max_val = 0;
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    var val = Convert.ToInt32(inval);
                    if (val < min_val)
                    {
                        min_val = val;
                    }
                    if (val > max_val)
                    {
                        max_val = val;
                    }

                }

                RangeLevel result; result = new RangeLevel();
                result.max_level = max_val + 200; /////for all images is : max_val+200;
                result.min_level = min_val; /////for all images is : min_val;
                return result;
            }




        }

        static System.Drawing.Bitmap ConvertTo24bppRgbd(System.Drawing.Bitmap bmp, int min_level, int max_level)
        {
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                return bmp;
            }
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new short[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[3 * npixels];
                var norfactor = (max_level - min_level) / 255;
                if (norfactor == 0)
                    norfactor = 1;
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    var val = Convert.ToInt32(inval);
                    val = (val - min_level) / norfactor;
                    if (val > 255)
                        val = Convert.ToByte(255);
                    val = it % bmp.Width;
                    var val8 = Convert.ToByte(val);
                    for (int itc = 0; itc < 3; itc++)
                    {
                        outvalues[it * 3 + itc] = val8;
                    }
                }
                var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 3 * npixels);
                outbmp.UnlockBits(outbmpdata);
                outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                return outbmp;
            }

            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new float[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[1 * npixels];
                var norfactor = (max_level - min_level) / 255;
                if (norfactor == 0)
                    norfactor = 1;
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];

                    var val = Convert.ToInt32(inval);
                    val = (val - min_level) / norfactor;
                    if (val > 255)
                        val = Convert.ToByte(255);
                    //if (it % 230 == 100) { val = 255; }
                    //val =  it % bmp.Width ; 
                    var val8 = Convert.ToByte(val);
                    for (int itc = 0; itc < 1; itc++)
                    {
                        outvalues[it * 1 + itc] = val8;
                    }
                }

                var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 1 * npixels);
                outbmp.UnlockBits(outbmpdata);

                var outbmp2 = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (var gr = Graphics.FromImage(outbmp2))
                {
                    gr.DrawImage(outbmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    return outbmp2;
                }
                outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                return outbmp;
            }

            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format64bppArgb)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new double[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[1 * npixels];
                var norfactor = (max_level - min_level) / 255;
                if (norfactor == 0)
                    norfactor = 1;
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];

                    var val = Convert.ToDouble(inval);
                    val = (val - min_level) / norfactor;
                    if (val > 255)
                        val = Convert.ToByte(255);
                    //if (it % 230 == 100) { val = 255; }
                    //val =  it % bmp.Width ; 
                    var val8 = Convert.ToByte(val);
                    for (int itc = 0; itc < 1; itc++)
                    {
                        outvalues[it * 1 + itc] = val8;
                    }
                }

                var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 1 * npixels);
                outbmp.UnlockBits(outbmpdata);

                var outbmp2 = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (var gr = Graphics.FromImage(outbmp2))
                {
                    gr.DrawImage(outbmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    return outbmp2;
                }
                outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                return outbmp;
            }

            else
            {
                throw new Exception("Pixel format not supported!!!");
            }
        }

        static System.Drawing.Bitmap ConvertTo8bpp(System.Drawing.Bitmap bmp)
        {
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                return bmp;
            }
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new short[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[3 * npixels];
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    var val = Convert.ToInt32(inval);
                    
                    val = it % bmp.Width;
                    var val8 = Convert.ToByte(val);
                    //for (int itc = 0; itc < 3; itc++)
                    //{
                    //    outvalues[it * 1 + itc] = val8;
                    //}
                }
                var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 3 * npixels);
                outbmp.UnlockBits(outbmpdata);
                outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                return outbmp;
            }

            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new float[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[1 * npixels];
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];

                    var val = Convert.ToInt32(inval);
                    var val8 = Convert.ToByte(val);
                    for (int itc = 0; itc < 1; itc++)
                    {
                        outvalues[it * 1 + itc] = val8;
                    }
                }

                var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 1 * npixels);
                outbmp.UnlockBits(outbmpdata);

                var outbmp2 = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (var gr = Graphics.FromImage(outbmp2))
                {
                    gr.DrawImage(outbmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    return outbmp2;
                }
                outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                return outbmp;
            }
            else
            {
                throw new Exception("Pixel format not supported!!!");
            }
        }
  
        static bool isImagEmpty(Bitmap bmp)
        {
            
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            byte[] r = new byte[bytes / 3];
            byte[] g = new byte[bytes / 3];
            byte[] b = new byte[bytes / 3];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, bytes);
            bmp.UnlockBits(bmpData);

            int count = 0;
            int stride = bmpData.Stride;
            int nonzeropixelcount = 0;

            for (int row = 0; row < bmpData.Height; row++)
            {
                for (int column = 0; column < bmpData.Width; column++)
                {
                    b[count] = (byte)(rgbValues[(row * stride) + (column * 3)]);
                    g[count] = (byte)(rgbValues[(row * stride) + (column * 3) + 1]);
                    r[count] = (byte)(rgbValues[(row * stride) + (column * 3) + 2]);
                    //Console.WriteLine("{0}\t{1}\t{2}", r[count] , g[count] , b[count]);
                    if (b[count] > 0 & r[count] > 0 & g[count] > 0)
                    {
                        nonzeropixelcount = nonzeropixelcount + 1;
                    }
                    count++;
                }
            }

            int pixelcount = bmpData.Height * bmpData.Width;
            int minNonZeroPixelsCount = pixelcount * 25 / 100;
            bool ImageIsEmpty = true; 
            if ( nonzeropixelcount > minNonZeroPixelsCount)
            {
                ImageIsEmpty = false;
            }
            return ImageIsEmpty;
        }

        public static Bitmap PadImage(Bitmap originalImage)
        {
            int largestDimension = Math.Max(originalImage.Height, originalImage.Width);
            Size squareSize = new Size(largestDimension, largestDimension);
            Bitmap squareImage = new Bitmap(squareSize.Width, squareSize.Height, originalImage.PixelFormat);
            using (Graphics graphics = Graphics.FromImage(squareImage))
            {
                graphics.FillRectangle(Brushes.Black, 0, 0, squareSize.Width, squareSize.Height);
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                var offset_x = (squareSize.Width / 2) - (originalImage.Width / 2);
                var offset_y = (squareSize.Height / 2) - (originalImage.Height / 2);
                graphics.DrawImage(originalImage, offset_x, offset_y, originalImage.Width, originalImage.Height);
            }
            return squareImage;
        }

        static void BraTsFoldering()
        {

            string targetPath = @"Y:\PhD-research\DataSet\braindatabase\BraTS-Test\Brats17TestingData\";
            string sourcepath = @"Y:\PhD-research\DataSet\braindatabase\BraTS-Test\Brats17TestingData\";
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);
            string[] allfolders = Directory.GetDirectories(sourcepath);
            foreach (var fldr in allfolders)
            {
                string[] alldirectories = Directory.GetDirectories(fldr, "*_T1c.*");
                foreach (var dr in alldirectories)
                {
                    var allfiles = Directory.GetFiles(dr, "*.mha");
                    var sourcename = allfiles[0];
                    string destname = Path.Combine(targetPath, Path.GetFileName(sourcename));
                    System.IO.File.Copy(sourcename, destname);
                    Console.WriteLine(Path.GetFileName(sourcename));
                }
            }

        }

        static void BratsPrepration()
    {
        string sourcepath = @"Y:\PhD-research\DataSet\Cardiac\ACDC-dataset\training\CMRI\";
        string targetPath = @"Y:\PhD-research\DataSet\Cardiac\ACDC-dataset\training\MRI\";
        if (!Directory.Exists(targetPath))
            Directory.CreateDirectory(targetPath);
        string[] allfolders = Directory.GetDirectories(sourcepath);
         foreach (var fldr in allfolders)
         {
             var allfile = Directory.GetFiles(fldr, "*.nii.gz"); //string[] alldirectories = Directory.GetDirectories(fldr, "*_T2.*");
             foreach (var file in allfile)
             {
                 var sourcename = file;
                 string destname = Path.Combine(targetPath,Path.GetFileName(sourcename));
                 System.IO.File.Copy(sourcename, destname);
                 Console.WriteLine(Path.GetFileName(sourcename)); 
             }
         }

    }

        static System.Drawing.Bitmap printGTSlides(System.Drawing.Bitmap bmp, string Name, string savename)
        {
            bool Notempty = false; double Xmin = 240; double Xmax = 0; double Ymin = 240; double Ymax = 0; string updateline = "false";

            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                return bmp;
            }


            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb)
            {

                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new float[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[3 * npixels];

       

                for (int it = 0; it < npixels; it++)
                {

                    var inval = values[it];
                    double y = Math.Floor(Convert.ToDouble(it) / Convert.ToDouble(bmp.Width));
                    double x = it - (y * Convert.ToDouble(bmp.Width));

                    if (inval == 0) { inval = 0; }

                    if (inval == 1) { inval = 0; Notempty = true; }
                    if (inval == 2) { inval = 1; Notempty = true; }

                        updateline = "true";
                   

                    var val = Convert.ToInt32(inval);

                    if (val > 255)
                        val = Convert.ToByte(255);
                    if (val < 0)
                        val = Convert.ToByte(0);
                    var val8 = Convert.ToByte(val);
                    for (int itc = 0; itc < 3; itc++)
                    {
                        //* Copying val8 to RGB
                        outvalues[it * 3 + itc] = val8;
                    }

                }

                if (Notempty == true)
                {
                    var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                    Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 3 * npixels);
                    outbmp.UnlockBits(outbmpdata);
                    outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                    outbmp.Save(savename);
                }
                return bmp;
                
            }


            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            {

           
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new short[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[3 * npixels];
                
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    //double y = Math.Floor(Convert.ToDouble (it)/ Convert.ToDouble(bmp.Width));
                    //double x = it- (y* Convert.ToDouble(bmp.Width));

             

                    if (inval == 0) { inval = 0;  }

                    if (inval == 1) { inval = 0; Notempty = true; }
                    if (inval == 2) { inval = 1; Notempty = true; }
                    //if (inval == 3) { inval = 0; Notempty = true; }
                    //if (inval == 4) { inval = 0; Notempty = true; }

                    updateline = "true";

                    var val = Convert.ToInt16(inval);
                  
                    if (val > 255)
                        val = Convert.ToByte(255);
                    if (val < 0)
                        val = Convert.ToByte(0);
                    var val8 = Convert.ToByte(val);
                    for (int itc = 0; itc < 3; itc++)
                    {
                        //* Copying val8 to RGB
                        outvalues[it * 3 + itc] = val8;
                    }
                }
                if (Notempty == true)
                {
                    var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                    Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 3 * npixels);
                    outbmp.UnlockBits(outbmpdata);
                    outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                    outbmp.Save(savename);
                    //return outbmp;
                    //Xmin = 50; Xmax = 100; Ymin = 100; Ymax = 400;
                    //CropImage(outbmp, savename, Xmin, Xmax, Ymin, Ymax);
                }
                return bmp;
            }


            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new byte[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues =new byte[3 * npixels];


                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    double y = Math.Floor(Convert.ToDouble(it) / Convert.ToDouble(bmp.Width));
                    double x = it - (y * Convert.ToDouble(bmp.Width));



                    if (inval == 0) { inval = 0; }

                    if (inval == 1) { inval = 0; Notempty = true; }
                    if (inval == 2) { inval = 1; Notempty = true; }

                    updateline = "true";

              

                    for (int itc = 0; itc <3; itc++)
                    {
                        outvalues[it * 3 + itc] = inval;
                    }
                }

                if (Notempty == true)
                {
                    var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                    Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 3 * npixels);
                    outbmp.UnlockBits(outbmpdata);
                    outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                    outbmp.Save(savename);
                    //return outbmp;
                    //Xmin = 40; Xmax = 400; Ymin = 320; Ymax = 100;
                    //CropImage(outbmp, savename, Xmin, Xmax, Ymin, Ymax);
                }

                return bmp;

            }

            else
            {
                throw new Exception("Pixel format not supported!!!");
            }
            
        }

        static System.Drawing.Bitmap ConvertTo24bppRgb(System.Drawing.Bitmap bmp, int min_level, int max_level)
        {
         
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new byte[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[3 * npixels];
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    for (int itc = 0; itc < 3; itc++)
                    {
                        //* Copying val8 to RGB
                        outvalues[it * 3 + itc] = inval;
                    }
                }
                var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 3 * npixels);
                outbmp.UnlockBits(outbmpdata);
                outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                return outbmp;
            }


            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format16bppGrayScale)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new short[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[3 * npixels];
                var norfactor = (max_level - min_level) / 255;
                if (norfactor == 0)
                    norfactor = 1;
                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    var val = Convert.ToInt32(inval);
                    val = (val - min_level) / norfactor;
                    if (val > 255)
                        val = Convert.ToByte(255);
                    if (val < 0)
                        val = Convert.ToByte(0);
                    var val8 = Convert.ToByte(val);

                    for (int itc = 0; itc < 3; itc++)
                    {
                        //* Copying val8 to RGB
                        outvalues[it * 3 + itc] = val8;
                    }
                }
                var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 3 * npixels);
                outbmp.UnlockBits(outbmpdata);
                outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                return outbmp;
            }

            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                return bmp;
            }
           
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb)
            {


                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new float[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);



                var outvalues = new byte[3 * npixels];

                for (int it = 0; it < npixels; it++)
                {

                    var inval = values[it];

                    var val = Convert.ToSingle(inval);
                    //if(val != 0)
                    //    Console.WriteLine(val);

                    if (val > 255)
                        val = Convert.ToByte(255);
                    if (val < 0)
                        val = Convert.ToByte(0);
                    var val8 = Convert.ToByte(val);
                    for (int itc = 0; itc < 3; itc++)
                    {
                        //* Copying val8 to RGB
                        outvalues[it * 3 + itc] = val8;
                    }

                }

                
                    var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                    Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 3 * npixels);
                    outbmp.UnlockBits(outbmpdata);
                    outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                
                    return outbmp;
               
      
            }


            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format64bppArgb)
            {
                var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
                var npixels = bmp.Width * bmp.Height;
                var values = new double[npixels];
                Marshal.Copy(bmpdata.Scan0, values, 0, npixels);
                bmp.UnlockBits(bmpdata);
                var outvalues = new byte[3 * npixels];

                var norfactor = (max_level - min_level) / 255;
                if (norfactor == 0)
                    norfactor = 1;


                for (int it = 0; it < npixels; it++)
                {
                    var inval = values[it];
                    var val = Convert.ToDouble(inval);
                    val = (val - min_level) / norfactor;
                    if (val > 255)
                        val = Convert.ToByte(255);
                    if (val < 0)
                        val = Convert.ToByte(0);
                    var val8 = Convert.ToByte(val);

                    for (int itc = 0; itc < 3; itc++)
                    {
                        //* Copying val8 to RGB
                        outvalues[it * 3 + itc] = val8;
                    }
                }

                var outbmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var outbmpdata = outbmp.LockBits(new Rectangle(0, 0, outbmp.Width, outbmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, outbmp.PixelFormat);
                Marshal.Copy(outvalues, 0, outbmpdata.Scan0, 3 * npixels);
                outbmp.UnlockBits(outbmpdata);
                outbmp.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);
                return outbmp;

            }

            else
            {
                throw new Exception("Pixel format not supported!!!");
            }
        }



 

   
  

    }
}
