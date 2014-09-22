using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RecordAndPlayback.Model;

namespace RecordAndPlayback.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        LeapMotionController leap = new LeapMotionController(LeapMotionController.DataSource.Record);

        const int Width = 640;
        const int Height = 480;

        public MainWindowViewModel()
        {
            leap.PropertyChanged += leap_PropertyChanged;
        }

        void leap_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName == "Fingers" ) {
                CanvasFinger.Clear();
                foreach ( var vec in leap.Fingers ) {
                    var ellipse = new Ellipse()
                    {
                        Width = 10,
                        Height = 10,
                        Fill = Brushes.Red,
                    };

                    Canvas.SetLeft( ellipse, vec.x * Width );
                    Canvas.SetTop( ellipse, Height - (vec.y * Height) );

                    CanvasFinger.Add( ellipse );
                }

                NotifyPropertyChanged( "CanvasFinger" );
            }
        }

        private ObservableCollection<FrameworkElement> _CanvasFinger = new ObservableCollection<FrameworkElement>();
        public ObservableCollection<FrameworkElement> CanvasFinger
        {
            get
            {
                return _CanvasFinger;
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
