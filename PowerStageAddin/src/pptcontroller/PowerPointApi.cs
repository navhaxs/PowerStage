using System;

using NetOffice;
using Office = NetOffice.OfficeApi;
using NetOffice.OfficeApi.Enums;
using PPt = NetOffice.PowerPointApi;
using NetOffice.PowerPointApi.Enums;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Media.Imaging;
using PowerStage.Models;
using PowerStage.Models;
namespace PowerStageAddin
{
    public class PowerPointApi
    {       
        // References: https://code.msdn.microsoft.com/office/How-to-Automate-control-8c8319b2
        // and the MSDN sample 'VBAutomationControlPPT'

        // adapted to NetOffice
        
        // Define PowerPoint Application object
        PPt.Application pptApplication = PowerStageAddin.Connect.thisPowerPoint;
        // Define Presentation object
        PPt.Presentation presentation;
        // Define Slide collection
        PPt.Slides slides;
        PPt.Slide slide;

        // Slide count
        int slidescount;
        // slide index
        int slideIndex;

        public PowerPointApi() : this(new PPt.Application())
        {
            // start a new instance of PowerPoint (and make it visible)
            // rather than passing in an existing instance of PowerPoint
            // (requiring this code to have been loaded as a "PowerPoint add-in")
            pptApplication.Visible = MsoTriState.msoTrue;
        }

        public PowerPointApi(PPt.Application thisPPtInstance)
        {
            if (thisPPtInstance != null)
            {
                pptApplication = thisPPtInstance;
                pptApplication.SlideSelectionChangedEvent += PptApplication_SlideSelectionChangedEvent; //SlideSelectionChanged
                pptApplication.SlideShowOnNextEvent += PptApplication_SlideShowOnNextEvent; ;
                pptApplication.SlideShowOnPreviousEvent += PptApplication_SlideShowOnPreviousEvent;
                pptApplication.SlideShowNextBuildEvent += PptApplication_SlideShowNextBuildEvent;
                pptApplication.SlideShowNextClickEvent += PptApplication_SlideShowNextClickEvent;
                pptApplication.SlideShowNextSlideEvent += PptApplication_SlideShowNextSlideEvent;
            }
        }

        private void PptApplication_SlideShowNextSlideEvent(PPt.SlideShowWindow Wn)
        {
            update();

        }

        private void PptApplication_SlideShowNextClickEvent(PPt.SlideShowWindow Wn, PPt.Effect nEffect)
        {
            update();
        }

        private void PptApplication_SlideShowNextBuildEvent(PPt.SlideShowWindow Wn)
        {
            update();
        }

        private void PptApplication_SlideShowOnPreviousEvent(PPt.SlideShowWindow Wn)
        {
            update();
        }

        private void PptApplication_SlideShowOnNextEvent(PPt.SlideShowWindow Wn)
        {
            update();
        }

        private void PptApplication_SlideSelectionChangedEvent(PPt.SlideRange SldRange)
        {
            update();
        }


        private void update()
        {
            getSlideThumnail();
            getSlideText();
        }
        private void getPPtData()
        {

            try
            {
                if (pptApplication != null)
                {
                    // Get Presentation Object
                    presentation = pptApplication.ActivePresentation;
                    // Get Slide collection object
                    slides = presentation.Slides;
                    // Get Slide count
                    slidescount = slides.Count;

                   
                    // Get current selected slide 
                    try
                    {
                        // Get selected slide object in normal view
                        slide = slides[pptApplication.ActiveWindow.Selection.SlideRange.SlideNumber];
                    }
                    catch
                    {
                        // Get selected slide object in reading view
                        slide = pptApplication.SlideShowWindows[1].View.Slide;

                    }
                }
            }
            catch
            {

                // if pptApplication.SlideShowWindows[1].View.Slide is invalid,
                // e.g. the 'Press any key to exit slideshow' screen
                // ignore.

            }
        }

        private void getPPtData2()
        {

            try
            {
                if (pptApplication != null)
                {
                    // Get Presentation Object
                    presentation = pptApplication.ActivePresentation;
                    // Get Slide collection object
                    slides = presentation.Slides;
                    // Get Slide count
                    slidescount = slides.Count;


                    // Get current selected slide 
                    try
                    {
                        // Get selected slide object in normal view
                        slide = slides[pptApplication.ActiveWindow.Selection.SlideRange.SlideNumber+1];
                    }
                    catch
                    {
                        // Get selected slide object in reading view
                        var slideNum = pptApplication.SlideShowWindows[1].View.Slide.SlideNumber;
                        slide = slides[slideNum + 1];
                    }
                }
            }
            catch
            {

                // if pptApplication.SlideShowWindows[1].View.Slide is invalid,
                // e.g. the 'Press any key to exit slideshow' screen
                // ignore.

            }
        }

