using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SwiftTailer.Wpf.Infrastructure
{
    public class FileInfoCollection
    {
        public List<FileInfo> Files { get; set; }

        public FileInfoCollection()
        {
            Files = new List<FileInfo>();
        }

        public FileInfoCollection(IEnumerable<FileInfo> files)
        {
            Files = files.ToList();
        }
    }

    public class FileDropMonitor : IDependencyObjectReceiver, IDisposable
    {
        private readonly SerialDisposable _cleanUp = new SerialDisposable();
        private readonly ISubject<FileInfoCollection> _fileDropped = new Subject<FileInfoCollection>();

        public void Receive(DependencyObject value)
        {
            var control = (UIElement) value;
            control.AllowDrop = true; // force AllowDrop to true
            
            // when drag enters droppable zone
            DragAdorner adorner = null;
            var createAdorner = Observable.FromEventPattern<DragEventHandler, DragEventArgs>
                (handler => control.PreviewDragEnter += handler, handler => control.PreviewDragEnter -= handler)
                .Select(eventArgs => eventArgs.EventArgs) // grab all the arguments as observables
                .Subscribe(eventArgs => // define handlers
                {
                    var files = (string[]) eventArgs.Data.GetData(DataFormats.FileDrop);
                    var container = new FileDropContainer(files);
                    var contentPresenter = new ContentPresenter {Content = container};

                    adorner = new DragAdorner(control, contentPresenter);
                });

            // when drag leaves droppable zone
            var clearAdorner = Observable.FromEventPattern<DragEventHandler, DragEventArgs>(
                h => control.PreviewDragLeave += h, h => control.PreviewDragLeave -= h)
                .ToUnit() // observable void type
                .Merge( // combines two events together
                    Observable.FromEventPattern<DragEventHandler, DragEventArgs>(h => control.PreviewDrop += h,
                        h => control.PreviewDrop -= h)
                    .ToUnit())
                .Subscribe(e =>
                {
                    if (adorner == null) return;

                    adorner.Detatch();
                    adorner = null;
                });

            // keep adorner mouse position in sync with drag mouse position
            var window = Window.GetWindow(value);
            var updatePositionOfAdornment = Observable.FromEventPattern<DragEventHandler, DragEventArgs>
                (h => control.PreviewDragOver += h, h => control.PreviewDragOver -= h)
                .Select(ev => ev.EventArgs)
                .Where(_ => adorner != null) // hackish? returns nothing if adorner is null (previous step)
                .Subscribe(e => adorner.MousePosition = e.GetPosition(window));

            // when files are dropped
            var dropped = Observable.FromEventPattern<DragEventHandler, DragEventArgs>(
                h => control.Drop += h, h => control.Drop -= h)
                .Select(ev =>
                {
                    if (!ev.EventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                        return new FileInfoCollection();

                    var files = (string[]) ev.EventArgs.Data.GetData(DataFormats.FileDrop);
                    var fileInfos = files.Select(f => new FileInfo(f));
                    return new FileInfoCollection(fileInfos);
                })
                .SubscribeSafe(_fileDropped);
                    
                    //.SubscribeSafe(_fileDropped); // set handlers to whatever handlers where added to Dropped
            
            _cleanUp.Disposable = Disposable.Create(() =>
            {
                updatePositionOfAdornment.Dispose();
                clearAdorner.Dispose();
                createAdorner.Dispose();
                dropped.Dispose();
                _fileDropped.OnCompleted();
            });
        }

        /// <summary>
        /// Dropped Event
        /// </summary>
        public IObservable<FileInfoCollection> Dropped => _fileDropped;

        private class DragAdorner : Adorner
        {
            private readonly AdornerLayer _adornerLayer;
            private readonly UIElement _adornment;
            private Point _mousePositon;

            public DragAdorner(
                UIElement adornedElement,
                UIElement adornment,
                DragDropEffects effects = DragDropEffects.None)
                : base(adornedElement)
            {
                _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
                _adornerLayer.Add(this);
                _adornment = adornment;

                IsHitTestVisible = false;
                Effects = effects;
            }

            private DragDropEffects Effects { get; set; }

            public Point MousePosition
            {
                get { return _mousePositon; }
                set
                {
                    if (_mousePositon == value) return;
                    _mousePositon = value;
                    _adornerLayer.Update(AdornedElement);
                }
            }

            public void Detatch()
            {
                _adornerLayer.Remove(this);
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                _adornment.Arrange(new Rect(finalSize));
                return finalSize;
            }

            public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
            {
                var result = new GeneralTransformGroup();
                result.Children.Add(base.GetDesiredTransform(transform));
                result.Children.Add(new TranslateTransform(MousePosition.X - 4, MousePosition.Y - 4));

                return result;
            }

            protected override Visual GetVisualChild(int index)
            {
                return _adornment;
            }

            protected override Size MeasureOverride(Size constraint)
            {
                _adornment.Measure(constraint);
                return _adornment.DesiredSize;
            }

            protected override int VisualChildrenCount => 1;
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
