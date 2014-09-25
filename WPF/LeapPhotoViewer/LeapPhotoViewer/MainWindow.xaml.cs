using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LeapPhotoViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        Leap.Controller leap = new Leap.Controller();

        bool isPinch = false;
        bool isGrab = false;
        Leap.Vector prevPosition;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded( object sender, RoutedEventArgs e )
        {
            leap.SetPolicyFlags( Leap.Controller.PolicyFlag.POLICY_IMAGES );

            for ( int i = 1; i <= 3; i++ ) {
                CanvasPhoto.Children.Add( CreateImage( string.Format( "{0}.jpg", i ), new Point( 400 * i, 200 ) ) );
            }

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private Image CreateImage( string fileName, Point point )
        {
            var image = new Image();
            image.Width = 300;
            image.Height = 200;
            image.Source = new BitmapImage( new Uri( @"pack://application:,,,/Image/" + fileName ) );
            image.RenderTransform = new ScaleTransform( 1, 1 );

            Canvas.SetLeft( image, point.X );
            Canvas.SetTop( image, point.Y );

            return image;
        }

        void CompositionTarget_Rendering( object sender, EventArgs e )
        {
            // 最新のフレームを取得する
            var frame = leap.Frame();

            // カメラ画像を取得する
            var images = frame.Images;
            if ( images.Count == 0 ) {
                return;
            }

            // 画像データをビットマップにする
            var left = images[0];
            ImageLeap.Source = BitmapSource.Create( left.Width, left.Height, 96, 96, PixelFormats.Gray8, null, left.Data, left.Width );

            CanvasFinger.Children.Clear();

            // 右手を探す
            var hand = frame.Hands.FirstOrDefault( h => h.IsRight );
            if ( hand ==null ) {
                isPinch = isGrab = false;
                return;
            }

            // 手と指の位置を取得する
            var handPosition = GetFingerPositionForRawImage( left, hand.StabilizedPalmPosition );
            var fingers = GetFingerPositionForRawImage( left, hand.Fingers );

            // 手と指の位置を描画する
            AddEllipse( left, handPosition, 30 );
            foreach ( var finger in fingers ) {
                AddEllipse( left, finger, 10 );
            }

            // グラブ
            if ( hand.GrabStrength >= 0.95 ) {
                ProcessGrab( handPosition );

                // 手の座標を保存する
                prevPosition = handPosition;
            }
            // ピンチ
            else if ( hand.PinchStrength >= 0.95 ) {
                // 親指を探す
                var finger = hand.Fingers.FirstOrDefault( f => f.Type() == Leap.Finger.FingerType.TYPE_THUMB );
                if ( finger == null ) {
                    isPinch = false;
                    return;
                }

                // 指の座標をカメラ画像に合わせる
                var fingerPosition = GetFingerPositionForRawImage( left, finger.StabilizedTipPosition );

                // ピンチ処理
                ProcessPinch( fingerPosition );

                // 指の座標を保存する
                prevPosition = fingerPosition;
            }
            else {
                isPinch = isGrab = false;
            }
        }

        /// <summary>
        /// FrameworkElementのあたり判定
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        FrameworkElement HitTest( Visual reference, Point point )
        {
            HitTestResult result = VisualTreeHelper.HitTest( CanvasPhoto, new Point( point.X, point.Y ) );
            if ( result == null ) {
                return null;
            }

            return result.VisualHit as FrameworkElement;
        }

        /// <summary>
        /// グラブの処理
        /// </summary>
        /// <param name="handPosition"></param>
        private void ProcessGrab( Leap.Vector handPosition )
        {
            // 手の座標が画像に接しているか調べる
            var element = HitTest( CanvasPhoto, new Point( handPosition.x, handPosition.y ) );
            if ( (element == null) ) {
                isGrab = false;
                return;
            }

            // Grab開始
            if ( !isGrab ) {
                isGrab = true;

                Trace.WriteLine( "Grab Start" );
                return;
            }

            Trace.WriteLine( "Grab Move : " + handPosition.z.ToString() );

            // 前フレームとの差分を取得
            var delta = handPosition - prevPosition;

            // エレメントを動かす
            MoveElement( element, delta );

            // ZIndexをZ位置によって動かす
            Canvas.SetZIndex( element, Canvas.GetZIndex( element ) + (int)(delta.z * 100) );

            // Z位置によって、画像の拡大率を変更する
            var scaleTransform = element.RenderTransform as ScaleTransform;
            if ( scaleTransform != null ) {
                var scale = delta.z / 30;
                if ( (scaleTransform.ScaleX + scale) >= 1.0 ) {
                    scaleTransform.ScaleX += scale;
                    scaleTransform.ScaleY += scale;
                }
            }

        }

        /// <summary>
        /// ピンチの処理
        /// </summary>
        /// <param name="fingerPosition"></param>
        private void ProcessPinch( Leap.Vector fingerPosition )
        {
            // 指が画像に接しているか調べる
            var element = HitTest( CanvasPhoto, new Point( fingerPosition.x, fingerPosition.y ) );
            if ( (element  == null) ) {
                isPinch = false;
                return;
            }

            // ピンチ開始
            if ( !isPinch ) {
                Trace.WriteLine( "Pinch Start" );

                isPinch  = true;
                return;
            }

            Trace.WriteLine( "Pinch Move" );

            // エレメントを動かす
            MoveElement( element, fingerPosition - prevPosition );
        }

        /// <summary>
        /// エレメントを動かす
        /// </summary>
        /// <param name="element"></param>
        /// <param name="delta"></param>
        private static void MoveElement( FrameworkElement element, Leap.Vector delta )
        {
            Canvas.SetLeft( element, Canvas.GetLeft( element ) + delta.x );
            Canvas.SetTop( element, Canvas.GetTop( element ) + delta.y );
        }

        /// <summary>
        /// 円を書く
        /// </summary>
        /// <param name="image"></param>
        /// <param name="position"></param>
        /// <param name="R"></param>
        void AddEllipse( Leap.Image image, Leap.Vector position, int R )
        {
            var ellipse = new Ellipse()
            {
                Width = R,
                Height = R,
                Fill = Brushes.Red,
            };

            Canvas.SetLeft( ellipse, position.x );
            Canvas.SetTop( ellipse, position.y );

            CanvasFinger.Children.Add( ellipse );
        }

        //x-axis offset in millimeters
        const float cameraOffset = 20;

        Leap.Vector GetFingerPositionForRawImage( Leap.Image image, Leap.Vector position )
        {
            // 3次元の指の位置を、2次元のカメラ画像に合わせる
            float hSlope = -(position.x + cameraOffset * (2 * image.Id - 1))/position.y;
            float vSlope = position.z/position.y;
            var pos = image.Warp( new Leap.Vector( hSlope, vSlope, 0 ) );

            // カメラ画像の拡大率を取得する
            var scaleX = ImageLeap.ActualWidth / image.Width;
            var scaleY = ImageLeap.ActualHeight / image.Height;

            // 座標を拡大する
            pos.x = (float)(ImageLeap.ActualWidth - (pos.x * scaleX));
            pos.y = (float)(pos.y * scaleY);
            pos.z = position.z;

            return pos;
        }

        private Leap.Vector[] GetFingerPositionForRawImage( Leap.Image image, Leap.FingerList fingers )
        {
            List<Leap.Vector> fingerPositions = new List<Leap.Vector>();

            foreach ( var finger in fingers ) {
                fingerPositions.Add( GetFingerPositionForRawImage( image, finger.TipPosition ) );
            }

            return fingerPositions.ToArray();
        }
    }
}
