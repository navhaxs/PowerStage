using System;
using System.Collections.Generic;
using System.Drawing;

namespace PowerStage
{
	/// <summary>
	/// This class shall keep all the functionality for capturing 
	/// the desktop.
	/// </summary>
	public class CaptureScreen
	{
        #region presenter view get hwnd
        private static List<IntPtr> presenterViewHwnd = new List<IntPtr>();
        public static List<IntPtr> GetPresenterViewHwnd()
        {
            presenterViewHwnd = new List<IntPtr>();
            PlatformInvokeUSER32.EnumWindows(EnumWindowsCallback2, (IntPtr)1);
            return presenterViewHwnd; // XXX first one only
        }
        #endregion
            
        #region powerpoint slide screenshot

        public static Bitmap GetScreenshot()
        {
            IntPtr hnd;
            var s = GetWindowTitles(true);
            if (s.Count > 0)
            {
                hnd = s[0];
            }
            else
            {
                hnd = IntPtr.Zero;
            }
            return PowerStage.CaptureScreen.GetWindowImage(hnd);
        }

        #endregion

        #region helper functions

        private static List<IntPtr> windowIntPtr = new List<IntPtr>();
        public static List<IntPtr> GetWindowTitles(bool includeChildren)
        {
            windowIntPtr = new List<IntPtr>();
            PlatformInvokeUSER32.EnumWindows(EnumWindowsCallback, includeChildren ? (IntPtr)1 : IntPtr.Zero);
            return windowIntPtr;
        }

        private static bool EnumWindowsCallback(IntPtr testWindowHandle, IntPtr includeChildren)
        {
            string title = PlatformInvokeUSER32.GetWindowTitle(testWindowHandle);

            if (TitleMatchesSlideShowWnd(title))
            {
                windowIntPtr.Add(testWindowHandle);
            }
            if (includeChildren.Equals(IntPtr.Zero) == false)
            {
                PlatformInvokeUSER32.EnumChildWindows(testWindowHandle, EnumWindowsCallback, IntPtr.Zero);
            }
            return true;
        }

        private static bool EnumWindowsCallback2(IntPtr testWindowHandle, IntPtr includeChildren)
        {
            string title = PlatformInvokeUSER32.GetWindowTitle(testWindowHandle);

            if (TitleMatchesSlideShowWnd(title))
            {
                windowIntPtr.Add(testWindowHandle);
            }
            if (includeChildren.Equals(IntPtr.Zero) == false)
            {
                PlatformInvokeUSER32.EnumChildWindows(testWindowHandle, EnumWindowsCallback, IntPtr.Zero);
            }
            return true;
        }

        private static bool TitleMatchesSlideShowWnd(string title)
        {
            bool match = title.StartsWith("PowerPoint Slide Show  -  ");
            return match;
        }

        private static bool TitleMatchesPresenterView(string title)
        {
            bool match = title.EndsWith(" - PowerPoint Presenter View");
            return match;
        }

