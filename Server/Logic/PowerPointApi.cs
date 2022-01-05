using GalaSoft.MvvmLight.Messaging;
using PowerSocketServer.Models;
using System.IO;
using System.Windows;
using System.Windows.Media;
using NetOffice.PowerPointApi;
using static PowerSocketServer.Helpers.BitmapHelpers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using PowerSocketServer.Utils;

namespace PowerSocketServer.Logic
{
    class PowerPointServer
    {
        private NetOffice.PowerPointApi.Application _pptInstance;

        public PowerPointServer()
        {
            // Actions to handle, as defined in EventMessages:

            Messenger.Default.Register<powerpointApiSyncSlides>(this, (aa) =>
            {
                SyncState();
                ExportSlides();
            });

            Messenger.Default.Register<powerpointApiSync>(this, (aa) =>
            {
                SyncState();
            });


            Reconnect();

            SyncState();
            ExportSlides();
        }

        public void Reconnect()
        {
            NetOffice.PowerPointApi.Application pptInstance = NetOffice.PowerPointApi.Application.GetActiveInstance();

            if (pptInstance == null)
            {
                // TODO: add polling to wait for POWERPNT to launch

                // cannot use WMI here as that will require adminstrator privilleges which we probably don't got
            }

            Connect(pptInstance);
        }

        private void Connect(NetOffice.PowerPointApi.Application pptInstance)
        {
            this._pptInstance = pptInstance;
            // Dispose any existing event listener and re-register with this ppt instance
            EventListener eventListener = new EventListener();
            eventListener.SlideNavigatedEvent += (o, args) => SyncState();
            eventListener.ContentChangedEvent += (o, args) =>
            {
                ExportSlides();
                // TODO: Notify clients via a websocket message
            };
            
            eventListener.RegisterPowerPointInstance(this._pptInstance);
        }

