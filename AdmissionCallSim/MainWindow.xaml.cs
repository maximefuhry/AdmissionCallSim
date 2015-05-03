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

namespace AdmissionCallSim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserControl draggedImage;
        private Point mousePosition;
        private int numberOfPhone = 0;
        private int numberOfAntena = 1;
        public ObservableCollection<PhoneInfo> phoneInfoList = new ObservableCollection<PhoneInfo>();
        private List<Phone> phoneList = new List<Phone>();
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = phoneInfoList;
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
                
                Phone currentPhone = draggedImage as Phone;

                if (Canvas.GetLeft(draggedImage) + offset.X + draggedImage.Width < canvas.ActualWidth && Canvas.GetLeft(draggedImage) + offset.X > 0)
                {
                    Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                    phoneInfoList[currentPhone.phoneId - 1].x = Canvas.GetLeft(draggedImage) + offset.X;
                }
                if (Canvas.GetTop(draggedImage) + offset.Y + draggedImage.Height < canvas.ActualHeight && Canvas.GetTop(draggedImage) + offset.Y > 0)
                {
                    Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);
                    phoneInfoList[currentPhone.phoneId - 1].y = Canvas.GetTop(draggedImage) + offset.Y;
                }
                phoneDataGrid.Items.Refresh();
            }
        }

        private void addPhone(object sender, RoutedEventArgs e)
        {
            phoneList.Add(new Phone(++numberOfPhone));
            phoneInfoList.Add(new PhoneInfo { id = numberOfPhone, x = 0, y = 0 });

            canvas.Children.Insert(numberOfAntena, phoneList.Last());
            Canvas.SetLeft(phoneList.Last(), 0);
            Canvas.SetTop(phoneList.Last(), 0);
            Canvas.SetZIndex(phoneList.Last(), 1);
                       
        }

        private void removePhone(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(phoneList.Last());
            phoneList.Remove(phoneList.Last());
            phoneInfoList.Remove(phoneInfoList.Last());
            numberOfPhone--;
        }

    }

   
}
