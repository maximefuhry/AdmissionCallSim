using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Threading;
using AdmissionCallSim.SimCore;

namespace AdmissionCallSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserControl draggedImage;
        private Point mousePosition;
        //private int numberOfPhone = 0;
        private int numberOfAntena = 1;
        public ObservableCollection<PhoneInfo> phoneInfoList = new ObservableCollection<PhoneInfo>();
		//public ObservableCollection<Antenna> antennaInfoList = new ObservableCollection<Antenna>();
        private List<Mobile> phoneList = new List<Mobile>();
		private Cell cell = new Cell(550, 350);
		private AUSimulator instance;
		private Thread simulatorThread = null;
		private static MainWindow _this;
        
        public MainWindow()
        {
            InitializeComponent();
			_this = this;
			instance = AUSimulator.getInstance();
			instance.addCell(cell);
			//simulatorThread = new Thread(instance.run);
			DataContext = phoneInfoList;
        }

		public void stopSimulator()
		{
			instance.Running = false;
			simulatorThread.Join();
		}

		public void startSimulator()
		{
			instance.Running = true;
			simulatorThread = new Thread(instance.run);
			simulatorThread.Start();
		}


        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as UserControl;
       
            if (image != null && canvas.CaptureMouse())
            {
                mousePosition = e.GetPosition(canvas);
                draggedImage = image;
                Panel.SetZIndex(draggedImage, 2); // in case of multiple images
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedImage != null)
            {
				Mobile currentMobile = draggedImage as Mobile;
				currentMobile.Distance = AUSimulator.computeDistance(currentMobile, currentMobile.NearestCell);
                canvas.ReleaseMouseCapture();
                Panel.SetZIndex(draggedImage, 0);
                draggedImage = null;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {

            if (draggedImage != null)
            {
				
                var position = e.GetPosition(canvas);
                var offset = position - mousePosition;
                mousePosition = position;
                
                Mobile currentMobile = draggedImage as Mobile;

				Int32 index = phoneInfoList.IndexOf(phoneInfoList.Where(ph => (ph.id).Equals(currentMobile.ID)).FirstOrDefault());

                if (Canvas.GetLeft(draggedImage) + offset.X + draggedImage.Width < canvas.ActualWidth && Canvas.GetLeft(draggedImage) + offset.X > 0)
                {
                    Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                    phoneInfoList[index].x = Canvas.GetLeft(draggedImage) + offset.X;
					currentMobile.X = Convert.ToInt32(Canvas.GetLeft(draggedImage) + offset.X);
                }
                if (Canvas.GetTop(draggedImage) + offset.Y + draggedImage.Height < canvas.ActualHeight && Canvas.GetTop(draggedImage) + offset.Y > 0)
                {
                    Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);
                    phoneInfoList[index].y = Canvas.GetTop(draggedImage) + offset.Y;
					currentMobile.Y = Convert.ToInt32(Canvas.GetTop(draggedImage) + offset.Y);
                }
                phoneDataGrid.Items.Refresh();
            }
        }

        private void addPhone(object sender, RoutedEventArgs e)
        {
			Mobile m = new Mobile();
			//m.NearestCell = cell;
            phoneList.Add(m);
			instance.addMobile(m);
            phoneInfoList.Add(new PhoneInfo { id = m.ID, x = m.X, y = m.Y, call_length = 0 });

            canvas.Children.Insert(numberOfAntena, phoneList.Last());
            Canvas.SetLeft(phoneList.Last(), 0);
            Canvas.SetTop(phoneList.Last(), 0);
            Canvas.SetZIndex(phoneList.Last(), 1);
        } 

        private void removePhone(object sender, RoutedEventArgs e)
        {
			try
			{
				instance.removeMobile(phoneList.Last());
				canvas.Children.Remove(phoneList.Last());
				phoneList.Remove(phoneList.Last());
				phoneInfoList.Remove(phoneInfoList.Last());
			}
			catch(InvalidOperationException ex)
			{
				MessageBox.Show(ex.Message);
			}
        }

		private void Btn_StartSimu_Click(object sender, RoutedEventArgs e)
		{
			if (simulatorThread == null)
			{
				startSimulator();
			}
		}

		private void Btn_StopSimu_Click(object sender, RoutedEventArgs e)
		{
			stopSimulator();
			simulatorThread = null;
		}

		public static void updateDataGrid(){
			_this.phoneDataGrid.Items.Refresh();
		}

		public static ObservableCollection<PhoneInfo> getPhoneInfoList()
		{
			return _this.phoneInfoList;
		}
    }
}
