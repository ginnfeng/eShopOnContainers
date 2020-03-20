using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using UTDll;
namespace UTool
{
	class UToolApp
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// 
		
		[STAThread]
		static void Main() 
		{
			Assembly sb=Assembly.GetEntryAssembly();
			//foreach(Assembly sb in Assembly.GetEntryAssembly();
				foreach(Module md in sb.GetModules())
				{
					string s;
					foreach(Type t in md.GetTypes())
					{
														
						s=t.ToString();
					}
				}
			UTest.startup(Language.CSharp);
			//Application.Run(new UTForm());
		}
		
	}
	/*
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
			this.ClientSize = new System.Drawing.Size(488, 346);
			this.Name = "Form1";
			this.Text = "Form1";

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new UTForm());
		}
	}
	*/
}
