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

namespace AdmissionCallSim.SimCore
{
	/// <summary>
	/// Interaction logic for Mobile.xaml
	/// </summary>
	public partial class Mobile : UserControl
	{
		public Mobile()
			: this(0, 0)
		{

		}

		public Mobile(Int32 x, Int32 y)
		{
			InitializeComponent();
			_x = x;
			_y = y;
			_Pe = _default_Pe;
			_type = Call.Type.NONE;
			_id = _nbrmobiles++;
		}

		~Mobile()
		{
			_nbrmobiles--;
		}

		private void openPopUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
			/*   DockPanel dock = new DockPanel();
			   phonePopUp.Child = dock; 
			   dock.Children.Add(new Button { Content = "test" });
			   dock.Children.Add(new Button { Content = "test2" });*/
			Menu menu = new Menu();
			foreach (Call.Type t in Enum.GetValues(typeof(Call.Type)))
			{
				if (t != Call.Type.NONE)
				{
					MenuItem mi = new MenuItem
					{
						Header = t.ToString()
					};
					mi.Click += call;
					menu.Items.Add(mi);
				}
			}

			phonePopUp.Child = menu;
			phonePopUp.LostFocus += closePopUp;

			phonePopUp.IsOpen = true;
		}

		private void closePopUp(object sender, RoutedEventArgs e)
		{
			phonePopUp.IsOpen = false;
		}

		private void call(object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			MessageBox.Show(mi.Header.ToString());
			if(mi.Header.Equals(Call.Type.VOICE.ToString())){
				int call_result  = startCall(Call.Type.VOICE, 50);
				if(call_result < 0)
				{
					MessageBox.Show("Call rejected");
				}
				else if (call_result == 0)
				{
					MessageBox.Show("Call pending");
				}
				else if (call_result > 0) 
				{
					MessageBox.Show("Call accepted");
				}
			}
			else if (mi.Header.Equals(Call.Type.DATA_BAND_L.ToString()))
			{
				MessageBox.Show("MESSAGE");
			}
			else if (mi.Header.Equals(Call.Type.DATA_BAND_H.ToString()))
			{
				MessageBox.Show("STREAMING");
			}
			else if (mi.Header.Equals(Call.Type.NONE.ToString()))
			{
				MessageBox.Show("UNKNOWN");
			}
			else
			{
				MessageBox.Show("UNKNOWN");
			}
		}
	}
}
