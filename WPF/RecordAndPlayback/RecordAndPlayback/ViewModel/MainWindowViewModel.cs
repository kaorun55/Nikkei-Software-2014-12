using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RecordAndPlayback.Model;

namespace RecordAndPlayback.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        LeapMotionControllerBase leap = new LeapMotionPlayback();

        public MainWindowViewModel()
        {
            leap.PropertyChanged += leap_PropertyChanged;
        }

        void leap_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName == "ImageRight" ) {
                NotifyPropertyChanged( e.PropertyName );
            }
            else if ( e.PropertyName == "ImageLeft" ) {
                NotifyPropertyChanged( e.PropertyName );
            }
            else if ( e.PropertyName == "FingerRight" ) {
                foreach ( var vec in leap.FingerRight ) {
                    CanvasRight.Clear();
                    var ellipse = new Ellipse()
                    {
                        Width = 10,
                        Height = 10,
                        Fill = Brushes.Red,
                    };

                    Canvas.SetLeft( ellipse, vec.x );
                    Canvas.SetTop( ellipse, vec.y );

                    CanvasRight.Add( ellipse );
                }

                NotifyPropertyChanged( "CanvasRight" );
            }
        }

        private ObservableCollection<FrameworkElement> _CanvasRight = new ObservableCollection<FrameworkElement>();
        public ObservableCollection<FrameworkElement> CanvasRight
        {
            get
            {
                return _CanvasRight;
            }
        }

        public BitmapSource ImageRight
        {
            get
            {
                return leap.ImageRight;
            }
        }

        public BitmapSource ImageLeft
        {
            get
            {
                return leap.ImageLeft;
            }
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
