using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Leap;

namespace Mvvm.Model
{
    public class LeapMotionController : INotifyPropertyChanged
    {
        Controller leap = new Controller();

        public LeapMotionController()
        {
            leap.SetPolicyFlags( Controller.PolicyFlag.POLICY_IMAGES );

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        void CompositionTarget_Rendering( object sender, EventArgs e )
        {
            var frame = leap.Frame();

            var images = frame.Images;
            if ( images.Count == 0 ) {
                return;
            }

            ImageLeft = BitmapSource.Create( images[0].Width, images[0].Height, 96, 96, PixelFormats.Gray8, null, images[0].Data, images[0].Width );
            NotifyPropertyChanged( "ImageLeft" );

            ImageRight = BitmapSource.Create( images[1].Width, images[1].Height, 96, 96, PixelFormats.Gray8, null, images[1].Data, images[1].Width );
            NotifyPropertyChanged( "ImageRight" );
        }

        public BitmapSource ImageRight
        {
            get;
            set;
        }

        public BitmapSource ImageLeft
        {
            get;
            set;
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged( [CallerMemberName] string propertyName = "" )
        {
            if ( PropertyChanged != null ) {
                PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
            }
        }
        #endregion
    }
}
