using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Leap;

namespace RecordAndPlayback.Model
{
    public class LeapMotionPlayback : LeapMotionControllerBase
    {
        Controller leap = new Controller();
        FileStream stream;

        public LeapMotionPlayback()
        {
            leap.SetPolicyFlags( leap.PolicyFlags | Controller.PolicyFlag.POLICY_IMAGES );

            stream = new FileStream( @"LeapPlayback.bin", FileMode.Open, FileAccess.Read );

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        void CompositionTarget_Rendering( object sender, EventArgs e )
        {
            if ( stream.Position == stream.Length ) {
                stream.Seek( 0, SeekOrigin.Begin );
            }

            var length = new byte[4];
            stream.Read( length, 0, length.Length );

            var binary = new byte[BitConverter.ToUInt32( length, 0 )];
            stream.Read( binary, 0, binary.Length );

            var frame = new Frame();
            frame.Deserialize( binary );

            var images = frame.Images;
            if ( images.Count == 2 ) {
                ImageLeft = BitmapSource.Create( images[0].Width, images[0].Height, 96, 96, PixelFormats.Gray8, null, images[0].Data, images[0].Width );
                NotifyPropertyChanged( "ImageLeft" );

                ImageRight = BitmapSource.Create( images[1].Width, images[1].Height, 96, 96, PixelFormats.Gray8, null, images[1].Data, images[1].Width );
                NotifyPropertyChanged( "ImageRight" );
            }

            var fingerPosition = new List<Vector>();
            float cameraOffset = 20; //x-axis offset in millimeters
            foreach ( var finger in frame.Fingers ) {
                Leap.Vector tip = finger.TipPosition;
                float hSlope = -(tip.x + cameraOffset * (2 * images[1].Id - 1))/tip.y;
                float vSlope = tip.z/tip.y;

                fingerPosition.Add( images[1].Warp( new Leap.Vector( hSlope, vSlope, 0 ) ) );
            }

            FingerRight = fingerPosition.ToArray();
            NotifyPropertyChanged( "FingerRight" );
        }

        protected override void DisposeManagedObjects()
        {
            base.DisposeManagedObjects();

            if ( stream !=null ) {
                stream.Dispose();
                stream = null;
            }
        }
    }
}
