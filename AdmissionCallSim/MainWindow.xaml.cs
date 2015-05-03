﻿using System;
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
        private List<Phone> phoneList = new List<Phone>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as UserControl;
       
            if (image != null && canvas.CaptureMouse())
            {
                mousePosition = e.GetPosition(canvas);
                draggedImage = image;
                Panel.SetZIndex(draggedImage, 1); // in case of multiple images
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
                //Console.WriteLine(mousePosition.X);
                //Console.WriteLine(draggedImage.Width);

                if (Canvas.GetLeft(draggedImage) + offset.X + draggedImage.Width < canvas.ActualWidth && Canvas.GetLeft(draggedImage) + offset.X > 0)
                {
                    Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                }
                if (Canvas.GetTop(draggedImage) + offset.Y + draggedImage.Height < canvas.ActualHeight && Canvas.GetTop(draggedImage) + offset.Y > 0)
                {
                    Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);
                }
            }
        }

        private void addPhone(object sender, RoutedEventArgs e)
        {
            phoneList.Add(new Phone());
            numberOfPhone++;
            canvas.Children.Insert(numberOfAntena, phoneList.Last());
            Canvas.SetLeft(phoneList.Last(), 0);
            Canvas.SetTop(phoneList.Last(), 0);
            Canvas.SetZIndex(phoneList.Last(), 50); 
        }

    }
}
