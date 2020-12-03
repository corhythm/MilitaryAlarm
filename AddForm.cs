namespace Alarm
{
	public delegate void FormSendDataHandler(string sendString1, string sendString2, string sendString3);
	public delegate void EditAlarmDelegate(string content, string hour, string minute);
	
	public partial class AddForm : System.Windows.Forms.Form
	{	
		private AlarmForm AlarmForm;
		private System.ComponentModel.IContainer components = null;
		private bool EditBoolean;
		
		public event FormSendDataHandler FormSendEvent;	
		public event EditAlarmDelegate EditAlarmEvent;

		public AddForm()
		{
			InitializeComponent();
			this.EditBoolean = false;
		}

		public AddForm(AlarmForm form)
		{
			InitializeComponent();
			this.AlarmForm = form;
			this.EditBoolean = false;	
		}

		public AddForm(AlarmForm form, string content, string hour, string minute)
		{
			InitializeComponent();
			this.AlarmForm = form;
	
			this.Title.Text = content;
			this.Hours.Text = hour;
			this.Minutes.Text = minute; 
		
			this.BAddButton.Text = "Edit";
			this.EditBoolean = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void AMorPM_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			
		}
		
		private void Hours_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			
		}
		
		private void Minutes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		private void BAddButton_Click(object sender, System.EventArgs e)
		{
			if(this.Hours.Text == "�ð�" || this.Minutes.Text == "��")
			{
				System.Windows.Forms.MessageBox.Show("�ð� Ȥ�� ���� �������ֽʽÿ�.", 
					"Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error); 
				return;
			}

			for(System.Int32 i = 0; i < this.Title.Text.Length; i++)
			{
				if(this.Title.Text[i] == ' ')
				{					
					System.Windows.Forms.MessageBox.Show("���� ������ ������ �� �˴ϴ�.(������� ó���ϱ� �������� ><)", 
						"Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
					this.Title.Text = "";
					this.Title.SelectAll();
					this.Title.Focus();	
					return;
				}
			}
			
			if(this.EditBoolean) // Edit�� ��
			{
				this.EditAlarmEvent(this.Title.Text, this.Hours.Text, this.Minutes.Text);
			}
			else
			{
				this.FormSendEvent(this.Title.Text, this.Hours.Text, this.Minutes.Text);
			}
			this.Close();
		}

		private void Timer_Tick(object sender, System.EventArgs e)
		{
			
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Hours = new System.Windows.Forms.ComboBox();
			this.Minutes = new System.Windows.Forms.ComboBox();
			this.BAddButton = new System.Windows.Forms.Button();
			this.Panel = new System.Windows.Forms.Panel();
			this.Title = new System.Windows.Forms.TextBox();
			this.Timer = new System.Windows.Forms.Timer();
			this.Panel.SuspendLayout();
			this.SuspendLayout();
			//
			// Title
			//
			this.Title.Text = "������ �Է��ϼ���.";
			this.Title.Size = new System.Drawing.Size(250, 30);
			this.Title.Location = new System.Drawing.Point(15, 15 );
			this.Title.TabIndex = 1;
			this.Title.Enter += (sender, e) => {
				if(this.Title.Text == "������ �Է��ϼ���.")
				{
					this.Title.Text = "";
				}
			};
			this.Title.Leave += (sender, e) => {
				if(this.Title.Text == "")
				{
					this.Title.Text = "������ �Է��ϼ���.";
				}
			};
			//	
			// Hours
			//
			this.Hours.AllowDrop = true;
			this.Hours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.Hours.BackColor = System.Drawing.SystemColors.ScrollBar;
			this.Hours.DisplayMember = "Int32";
			this.Hours.FormattingEnabled = true;
			this.Hours.Items.AddRange(new object[] { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"});
			this.Hours.Location = new System.Drawing.Point(15, 60);
			this.Hours.Name = "Hours";
			this.Hours.Size = new System.Drawing.Size(100, 30);
			this.Hours.TabIndex = 2;
			this.Hours.Text = "�ð�";
			this.Hours.Font = new System.Drawing.Font("AR DESTINE", 10);
			this.Hours.ValueMember = "Int32";
			this.Hours.SelectedIndexChanged += new System.EventHandler(this.Hours_SelectedIndexChanged);
			//
			// Minutes
			//
			this.Minutes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.Minutes.BackColor = System.Drawing.SystemColors.ScrollBar;
			this.Minutes.FormattingEnabled = true;
			this.Minutes.Items.AddRange(new object[] { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
			"21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
			"50", "51", "52", "53", "54", "55", "56", "57", "58", "59"});
			this.Minutes.Location = new System.Drawing.Point(150, 60);
			this.Minutes.Name = "Minutes";
			this.Minutes.Font =  new System.Drawing.Font("AR DELANEY", 10);
			this.Minutes.Size = new System.Drawing.Size(100, 25);
			this.Minutes.TabIndex = 3;
			this.Minutes.Text = "��";
			this.Minutes.SelectedIndexChanged += new System.EventHandler(this.Minutes_SelectedIndexChanged);
			//
			// BAddButton
			// 
			this.BAddButton.Name = "BAddButton";
			this.BAddButton.Text = "Add";
			this.BAddButton.BackColor = System.Drawing.Color.LightCyan;
			this.BAddButton.Font = new System.Drawing.Font("AR CENA", 10);
			this.BAddButton.Size = new System.Drawing.Size(100, 30);
			this.BAddButton.Location = new System.Drawing.Point(80, 120);
			this.BAddButton.Click += new System.EventHandler(this.BAddButton_Click);
			//
			// Panel
			//
			this.Panel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.Panel.Name = "Panel";
			this.Panel.Size = new System.Drawing.Size(280, 180);
			this.Panel.Location = new System.Drawing.Point(10, 10);
			this.Panel.Controls.AddRange(new System.Windows.Forms.Control[]{this.Hours, this.Minutes, this.BAddButton, this.Title});
			//
			this.Timer.Tick += new System.EventHandler(this.Timer_Tick);	
			this.Timer.Enabled = true;
			this.Timer.Interval = 1000;
			//
			//
			// AddForm
			//
			this.Name = "AddForm";	
			this.Text = "Add Alarm";
			this.TabIndex = 0;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog; // �� ũ�� ����
			this.ClientSize = new System.Drawing.Size(300, 200);
			this.Controls.AddRange(new System.Windows.Forms.Control[]{this.Panel});
			this.Font = new System.Drawing.Font("��������ڵ�", 12F);
			this.Panel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		private System.Windows.Forms.TextBox Title;
		private System.Windows.Forms.Panel Panel;
		private System.Windows.Forms.ComboBox Hours;
		private System.Windows.Forms.ComboBox Minutes;
		private System.Windows.Forms.Button BAddButton;
		private System.Windows.Forms.Timer Timer;




	}
}