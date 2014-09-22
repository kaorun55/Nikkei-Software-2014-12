using System;
using System.IO;
using Leap;

namespace RecordAndPlayback.Model
{
    public class LeapMotionRecorder : IDisposable
    {
        FileStream stream;

        public LeapMotionRecorder( string fileName )
        {
            stream = new FileStream( fileName, FileMode.Create, FileAccess.Write );
        }

        public void Record( Frame frame )
        {
            var binary = frame.Serialize;
            var length = System.BitConverter.GetBytes(binary.Length);

            // データ長&Frameデータ
            stream.Write( length, 0, length.Length );
            stream.Write( binary, 0, binary.Length );
            stream.Flush();
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