        public void NextSlide() {
            getPPtData();
            slideIndex = slide.SlideIndex + 1;
            if (slideIndex > slidescount)
            {
                //MessageBox.Show("It is already last page")

                // rumble wiimote
            }
            else
            {
                try
                {
                    slide = slides[slideIndex];
                    slides[slideIndex].Select();

                }
                catch
                {

                    pptApplication.SlideShowWindows[1].View.Next();
                    slide = pptApplication.SlideShowWindows[1].View.Slide;
                }
            }
        }

        public void PrevSlide()
        {
            getPPtData();
            slideIndex = slide.SlideIndex - 1;
            if (slideIndex >= 1)
            {
                try
                {
                    slide = slides[slideIndex];
                    slides[slideIndex].Select();
                }
                catch
                {
                    pptApplication.SlideShowWindows[1].View.Previous();
                    slide = pptApplication.SlideShowWindows[1].View.Slide;
                }
            }
            else
            {
                //MessageBox.Show("It is already Fist Page");
            }
        }


        // Transform to First Page
        public void gotoFirst()
        {
            getPPtData();
            try
            {
                // Call Select method to select first slide in normal view
                slides[1].Select();
                slide = slides[1];
            }
            catch
            {
                // Transform to first page in reading view
                pptApplication.SlideShowWindows[1].View.First();
                slide = pptApplication.SlideShowWindows[1].View.Slide;
            }
        }

        // Transform to Last Page

        public void gotoLast()
        {
            getPPtData();
            try
            {
                slides[slidescount].Select();
                slide = slides[slidescount];
            }
            catch
            {
                pptApplication.SlideShowWindows[1].View.Last();
                slide = pptApplication.SlideShowWindows[1].View.Slide;
            }
        }

        public void blankWhite()
        {
            try
            {
                if (PowerStageAddin.Win32Api.isPowerPointSlideShowActive())
                {
                    //App.ui.DoSendKey("w"); // Dirty hack due to lack of API function:(
                }
            }
            catch
            {

            }
        }

        public void blankBlack()
        {
            try
            {
                if (PowerStageAddin.Win32Api.isPowerPointSlideShowActive())
                {
                    //App.ui.DoSendKey("b"); // Dirty hack due to lack of API function:(
                }
            }
            catch
            {

            }
        }

        public void slideshowStart()
        {
            getPPtData();
            try
            {

                if (pptApplication.SlideShowWindows.Count == 0)
                {
                    pptApplication.ActivePresentation.SlideShowSettings.Run();
                }

            }
            catch
            {
            }
        }

        public void slideshowStop()
        {
            getPPtData();
        }

        public void showTaskbar()
        {
            getPPtData();
        }



        internal void zoomIn()
        {
            try
            {
                if (PowerStageAddin.Win32Api.isPowerPointSlideShowActive())
                {
                    //App.ui.DoSendKey("{+}"); // Dirty hack due to lack of API function:(
                }
            }
            catch
            {

            }
        }


        internal void zoomOut()
        {
            try
            {
                if (PowerStageAddin.Win32Api.isPowerPointSlideShowActive())
                {
                    //App.ui.DoSendKey("-"); // Dirty hack due to lack of API function:(
                }
            }
            catch
            {

            }
        }


        public void getSlideText()
        {
            getPPtData();

            if (slide == null)
                return;

            string title = "", text = "";

            if (slide.Shapes.HasTitle == MsoTriState.msoTrue && 
                slide.Shapes.Title.TextFrame.HasText == MsoTriState.msoTrue)
                    title = slide.Shapes.Title.TextFrame.TextRange.Text;
        
            foreach (PPt.Shape shape in slide.Shapes)
            {
                //shape.PlaceholderFormat.Type == PpPlaceholderType.ppPlaceholderTitle ||
                //shape.PlaceholderFormat.Type == PpPlaceholderType.ppPlaceholderCenterTitle ||
                //shape.PlaceholderFormat.Type == PpPlaceholderType.ppPlaceholderVerticalTitle ||

                if (shape.Type == MsoShapeType.msoPlaceholder && (                    
                    shape.PlaceholderFormat.Type == PpPlaceholderType.ppPlaceholderFooter ||
                    shape.PlaceholderFormat.Type == PpPlaceholderType.ppPlaceholderSlideNumber))
                    continue;

                if (shape.HasTextFrame == MsoTriState.msoTrue && shape.TextFrame.HasText == MsoTriState.msoTrue)
                {


                    if (shape.Type == MsoShapeType.msoTextBox) { }
                        text += shape.TextFrame.TextRange.Text + "\n";

                    if (shape.Type == MsoShapeType.msoPlaceholder)
                        System.Diagnostics.Debug.Print("msoPlaceholder: " + shape.TextFrame.TextRange.Text);
                }
            }

            Messenger.Default.Send(new SlideTextUpdateMessage
            {
                SlideTitle = title,
                SlideText = text
            });
        }

