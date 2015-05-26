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
			X = x;
			Y = y;
			Gain = 1.25;
			Type = Call.Type.NONE;
			ID = Nbrmobiles++;
		}

		~Mobile()
		{
			Nbrmobiles--;
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
				CallResult call_result  = startCall(Call.Type.VOICE, 50);
				switch(call_result)
				{
					case CallResult.FAILURE: MessageBox.Show("Call rejected"); break;
					case CallResult.PENDING: MessageBox.Show("Call pending"); break;
					case CallResult.SUCCESS: MessageBox.Show("Call accepted"); break;
					default: break;
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

		private void test(object sender, EventArgs e)
		{
			AUSimulator simulator = AUSimulator.getInstance();
			simulator.addMobile(this);
			//simulator.provokeRandomCalls();  => Is now private
		}
	}
}
