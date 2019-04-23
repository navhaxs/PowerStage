using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerSocketServer.Views
{
    /// <summary>
    /// Interaction logic for QRCode.xaml
    /// </summary>
    public partial class QRCode : UserControl
    {
        public QRCode()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty URLProperty =
            DependencyProperty.RegisterAttached(
              "URL",
              typeof(String),
              typeof(QRCode),
              new PropertyMetadata("http://localhost:8977")
            );
        public static void SetURL(UIElement element, String value)
        {
            element.SetValue(URLProperty, value);
        }
        public static String GetURL(UIElement element)
        {
            return (String)element.GetValue(URLProperty);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(GetURL(this), QRCodeGenerator.ECCLevel.Q);
            XamlQRCode qrCode = new XamlQRCode(qrCodeData);
            DrawingImage qrCodeAsXaml = qrCode.GetGraphic(20);
            image.Source = qrCodeAsXaml;
        }
    }
}