        public void getSlideThumnail()
        {
            ImageSource imgSource;
            ImageSource imgSource2;

            getPPtData();

            if (slide == null)
                return;

            Messenger.Default.Send(new SlideProgressUpdateMessage {
                CurrentSlideNum = slide.SlideNumber,
                TotalSlideNum = presentation.Slides.Count
            });

            slide.Copy();
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Bitmap))
            {
                imgSource = BinaryStructConverter.ImageFromClipboardDib();

                if (slide.SlideNumber == presentation.Slides.Count)
                {
                    Messenger.Default.Send(new ThumbnailUpdateMessage
                    {
                        CurrentSlideImage = imgSource,
                        NextSlideImageIsEmpty = true,
                        NextSlideImage = null
                    }
                    );
                } else {
                    getPPtData2();
                    slide.Copy();
                    if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Bitmap))
                    {
                        imgSource2 = BinaryStructConverter.ImageFromClipboardDib();
                        Messenger.Default.Send(new ThumbnailUpdateMessage
                        {
                            CurrentSlideImage = imgSource,
                            NextSlideImage = imgSource2
                        }
                        );
                    }

                }




            }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct BITMAPFILEHEADER
        {
            public static readonly short BM = 0x4d42; // BM

            public short bfType;
            public int bfSize;
            public short bfReserved1;
            public short bfReserved2;
            public int bfOffBits;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        public static class BinaryStructConverter
        {
            public static ImageSource ImageFromClipboardDib()
            {
                try
                {
                    MemoryStream ms = Clipboard.GetData("DeviceIndependentBitmap") as MemoryStream;
                    if (ms != null)
                    {
                        byte[] dibBuffer = new byte[ms.Length];
                        ms.Read(dibBuffer, 0, dibBuffer.Length);

                        BITMAPINFOHEADER infoHeader =
                            BinaryStructConverter.FromByteArray<BITMAPINFOHEADER>(dibBuffer);

                        int fileHeaderSize = Marshal.SizeOf(typeof(BITMAPFILEHEADER));
                        int infoHeaderSize = infoHeader.biSize;
                        int fileSize = fileHeaderSize + infoHeader.biSize + infoHeader.biSizeImage;

                        BITMAPFILEHEADER fileHeader = new BITMAPFILEHEADER();
                        fileHeader.bfType = BITMAPFILEHEADER.BM;
                        fileHeader.bfSize = fileSize;
                        fileHeader.bfReserved1 = 0;
                        fileHeader.bfReserved2 = 0;
                        fileHeader.bfOffBits = fileHeaderSize + infoHeaderSize + infoHeader.biClrUsed * 4;

                        byte[] fileHeaderBytes =
                            BinaryStructConverter.ToByteArray<BITMAPFILEHEADER>(fileHeader);

                        MemoryStream msBitmap = new MemoryStream();
                        msBitmap.Write(fileHeaderBytes, 0, fileHeaderSize);
                        msBitmap.Write(dibBuffer, 0, dibBuffer.Length);
                        msBitmap.Seek(0, SeekOrigin.Begin);

                        return BitmapFrame.Create(msBitmap);
                    }
                }
                catch (Exception)
                {
                    // don't throw;
                    // COM APIs are dumb..
                }
                
                return null;
            }

            public static T FromByteArray<T>(byte[] bytes) where T : struct
            {
                IntPtr ptr = IntPtr.Zero;
                try
                {
                    int size = Marshal.SizeOf(typeof(T));
                    ptr = Marshal.AllocHGlobal(size);
                    Marshal.Copy(bytes, 0, ptr, size);
                    object obj = Marshal.PtrToStructure(ptr, typeof(T));
                    return (T)obj;
                }
                finally
                {
                    if (ptr != IntPtr.Zero)
                        Marshal.FreeHGlobal(ptr);
                }
            }

            public static byte[] ToByteArray<T>(T obj) where T : struct
            {
                IntPtr ptr = IntPtr.Zero;
                try
                {
                    int size = Marshal.SizeOf(typeof(T));
                    ptr = Marshal.AllocHGlobal(size);
                    Marshal.StructureToPtr(obj, ptr, true);
                    byte[] bytes = new byte[size];
                    Marshal.Copy(ptr, bytes, 0, size);
                    return bytes;
                }
                finally
                {
                    if (ptr != IntPtr.Zero)
                        Marshal.FreeHGlobal(ptr);
                }
            }
        }

    }
}
