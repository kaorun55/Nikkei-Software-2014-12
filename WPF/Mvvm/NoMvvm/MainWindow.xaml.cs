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
using Leap;

namespace NoMvvm
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        protected Controller leap = new Controller();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            leap.SetPolicyFlags( leap.PolicyFlags | Controller.PolicyFlag.POLICY_IMAGES );

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        void CompositionTarget_Rendering( object sender, EventArgs e )
        {
            var frame = leap.Frame();

            var images = frame.Images;
            if ( images.Count == 0 ) {
                return;
            }

            ImageLeft.Source = BitmapSource.Create( images[0].Width, images[0].Height, 96, 96, PixelFormats.Gray8, null, images[0].Data, images[0].Width );
            ImageRight.Source = BitmapSource.Create( images[1].Width, images[1].Height, 96, 96, PixelFormats.Gray8, null, images[1].Data, images[1].Width );
        }
    }
}
