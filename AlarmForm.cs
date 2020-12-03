namespace Alarm
{
	public partial class AlarmForm : System.Windows.Forms.Form
	{
		[System.Runtime.InteropServices.DllImport("winmm.dll")]
		public static extern int waveOutSetVolume(System.IntPtr hwo, uint dwVolume);

		private System.ComponentModel.IContainer components = null;
		
		private int? CheckedIndex = null;

		public AlarmForm()
		{
			InitializeComponent();	
			this.SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer | System.Windows.Forms.ControlStyles.UserPaint |
				 System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void AlarmForm_Load(object sender, System.EventArgs e)
		{
		
		}
		
		private void BStart_Click(object sender, System.EventArgs e)
		{	
			if(BStart.Text.Equals("Start"))
			{
				BStart.Text = "Stop";	
				timer.Enabled = true;
				timer.Interval = 1000;
				timer_Tick(sender, e);
				//BAdd.Enabled = false;
			}
			else
			{
				BStart.Text = "Start";
				timer.Stop();
				//BAdd.Enabled = true;	
			}	
		}
		
		private void timer_Tick(object  sender, System.EventArgs e)
		{
			LMessage.Text = "통신은 절대 꿀이 아니다 " + System.DateTime.Now.ToLongTimeString(/*"hh:mm:ss"*/);
			TimeCheck(int.Parse(System.DateTime.Now.ToString("HH")), int.Parse(System.DateTime.Now.ToString("mm")), int.Parse(System.DateTime.Now.ToString("ss")));
		}
		
		private void BAdd_Click(object sender, System.EventArgs e)
		{
			AddForm AddForm = new AddForm(this);
			AddForm.Owner = this;
			AddForm.FormSendEvent += new FormSendDataHandler(DieaseUpdateEventMethod);
			AddForm.ShowDialog();			
			AddForm.Dispose();
		}
		
		private void BEdit_Click(object sender, System.EventArgs e)
		{
			int CheckedCount = 0;
			
			if(System.Windows.Forms.MessageBox.Show("선택한 항목을 수정하시겠습니까?", "알람 수정", 
				System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
			{
				for(System.Int32 i = this.listView.Items.Count - 1; i >= 0; i--)
				{
					if(this.listView.Items[i].Checked)
					{
						CheckedCount++;
						CheckedIndex = i;
					}
				}

				if(CheckedCount != 1) // 선택된 항목이 없을 때
				{
					System.Windows.Forms.MessageBox.Show("여러 개의 항목이 선택되었거나 선택된 항목이 없습니다.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
					
					for(System.Int32 i = this.listView.Items.Count - 1; i >= 0; i--)
					{
						if(this.listView.Items[i].Checked)
						{
							this.listView.Items[i].Checked = false;	
							//break;
						}
					}

					return;
				}
				else // 제대로 선택이 됐을 때
				{
					string[] HourAndMinutes = this.listView.Items[this.CheckedIndex.Value].SubItems[2].Text.Split(new System.Char[] {':'}); // e.g) 12:20
				
					AddForm AddForm = new AddForm(this, this.listView.Items[CheckedIndex.Value].SubItems[1].Text, HourAndMinutes[0], HourAndMinutes[1]);
					AddForm.Owner = this;
					AddForm.EditAlarmEvent += new EditAlarmDelegate(Editer);
					AddForm.ShowDialog();			
					AddForm.Dispose();

					using(this.SW = new System.IO.StreamWriter(new System.IO.FileStream(@"AlarmList.txt", System.IO.FileMode.Create), System.Text.Encoding.Default))
					{
						SW.WriteLine((ReadCount - CheckedCount).ToString()); // Int32 형 그대로 Write하면 Encoding할 때, 다른 문자 값이 들어간다.
				
						for(System.Int32 i = 0; i < this.listView.Items.Count; i++)
						{	
							SW.WriteLine((i + 1).ToString() + ' ' + this.listView.Items[i].SubItems[1].Text + ' ' + this.listView.Items[i].SubItems[2].Text); 	
						}
					}
				}				
			}

			for(System.Int32 i = this.listView.Items.Count - 1; i >= 0; i--)
			{
				if(this.listView.Items[i].Checked)
				{
					this.listView.Items[i].Checked = false;	
					break;
				}
			}
		}

		private void BDelete_Click(object sender, System.EventArgs e)
		{
			int CheckedCount = 0;

			if(System.Windows.Forms.MessageBox.Show("선택한 항목들이 삭제됩니다. \r 계속하시겠습니까?", "항목 삭제", 
				System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
			{	
				for(System.Int32 i = this.listView.Items.Count - 1; i >= 0; i--)
				{
					if(this.listView.Items[i].Checked)
					{
						this.listView.Items[i].Remove();
						CheckedCount++;
					}
				}
				
				if(CheckedCount == 0) 
				{
					System.Windows.Forms.MessageBox.Show("선택된 항목이 없습니다.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
					return;
				}

				using(this.SW = new System.IO.StreamWriter(new System.IO.FileStream(@"AlarmList.txt", System.IO.FileMode.Create), System.Text.Encoding.Default))
				{
					SW.WriteLine((ReadCount - CheckedCount).ToString()); // Int32 형 그대로 Write하면 Encoding할 때, 다른 문자 값이 들어간다.
				
					for(System.Int32 i = 0; i < this.listView.Items.Count; i++)
					{
						SW.WriteLine((i + 1).ToString() + ' ' + this.listView.Items[i].SubItems[1].Text + ' ' + this.listView.Items[i].SubItems[2].Text); 	
					}
				} 
				
			}

			UpdateListView();
	
		}

		private void DieaseUpdateEventMethod(object content, object hour, object minute)
		{	
			using(this.SW = new System.IO.StreamWriter(new System.IO.FileStream(@"AlarmList.txt", System.IO.FileMode.Create), System.Text.Encoding.Default))
			{
				SW.WriteLine((++ReadCount).ToString()); // Int32 형 그대로 Write하면 Encoding할 때, 다른 문자 값이 들어간다.
				
				for(System.Int32 i = 0; i < this.listView.Items.Count; i++)
				{
					SW.WriteLine(this.listView.Items[i].SubItems[0].Text + ' ' + this.listView.Items[i].SubItems[1].Text + ' ' + this.listView.Items[i].SubItems[2].Text); 	
				}
				SW.WriteLine(ReadCount.ToString() + ' ' + content.ToString() + ' ' + hour.ToString() + ':' + minute.ToString());
			} 						
			UpdateListView();	
		}

		private void Editer(object content, object hour, object minute)
		{
			this.listView.Items[this.CheckedIndex.Value].SubItems[1].Text = (string)content;
			this.listView.Items[this.CheckedIndex.Value].SubItems[2].Text = (string)hour + ':' + (string)minute;			
		}

		private void UpdateListView()
		{
			string[] ReadSplitString;

			this.listView.Items.Clear();
			this.listView.BeginUpdate();
			
			using(this.SR = new System.IO.StreamReader(new System.IO.FileStream(@"AlarmList.txt", System.IO.FileMode.Open), System.Text.Encoding.Default))
			{
				ReadCount = int.Parse(SR.ReadLine()); // 알람 몇 개 등록돼 있는지...
				for(int i = 0; i < ReadCount; i++)
				{	
					ReadSplitString = SR.ReadLine().Split(new System.Char[] {' '});
					this.listView.Items.Add(new System.Windows.Forms.ListViewItem(new string[] {
						ReadSplitString[0], ReadSplitString[1], ReadSplitString[2], "  "}));
				}
			}
		
			this.listView.EndUpdate();						
			this.Refresh();
		}

		private void BCancel_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Application.Exit();
		}

		private void ExitToolStripMeunItem_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Application.Exit();
		}

		private void notifyIcon1_MouseDoubleClick(object sender, System.EventArgs e)
		{
			this.Visible = true; // 폼 표시
			if(this.WindowState == System.Windows.Forms.FormWindowState.Minimized)
			{
				this.WindowState = System.Windows.Forms.FormWindowState.Normal; // 최소화 멈춤
			} 
			// this.Activate(); // 폼 활성화
		}

		private void AlarmForm_Resize(object sender, System.EventArgs e)
		{
			if(this.WindowState == System.Windows.Forms.FormWindowState.Normal)
			{
				notifyIcon1.Visible = false;
			}
			else if(this.WindowState == System.Windows.Forms.FormWindowState.Maximized)
			{
				notifyIcon1.Visible = false;	
			}
			else if(this.WindowState == System.Windows.Forms.FormWindowState.Minimized)
			{
				notifyIcon1.Visible = true;
				this.Visible = false;
				this.notifyIcon1.ShowBalloonTip(1);
			}
		}

		private void TimeCheck(int NowHour, int NowMinute, int NowSecond)
		{
			string[] HourAndMinutes;
			int NowTime;
			int AlarmTime;
			int LeftTime;	
		
			for(System.Int32 i = 0; i < this.listView.Items.Count; i++)
			{
				this.listView.BeginUpdate();
				
				HourAndMinutes = this.listView.Items[i].SubItems[2].Text.Split(new System.Char[] {':'}); // e.g) 12:20
				
				NowTime = (NowHour * 3600) + (NowMinute * 60) + NowSecond;
				AlarmTime = (int.Parse(HourAndMinutes[0]) * 3600) + (int.Parse(HourAndMinutes[1]) * 60); // 알람 예정 시간.
				
				if(NowTime >= AlarmTime)
				{
					LeftTime = ((24 * 3600) + AlarmTime) - NowTime;
				}
				else
				{
					LeftTime = AlarmTime - NowTime;
					
				}
				this.listView.Items[i].SubItems[3].Text = (LeftTime / 3600).ToString() + ':' + (LeftTime % 3600 / 60).ToString() + ':' + (LeftTime % 3600 % 60).ToString(); 
				//this.listView.Items[i].SubItems[3].Text = LeftTime.ToString();
				
				this.listView.EndUpdate();				
				this.listView.Refresh();
					
			
				if(NowHour == int.Parse(HourAndMinutes[0]) && NowMinute == int.Parse(HourAndMinutes[1]) && NowSecond == 01) // 정해진 시간에 알람 울리기.
				{
					SimpleSound.Play();
					
					if(this.WindowState == System.Windows.Forms.FormWindowState.Minimized)
					{	
						this.Visible = true;
						this.WindowState = System.Windows.Forms.FormWindowState.Normal;
					}
					System.Windows.Forms.MessageBox.Show(this.listView.Items[i].SubItems[1].Text, "Alarm", System.Windows.Forms.MessageBoxButtons.OK);
				}
			}
			
		}

		private void Player_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			if(SimpleSound.IsLoadCompleted)
			{
				System.Console.WriteLine(".wav file 로드 완료");
			}
		}
		
		private void SetSoundVolume(int volume)
		{
			try
			{
				int NewVolume = (((ushort.MaxValue) / 10) * volume);
				uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff | (uint)NewVolume << 16));
				
				waveOutSetVolume(System.IntPtr.Zero, NewVolumeAllChannels);
			}
			catch(System.Exception)
			{
				
			}		

		}	

		private void InitializeComponent()
		{	
			this.components = new System.ComponentModel.Container();
			this.resources = new System.ComponentModel.ComponentResourceManager(typeof(AlarmForm));
			this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			//this.ButtonPanel = new System.Windows.Forms.Panel();
			this.BStart = new System.Windows.Forms.Button();
			this.BDelete = new System.Windows.Forms.Button();
			this.BAdd = new System.Windows.Forms.Button();
			this.BCancel = new System.Windows.Forms.Button();
			this.BEdit = new System.Windows.Forms.Button();
			this.LMessage = new System.Windows.Forms.Label();
			this.Groupbox = new System.Windows.Forms.GroupBox();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.NumData = new System.Windows.Forms.NumericUpDown();
			this.listView = this.listView = new System.Windows.Forms.ListView();
			 ListViewGroupPRE = new System.Windows.Forms.ListViewGroup("PRE");
			this.SortHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ContentHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.TimetoRingHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.LeftTimeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			// this.menuStrip = new System.Windows.Forms.MenuStrip();
			SimpleSound = new System.Media.SoundPlayer(@"Sound\ok.wav");
			this.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			//
			// notifyIcon1
			//
			this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
			this.notifyIcon1.Icon = new System.Drawing.Icon(@".\.ico files\alarm.ico");			
			this.notifyIcon1.Text = "Alarm";
			this.notifyIcon1.Visible = false;
			this.notifyIcon1.BalloonTipTitle = "Alarm";
			this.notifyIcon1.BalloonTipText = "알람을 추가 / 변경하시려면 여기를 클릭하세요";
			this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
			this.contextMenuStrip1.ResumeLayout(false);
			this.contextMenuStrip1.PerformLayout();
			//
			// contextMenuStrip1
			//
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[]{this.ExitToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(99, 26);
			//
			// ExitToolStripMenuItem
			//
			this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
			this.ExitToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.ExitToolStripMenuItem.Click += new System.EventHandler(ExitToolStripMeunItem_Click);
			this.ExitToolStripMenuItem.Text = "종료";
			//
			// BStart
			//
			this.BStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			//this.BStart.BackgroundImage = System.Drawing.Image.FromFile(@"C:\Users\a\Pictures\Sample Pictures\desert.jpg");
			this.BStart.Font = new System.Drawing.Font("AR DELANEY", 13.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.BStart.BackColor = System.Drawing.Color.SaddleBrown;
			this.AutoSize = true;
			//this.BStart.BackColor = System.Drawing.SystemColors.ScrollBar;
			this.BStart.Name = "BStart";
			this.BStart.Size = new System.Drawing.Size(100, 30);
			this.BStart.Location = new System.Drawing.Point(75, 320);
			this.BStart.TabIndex = 0;
			this.BStart.Text = "Start";
			this.BStart.UseVisualStyleBackColor = false;
			this.BStart.Click += new System.EventHandler(this.BStart_Click);
			//		
			// BDelete
			//
			this.BDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.BDelete.Font = new System.Drawing.Font("AR DARLING", 13.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			//this.BDelete.BackgroundImage = System.Drawing.Image.FromFile(@"C:\Users\a\Pictures\Sample Pictures\desert.jpg");
			this.BDelete.BackColor = System.Drawing.Color.Firebrick;
			this.BDelete.Size = new System.Drawing.Size(100, 30);
			this.BDelete.Text = "Delete";
			this.BDelete.Location = new System.Drawing.Point(315, 320);
			this.BDelete.TabIndex = 2;
			this.BDelete.UseVisualStyleBackColor = false;
			this.BDelete.Click += new System.EventHandler(this.BDelete_Click);
			//
			// BAdd
			//
			this.BAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			//this.BAdd.BackgroundImage = System.Drawing.Image.FromFile(@"C:\Users\a\Pictures\Sample Pictures\desert.jpg");
			this.BAdd.BackColor = System.Drawing.Color.LavenderBlush;
			this.BAdd.Font = new System.Drawing.Font("AR ESSENCE", 13.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.BAdd.Name = "BAdd";
			this.BAdd.Text = "Add Alarm";
			this.BAdd.Size = new System.Drawing.Size(100, 30);
			this.BAdd.Location = new System.Drawing.Point(195, 320);
			this.BAdd.TabIndex = 1;
			this.BAdd.UseVisualStyleBackColor = false;
			this.BAdd.Click += new System.EventHandler(this.BAdd_Click);
			//
			// BEdit
			//
			this.BEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			//thisBEdit.BackgroundImage = System.Drawing.Image.FromFile(@"C:\Users\a\Pictures\Sample Pictures\desert.jpg");
			this.BEdit.BackColor = System.Drawing.Color.Goldenrod;
			this.BEdit.Font = new System.Drawing.Font("AR ESSENCE", 13.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.BEdit.Name = "BEdit";
			this.BEdit.Text = "Edit Alarm";
			this.BEdit.Size = new System.Drawing.Size(100, 30);
			this.BEdit.Location = new System.Drawing.Point(135, 360);
			this.BEdit.TabIndex = 3;
			this.BEdit.UseVisualStyleBackColor = false;
			this.BEdit.Click += new System.EventHandler(BEdit_Click);
			//
			// BCancel
			//
			this.BCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			//this.BCancel.AutoSize = true;
			//this.BCancel.BackgroundImage = System.Drawing.Image.FromFile(@"C:\Users\a\Pictures\Sample Pictures\desert.jpg");
			// this.BCancel.BackgroundImageLayout = System.Drawing.ImageLayout.Center;
			this.BCancel.BackColor = System.Drawing.Color.LawnGreen;
			this.BCancel.Font = new System.Drawing.Font("AR DELANEY", 13.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.BCancel.Name = "BCancel";
			this.BCancel.Size = new System.Drawing.Size(100, 30);
			this.BCancel.Location = new System.Drawing.Point(265, 360);
			this.BCancel.TabIndex = 4;
			this.BCancel.Text = "Exit";
			this.BCancel.UseVisualStyleBackColor = false;
			this.BCancel.Click += new System.EventHandler(this.BCancel_Click);
			//
			// ButtonPanel
			//
			//this.Location = new System.Drawing.Point(300, 400);
			//this.Size = new System.Drawing.Size(400, 40);
			//this.Controls.AddRange(new System.Windows.Forms.Control[]{this.BStart, this.BAdd, this.BCancel});
			//
			// LMessage
			//
			this.LMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			//this.LMessage.Font = new System.Drawing.Font("AR DESTINE", 19.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.LMessage.Font = new System.Drawing.Font("AR CARTER", 20);
			this.LMessage.AutoSize = true;
			this.LMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.LMessage.Location = new System.Drawing.Point(40, 10);
			this.LMessage.Name = "Message";
			this.LMessage.Size = new System.Drawing.Size(17,7);
			//this.LMessage.TabIndex = 1;
			//
			// GroupBox
			//
			this.Groupbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
			this.Groupbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			//this.Groupbox.BorderColor = System.Drawing.Color.Black;
			//this.BackgroundImage = System.Drawing.Image.FromFile(@"C:\Users\a\Pictures\Sample Pictures\koala.jpg");
			this.Groupbox.BackColor = System.Drawing.Color.DarkOrchid;
			this.Groupbox.Controls.AddRange(new System.Windows.Forms.Control[]{this.listView});
			this.Groupbox.Location = new System.Drawing.Point(10, 60);
			this.Groupbox.Name = "GroupBox";
			this.Groupbox.Size = new System.Drawing.Size(480, 250);
			this.Groupbox.TabIndex = 5;
			this.Groupbox.TabStop = false;
			this.Groupbox.Text = "【Alarm List】";
			this.Groupbox.Font = new System.Drawing.Font("AR CHRISTY", 10);
			//this.Groupbox.Font = new System.Drawing.Font("AR DELANEY", 9.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			//	
			// timer
			//
			this.timer.Tick += new System.EventHandler(this.timer_Tick);	
			//
			// NumData
			//
			this.NumData.Location = new System.Drawing.Point(700, 145);
			this.NumData.Name = "NumData";
			this.NumData.Size = new System.Drawing.Size(67, 21);
			//this.NumData.TabIndex = 5;
			((System.ComponentModel.ISupportInitialize)(this.NumData)).BeginInit();
			// 
			// listView
			//
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.Location = new System.Drawing.Point(190, 120);
			this.listView.Name = "listView";
			this.listView.FullRowSelect = true;
			this.listView.GridLines = false;
			this.listView.MultiSelect = true;
			this.listView.Size = new System.Drawing.Size(500, 250);
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.FullRowSelect = true;
			this.listView.GridLines = true;
			this.listView.CheckBoxes = true;

			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
				this.SortHeader,
				this.ContentHeader,
				this.TimetoRingHeader,
				this.LeftTimeHeader
			});

			for(int i = 0; i < this.listView.Columns.Count; i++)
			{
				this.listView.Columns[i].TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			}
			
			UpdateListView();
			//
			//
			// SortHeader
			//
			this.SortHeader.Text = "구분";
			this.SortHeader.Width = 50;
			//
			// ContentHeader			
			//
			this.ContentHeader.Text = "내용";
			this.ContentHeader.Width = 200;
			//
			// TimetoRingHeader
			//
			this.TimetoRingHeader.Text = "예정 시간";
			this.TimetoRingHeader.Width = 100;
			//
			// LeftTimeHeader
			//
			this.LeftTimeHeader.Text = "남은 시간";
			this.LeftTimeHeader.Width = 120;
			//
			// SimpleSound
			//
			SimpleSound.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(Player_LoadCompleted);
			SimpleSound.LoadAsync();
			//
			// AlarmForm
			//
			this.SuspendLayout();
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(600, 410);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog; // 폼 크기 고정
			this.MaximizeBox = false;		
			this.Name = "AlarmForm";
			this.Icon = this.notifyIcon1.Icon;
			this.Text = "Alarm";		
			this.DoubleBuffered = true;	
			this.Controls.AddRange(new System.Windows.Forms.Control[]{this.LMessage, this.Groupbox, this.BStart, this.BDelete, this.BAdd, this.BCancel, this.BEdit});
			this.Load += new System.EventHandler(this.AlarmForm_Load);
			this.Resize += new System.EventHandler(this.AlarmForm_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

	
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
		private System.ComponentModel.ComponentResourceManager resources;
		//private System.Windows.Forms.Panel ButtonPanel;
		private System.Windows.Forms.Button BStart;
		private System.Windows.Forms.Button BDelete;
		private System.Windows.Forms.Button BAdd;
		private System.Windows.Forms.Button BCancel;
		private System.Windows.Forms.Button BEdit;
		private System.Windows.Forms.Label LMessage;
		private System.Windows.Forms.GroupBox Groupbox;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.NumericUpDown NumData;
		private System.Media.SoundPlayer SimpleSound;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ListViewGroup ListViewGroupPRE;
		private System.Windows.Forms.ColumnHeader SortHeader;
		private System.Windows.Forms.ColumnHeader ContentHeader;
		private System.Windows.Forms.ColumnHeader TimetoRingHeader;
		private System.Windows.Forms.ColumnHeader LeftTimeHeader;
		private int ReadCount = 0;
		private System.IO.StreamReader SR;
		private System.IO.StreamWriter SW;




	}
}  