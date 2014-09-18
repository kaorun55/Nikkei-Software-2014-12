using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Leap;

namespace RecordAndPlayback.Model
{
    public class LeapMotionControllerBase : INotifyPropertyChanged, IDisposable
    {
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

        public Vector[] FingerRight
        {
            get;
            set;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged( [CallerMemberName] string propertyName = "" )
        {
            if ( PropertyChanged != null ) {
                PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
            }
        }
        #endregion

        #region IDisposable
        // http://msdn.microsoft.com/ja-jp/library/system.idisposable(v=vs.110).aspx
        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose( bool disposing )
        {
            if ( disposed )
                return;

            if ( disposing ) {
                // Free any other managed objects here.
                //
                DisposeManagedObjects();
            }

            // Free any unmanaged objects here.
            //
            DisposeUnmanagedObjects();

            disposed = true;
        }

        protected virtual void DisposeManagedObjects()
        {
        }

        protected virtual void DisposeUnmanagedObjects()
        {
        }
        #endregion
    }
}
