using System;
using System.IO;
using Leap;

namespace RecordAndPlayback.Model
{
    public class LeapMotionPlayback : IDisposable
    {
        FileStream stream;

        public LeapMotionPlayback( string fileName )
        {
            stream = new FileStream( fileName, FileMode.Open, FileAccess.Read );
        }

        public Frame Frame()
        {
            if ( stream.Position == stream.Length ) {
                stream.Seek( 0, SeekOrigin.Begin );
            }

            // データ長
            var length = new byte[4];
            stream.Read( length, 0, length.Length );

            // Frameデータ
            var binary = new byte[BitConverter.ToUInt32( length, 0 )];
            stream.Read( binary, 0, binary.Length );

            // デシリアライズ
            var frame = new Frame();
            frame.Deserialize( binary );

            return frame;
        }

        public void Dispose()
        {
            if ( stream !=null ) {
                stream.Dispose();
                stream = null;
            }
        }
    }
}
