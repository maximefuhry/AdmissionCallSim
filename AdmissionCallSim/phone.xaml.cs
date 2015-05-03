using System;
using System.Collections.Generic;
using System.Text;
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
	/// Interaction logic for phone.xaml
	/// </summary>
	public partial class Phone : UserControl
	{
        public int phoneId;
        public string callMode;
        public enum callModes { voice, data, sms }

		public Phone(int id)
		{
			this.InitializeComponent();
            this.phoneId = id;
		}

		private void openPopUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
         /*   DockPanel dock = new DockPanel();
            phonePopUp.Child = dock; 
            dock.Children.Add(new Button { Content = "test" });
            dock.Children.Add(new Button { Content = "test2" });*/
            Menu menu = new Menu();
            menu.Items.Add(new MenuItem { Header = callModes.voice });
            menu.Items.Add(new MenuItem { Header = callModes.data });
            menu.Items.Add(new MenuItem { Header = callModes.sms });
            phonePopUp.Child = menu;
         
            phonePopUp.IsOpen = true;
		}
	}
}