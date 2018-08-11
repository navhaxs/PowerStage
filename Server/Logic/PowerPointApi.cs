using System.Data;
using System.Windows;
using System.Windows.Media;
using static PowerSocketServer.Helpers.BitmapHelpers;

namespace PowerSocketServer.Logic
{
    class PowerPointApi
    {
        readonly NetOffice.PowerPointApi.Application _pptInstance;

        public PowerPointApi(NetOffice.PowerPointApi.Application pptInstance)
        {
            this._pptInstance = pptInstance;
        }

        public void NextSlide() {
            SyncState();

            int slideIndex = state.slide.SlideIndex + 1;
            if (slideIndex > state.slidescount)
            {
                //MessageBox.Show("It is already last page")

                // rumble wiimote
            }
            else
            {
                try
                {
                    state.slides[slideIndex].Select();
                }
                catch
                {

                    _pptInstance.SlideShowWindows[1].View.Next();
                }
            }

            SyncState();
        }

        public void PrevSlide()
        {
            SyncState();

            int slideIndex = state.slide.SlideIndex - 1;
            if (slideIndex >= 1)
            {
                try
                {
                    state.slides[slideIndex].Select();
                }
                catch
                {
                    _pptInstance.SlideShowWindows[1].View.Previous();
                    //slide = pptApplication.SlideShowWindows[1].View.Slide;
                }
            }
            else
            {
                //MessageBox.Show("It is already Fist Page");
            }

            SyncState();
        }

        public void getSlideThumnail()
        {
            ImageSource imgSource;
            ImageSource imgSource2;

            
            SyncState();
            if (state.slide == null)
                return;

            //CurrentSlideNum = slide.SlideNumber,
            //TotalSlideNum = presentation.Slides.Count
            
            state.slide.Copy();
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Bitmap))
            {
                imgSource = BinaryStructConverter.ImageFromClipboardDib();

                //if (slide.SlideNumber == presentation.Slides.Count)
                //{
                //    Messenger.Default.Send(new ThumbnailUpdateMessage
                //        {
                //            CurrentSlideImage = imgSource,
                //            NextSlideImageIsEmpty = true,
                //            NextSlideImage = null
                //        }
                //    );
                //} else {
                //    getPPtData2();
                //    slide.Copy();
                //    if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Bitmap))
                //    {
                //        imgSource2 = BinaryStructConverter.ImageFromClipboardDib();
                //        Messenger.Default.Send(new ThumbnailUpdateMessage
                //            {
                //                CurrentSlideImage = imgSource,
                //                NextSlideImage = imgSource2
                //            }
                //        );
                //    }
                //}
            }
        }


        /**
         * State
         */

        private State state = new State();
        private class State
        {
            public void Update(NetOffice.PowerPointApi.Application pptInstance)
            {
                try
                {
                    if (pptInstance != null)
                    {
                        
                        // Get Presentation Object
                        presentation = pptInstance.ActivePresentation;
                        // Get Slide collection object
                        slides = presentation.Slides;
                        // Get Slide count
                        slidescount = slides.Count;
                
                        // Get current selected slide 
                        try
                        {
                            // Get selected slide object in normal view
                            slide = slides[pptInstance.ActiveWindow.Selection.SlideRange.SlideNumber];
                        }
                        catch
                        {
                            // Get selected slide object in reading view
                            slide = pptInstance.SlideShowWindows[1].View.Slide;
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
            public NetOffice.PowerPointApi.Presentation presentation;
            public NetOffice.PowerPointApi.Slides slides;
            public NetOffice.PowerPointApi.Slide slide;
            public int slidescount;
        }

        /**
         * Update state from PowerPoint instance
         */
        private void SyncState()
        {
            state.Update(_pptInstance);
        }
    }
}
