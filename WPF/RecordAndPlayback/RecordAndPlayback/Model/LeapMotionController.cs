using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Leap;

namespace RecordAndPlayback.Model
{
    public class LeapMotionController : INotifyPropertyChanged, IDisposable
    {
        public enum DataSource
        {
            Controller, // コントローラーから取得(保存なし)
            Record,     // コントローラーから取得(保存あり)
            Playback,   // 保存データから取得
        }

        const string FileName = @"LeapPlayback.bin";

        Controller leap = new Controller();
        LeapMotionRecorder recorder = null;
        LeapMotionPlayback player = null;

        DataSource dataSource = DataSource.Controller;

        public LeapMotionController( DataSource source )
        {
            dataSource = source;
            if ( dataSource == DataSource.Record ) {
                recorder = new LeapMotionRecorder( FileName );
            }
            else if ( dataSource == DataSource.Playback ) {
                player = new LeapMotionPlayback( FileName );
            }

            leap.SetPolicyFlags( leap.PolicyFlags | Controller.PolicyFlag.POLICY_IMAGES );

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        /// <summary>
        /// 一定の周期(WPFの描画周期)で呼び出される
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CompositionTarget_Rendering( object sender, EventArgs e )
        {
            var frame = GetFrame();

            _Fingers.Clear();

            var ibox = frame.InteractionBox;
            foreach ( var finger in frame.Fingers ) {
                _Fingers.Add( ibox.NormalizePoint( finger.StabilizedTipPosition ) );
            }

            NotifyPropertyChanged( "Fingers" );
        }

        Frame GetFrame()
        {
            if (dataSource == DataSource.Controller) {
                return leap.Frame();
            }
            else if ( dataSource == DataSource.Record ) {
                var frame = leap.Frame();
                recorder.Record( frame );
                return frame;
            }
            else {
                return player.Frame();
            }
        }

        List<Vector> _Fingers = new List<Vector>();

        public Vector[] Fingers
        {
            get
            {
                return _Fingers.ToArray();
            }
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

        #region INotifyPropertyChanged
        public void Dispose()
        {
            if ( player != null ) {
                player.Dispose();
                player = null;
            }

            if ( recorder != null ) {
                recorder.Dispose();
                recorder = null;
            }
        }
        #endregion
    }
}
