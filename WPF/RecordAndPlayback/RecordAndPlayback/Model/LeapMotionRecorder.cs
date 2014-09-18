using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;

namespace RecordAndPlayback.Model
{
    public class LeapMotionRecorder : LeapMotionController
    {
        FileStream stream;

        public LeapMotionRecorder()
        {
            stream = new FileStream( @"LeapPlayback.bin", FileMode.Create, FileAccess.Write );
        }

        protected override void Update()
        {
            base.Update();

            var frame = leap.Frame();
            var binary = frame.Serialize;
            var length = System.BitConverter.GetBytes(binary.Length);

            stream.Write( length, 0, length.Length );
            stream.Write( binary, 0, binary.Length );
            stream.Flush();
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