        public static Bitmap GetWindowImage(IntPtr hwnd)
        {

            RECT rc;
            PlatformInvokeUSER32.GetWindowRect(hwnd, out rc);

            //In size variable we shall keep the size of the screen.
            SIZE size;
			
			//Variable to keep the handle to bitmap.
			IntPtr hBitmap;

            //Here we get the handle to the desktop device context.
            IntPtr hDC = PlatformInvokeUSER32.GetDC(PlatformInvokeUSER32.GetDesktopWindow());

            //Here we make a compatible device context in memory for screen device context.
            IntPtr hMemDC = PlatformInvokeGDI32.CreateCompatibleDC(hDC);

            //We pass SM_CXSCREEN constant to GetSystemMetrics to get the X coordinates of screen.
            size.cx = 600; // PlatformInvokeUSER32.GetSystemMetrics(PlatformInvokeUSER32.SM_CXSCREEN);

			//We pass SM_CYSCREEN constant to GetSystemMetrics to get the Y coordinates of screen.
			size.cy = 600;// PlatformInvokeUSER32.GetSystemMetrics(PlatformInvokeUSER32.SM_CYSCREEN);
			
			//We create a compatible bitmap of screen size using screen device context.
			hBitmap = PlatformInvokeGDI32.CreateCompatibleBitmap(hDC, rc.Width, rc.Height);

			//As hBitmap is IntPtr we can not check it against null. For this purspose IntPtr.Zero is used.
			if (hBitmap!=IntPtr.Zero)
			{
				//Here we select the compatible bitmap in memeory device context and keeps the refrence to Old bitmap.
				IntPtr hOld = (IntPtr) PlatformInvokeGDI32.SelectObject(hMemDC, hBitmap);
				//We copy the Bitmap to the memory device context.
				PlatformInvokeGDI32.BitBlt(hMemDC, 0, 0,rc.Width, rc.Height, hDC, rc.Left, rc.Top, PlatformInvokeGDI32.SRCCOPY);
				//We select the old bitmap back to the memory device context.
				PlatformInvokeGDI32.SelectObject(hMemDC, hOld);
				//We delete the memory device context.
				PlatformInvokeGDI32.DeleteDC(hMemDC);
				//We release the screen device context.
				PlatformInvokeUSER32.ReleaseDC(PlatformInvokeUSER32.GetDesktopWindow(), hDC);
				//Image is created by Image bitmap handle and stored in local variable.
				Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap); 
				//Release the memory for compatible bitmap.
				PlatformInvokeGDI32.DeleteObject(hBitmap);
				//This statement runs the garbage collector manually.
				GC.Collect();
				//Return the bitmap 
				return bmp;
			}
		
			//If hBitmap is null retunrn null.
			return null;
		}

        public static Bitmap GetDesktopImage()
        {
            //In size variable we shall keep the size of the screen.
            SIZE size;

            //Variable to keep the handle to bitmap.
            IntPtr hBitmap;

            //Here we get the handle to the desktop device context.
            IntPtr hDC = PlatformInvokeUSER32.GetDC(PlatformInvokeUSER32.GetDesktopWindow());

            //Here we make a compatible device context in memory for screen device context.
            IntPtr hMemDC = PlatformInvokeGDI32.CreateCompatibleDC(hDC);

            //We pass SM_CXSCREEN constant to GetSystemMetrics to get the X coordinates of screen.
            size.cx = PlatformInvokeUSER32.GetSystemMetrics(PlatformInvokeUSER32.SM_CXSCREEN);

            //We pass SM_CYSCREEN constant to GetSystemMetrics to get the Y coordinates of screen.
            size.cy = PlatformInvokeUSER32.GetSystemMetrics(PlatformInvokeUSER32.SM_CYSCREEN);

            //We create a compatible bitmap of screen size using screen device context.
            hBitmap = PlatformInvokeGDI32.CreateCompatibleBitmap(hDC, size.cx, size.cy);

            //As hBitmap is IntPtr we can not check it against null. For this purspose IntPtr.Zero is used.
            if (hBitmap != IntPtr.Zero)
            {
                //Here we select the compatible bitmap in memeory device context and keeps the refrence to Old bitmap.
                IntPtr hOld = (IntPtr)PlatformInvokeGDI32.SelectObject(hMemDC, hBitmap);
                //We copy the Bitmap to the memory device context.
                PlatformInvokeGDI32.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, 0, 0, PlatformInvokeGDI32.SRCCOPY);
                //We select the old bitmap back to the memory device context.
                PlatformInvokeGDI32.SelectObject(hMemDC, hOld);
                //We delete the memory device context.
                PlatformInvokeGDI32.DeleteDC(hMemDC);
                //We release the screen device context.
                PlatformInvokeUSER32.ReleaseDC(PlatformInvokeUSER32.GetDesktopWindow(), hDC);
                //Image is created by Image bitmap handle and stored in local variable.
                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
                //Release the memory for compatible bitmap.
                PlatformInvokeGDI32.DeleteObject(hBitmap);
                //This statement runs the garbage collector manually.
                GC.Collect();
                //Return the bitmap 
                return bmp;
            }

            //If hBitmap is null retunrn null.
            return null;
        }

        #endregion
    }
}