        // Methods
        public void NextSlide()
        {
            //GlobalTryCatch(() =>
            //{

                SyncState();

                if (state.slide == null)
                {
                    Messenger.Default.Send(new ResponseMessage() { WsResponseMessage = "Failed to Next Slide: No current slide" });
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

            //});
        }

        public void PrevSlide()
        {
            //GlobalTryCatch(() =>
            //{
                SyncState();

                if (state.slide == null)
                {
                    Messenger.Default.Send(new ResponseMessage() { WsResponseMessage = "Failed to Prev Slide: No current slide" });
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
            //});
        }

        private readonly object syncSlidesLock = new object();
        public async void ExportSlides()
        {
            KeepAlive();

            await Task.Run(() =>
            {
                lock (syncSlidesLock)
                {

                    //GlobalTryCatch(() => {

                        Messenger.Default.Send(new SetIsExportingSlides() { IsExportingSlides = true });

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

                        Slides slides = null;
                        try
                        {
                            if (state.presentation != null)
                            {
                                slides = state.presentation.Slides;
                            }
                        }
                        catch (NetOffice.Exceptions.PropertyGetCOMException e)
                        {
                            Messenger.Default.Send(new SetIsExportingSlides() { IsExportingSlides = false });
                            Debug.Print("Slides sync failed. No active presentation/slides available");
                            return;
                        }

                        if (slides == null)
                        {
                            Messenger.Default.Send(new SetIsExportingSlides() { IsExportingSlides = false });
                            Debug.Print("Slides sync failed. No active presentation/slides available");
                            return;
                        }

                        foreach (Slide slide in slides)
                        {
                            // TODO lower resolution
                            float slideHeight = state.presentation.PageSetup.SlideHeight;
                            float slideWidth = state.presentation.PageSetup.SlideWidth;

                            slide.Export(System.IO.Path.Combine(temp, $"slide_{slide.SlideIndex}.png"), "PNG", slideWidth, slideHeight);
                            double progress = ((double)(slide.SlideIndex + 1) / (state.presentation.Slides.Count) * 100);
                            Messenger.Default.Send(new SetIsExportingSlides() { IsExportingSlides = true, Progress = (int)progress });
                        }

                        Messenger.Default.Send(new ResponseMessage() { WsResponseMessage = "Sync Success" });
                        Messenger.Default.Send(new SetIsExportingSlides() { IsExportingSlides = false });

                    //});

                }




            });

        }

        // Event listeners
        private class EventListener
        {
            public event EventHandler SlideNavigatedEvent;
            public event EventHandler ContentChangedEvent;
            public void RegisterPowerPointInstance(NetOffice.PowerPointApi.Application pptInstance)
            {
                if (pptInstance == null)
                {
                    // TODO loop
                    return;
                }

                // Content changed
                pptInstance.AfterPresentationOpenEvent += (pres) => OnRaiseContentChangedEvent(EventArgs.Empty);
                pptInstance.AfterPresentationOpenEvent += (pres) => OnRaiseContentChangedEvent(EventArgs.Empty);
                //pptInstance.SlideShowBeginEvent += (pres) => OnRaiseContentChangedEvent(EventArgs.Empty);

                // Slide changed
                pptInstance.SlideSelectionChangedEvent += range => OnRaiseSlideNavigatedEvent(EventArgs.Empty);
                pptInstance.SlideShowOnNextEvent += wn => OnRaiseSlideNavigatedEvent(EventArgs.Empty);
                pptInstance.SlideShowOnPreviousEvent += wn => OnRaiseSlideNavigatedEvent(EventArgs.Empty);
                pptInstance.SlideShowNextBuildEvent += wn => OnRaiseSlideNavigatedEvent(EventArgs.Empty);
                pptInstance.SlideShowNextClickEvent += (wn, effect) => OnRaiseSlideNavigatedEvent(EventArgs.Empty);
                pptInstance.SlideShowNextSlideEvent += wn => OnRaiseSlideNavigatedEvent(EventArgs.Empty);
            }

            // Wrap event invocations inside a protected virtual method
            // to allow derived classes to override the event invocation behavior
            protected virtual void OnRaiseSlideNavigatedEvent(EventArgs e)
            {
                // Make a temporary copy of the event to avoid possibility of
                // a race condition if the last subscriber unsubscribes
                // immediately after the null check and before the event is raised.
                EventHandler handler = SlideNavigatedEvent;

                // Event will be null if there are no subscribers
                if (handler != null)
                {
                    // Use the () operator to raise the event.
                    handler(this, e);
                }
            }

            protected virtual void OnRaiseContentChangedEvent(EventArgs e)
            {
                // Make a temporary copy of the event to avoid possibility of
                // a race condition if the last subscriber unsubscribes
                // immediately after the null check and before the event is raised.
                EventHandler handler = ContentChangedEvent;

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
                        if (pptInstance.Presentations.Count == 0 || pptInstance.ActivePresentation == null)

                        {

                        }
                        else if (pptInstance.SlideShowWindows.Count > 0)
                        {
                            // Get selected slide object in reading view
                            slide = pptInstance.SlideShowWindows[1].View.Slide;
                        }
                        else
                        {
                            // Get selected slide object in normal view
                            slide = slides[pptInstance.ActiveWindow.Selection.SlideRange.SlideNumber];
                        }

                        try
                        {



                        }
                        catch
                        {

                        }

                        // Get Slide count
                        info = new StateInfo() {
                            presentationName = pptInstance.ActivePresentation.Name,
                            totalSlidesCount = slides.Count,
                            currentSlideIndex = slide != null ? slide.SlideIndex : -1
                        };
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
            private StateInfo _info;
            public StateInfo info
            {
                get
                {
                    return _info;
                }
                set
                {
                    _info = value;
                    Messenger.Default.Send(new StateUpdateMessage() { state = value });
                }
            }
        }

        public class StateInfo
        {
            public string presentationName { get; set; }

            public int totalSlidesCount { get; set; }
            public int currentSlideIndex { get; set; }

            public override string ToString()
            {
                return $"{currentSlideIndex}/{totalSlidesCount}";
            }
        }

        public StateInfo GetStateInfo()
        {
            return state.info;
        }


        /**
         * Update state from PowerPoint instance
         */
        public void SyncState()
        {
            // retry Connect
            if (_pptInstance == null)
            {
                Reconnect();
            }

            state.Update(_pptInstance);
        }

        /**
         * Exception handler
         */


        [Serializable]
        public class MyException : Exception
        {
            public MyException() { }
            public MyException(string message) : base(message) { }
            public MyException(string message, Exception inner) : base(message, inner) { }
            protected MyException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
        public void GlobalTryCatch(Action action)
        {
            try
            {

                action.Invoke();
            }
            catch (MyException e)
            {
                // TODO exponential backoff
                //Reconnect();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void KeepAlive()
        {
            //Slides slides;
            //try
            //{
            //    if (state == null || state.presentation == null)
            //    {
            //        Reconnect();
            //    }
            //    else
            //    {
            //        slides = state.presentation.Slides;
            //    }
            //}
            //catch (NetOffice.Exceptions.PropertyGetCOMException e)
            //{
            //    Reconnect();
            //}
        }
    }
}
