using GalaSoft.MvvmLight.Messaging;
using PowerSocketServer.Models;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Media;
using NetOffice.PowerPointApi;
using static PowerSocketServer.Helpers.BitmapHelpers;
using System;

namespace PowerSocketServer.Logic
{
    class PowerPointApi
    {
        readonly NetOffice.PowerPointApi.Application _pptInstance;

        public PowerPointApi(NetOffice.PowerPointApi.Application pptInstance)
        {
            this._pptInstance = pptInstance;
            EventListener eventListener = new EventListener();
            eventListener.PowerPointSlideChangedEvent += (o, args) => SyncState(); 
            eventListener.PowerPointPresentationOpenEvent += (o, args) => ExportSlides(); 
            eventListener.RegisterPowerPointInstance(pptInstance);
        }

        // Methods
        public void NextSlide() {
            SyncState();

            if (state.slide == null)
            {
                Messenger.Default.Send(new EventMessages() { Message = "Failed to Next Slide: No current slide" });
                return;
            }

            int slideIndex = state.slide.SlideIndex + 1;
            if (slideIndex > state.info.totalSlidesCount)
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

            if (state.slide == null)
            {
                Messenger.Default.Send(new EventMessages() { Message = "Failed to Prev Slide: No current slide" });
                return;
            }

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

        public void ExportSlides()
        {
            var temp = PowerSocketServer.Helpers.TempDir.GetTempDirPath();

            System.IO.Directory.CreateDirectory(temp);
            System.IO.DirectoryInfo di = new DirectoryInfo(temp);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete(); 
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true); 
            }

            foreach (Slide slide in state.presentation.Slides)
            {
                // TODO 1080p
                slide.Export(System.IO.Path.Combine(temp, $"slide_{slide.SlideIndex}.png"), "PNG", 1024, 768);
            }
        }

        // Event listeners
        private class EventListener
        {
            public event EventHandler PowerPointSlideChangedEvent;  
            public event EventHandler PowerPointPresentationOpenEvent; 
            public void RegisterPowerPointInstance(NetOffice.PowerPointApi.Application pptInstance)
            {
                if (pptInstance == null)
                {
                    // TODO loop
                    return;
                }

                // Content changed
                pptInstance.AfterPresentationOpenEvent += (pres) => OnRaisePresentationOpenEvent(EventArgs.Empty);

                // Slide changed
                pptInstance.SlideSelectionChangedEvent += range => OnRaiseCustomEvent(EventArgs.Empty);
                pptInstance.SlideShowOnNextEvent += wn => OnRaiseCustomEvent(EventArgs.Empty);
                pptInstance.SlideShowOnPreviousEvent += wn => OnRaiseCustomEvent(EventArgs.Empty);
                pptInstance.SlideShowNextBuildEvent += wn => OnRaiseCustomEvent(EventArgs.Empty);
                pptInstance.SlideShowNextClickEvent += (wn, effect) => OnRaiseCustomEvent(EventArgs.Empty);
                pptInstance.SlideShowNextSlideEvent += wn => OnRaiseCustomEvent(EventArgs.Empty);
            }

            // Wrap event invocations inside a protected virtual method
            // to allow derived classes to override the event invocation behavior
            protected virtual void OnRaiseCustomEvent(EventArgs e)
            {
                // Make a temporary copy of the event to avoid possibility of
                // a race condition if the last subscriber unsubscribes
                // immediately after the null check and before the event is raised.
                EventHandler handler = PowerPointSlideChangedEvent;

                // Event will be null if there are no subscribers
                if (handler != null)
                {
                    // Use the () operator to raise the event.
                    handler(this, e);
                }
            }

            protected virtual void OnRaisePresentationOpenEvent(EventArgs e)
            {
                // Make a temporary copy of the event to avoid possibility of
                // a race condition if the last subscriber unsubscribes
                // immediately after the null check and before the event is raised.
                EventHandler handler = PowerPointPresentationOpenEvent;

                // Event will be null if there are no subscribers
                if (handler != null)
                {
                    // Use the () operator to raise the event.
                    handler(this, e);
                }
            }
        }

        // Test Methods
        /**
         * Warning: This will override the user's clipboard. Sorry :/
         */
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

                        // Get Slide count
                        info = new StateInfo() {totalSlidesCount = slides.Count, currentSlideIndex = slide != null ? slide.SlideIndex : -1};
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
            public StateInfo info;
        }

        public class StateInfo
        {
            public int totalSlidesCount { get; set; }
            public int currentSlideIndex { get; set; }
        }

        public StateInfo GetStateInfo()
        {
            return state.info;
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
