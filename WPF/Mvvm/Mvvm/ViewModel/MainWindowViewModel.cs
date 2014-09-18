using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Mvvm.Model;

namespace Mvvm.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        LeapMotionController leap = new LeapMotionController();

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
