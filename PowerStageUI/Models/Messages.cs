using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PowerStage.Models
{

    /* Stage display */
    public class ThumbnailUpdateMessage
    {
        public ImageSource CurrentSlideImage { get; set; }

        public bool NextSlideImageIsEmpty { get; set; } = false;
        public ImageSource NextSlideImage { get; set; }
    }

    public struct SlideProgressUpdateMessage
    {
        public int CurrentSlideNum { get; set; }
        public int TotalSlideNum { get; set; }
    }

    public struct StageMsgMessage
    {
        public string Msg { get; set; }
        public bool Visibility { get; set; }
    }

    public struct SlideTextUpdateMessage
    {
        public string SlideText { get; set; }
        public string SlideTitle { get; set; }
    }

    /* Overlay ("Fade to Logo"/"Freeze") */
    public struct OverlayMessage
    {
        public OverlayMode mode { get; set; }

        public enum OverlayMode
        {
            FadeToBlack = 0,
            FadeToLogo = 1,
            CutToFreeze = 2,
            FadeToClear = 3,
            CutToClear = 4
        }

    }

}
