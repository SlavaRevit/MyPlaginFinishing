namespace SelectionFilter
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.offsetLabel = new System.Windows.Forms.Label();
			this.HeightLabel = new System.Windows.Forms.Label();
			this.BaseOffsetLabel = new System.Windows.Forms.Label();
			this.offsetBox = new System.Windows.Forms.TextBox();
			this.HeightBox = new System.Windows.Forms.TextBox();
			this.BaseBox = new System.Windows.Forms.TextBox();
			this.OkButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(712, 299);
			this.label1.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 29);
			this.label1.TabIndex = 0;
			this.label1.Text = "label1";
			// 
			// offsetLabel
			// 
			this.offsetLabel.AutoSize = true;
			this.offsetLabel.Location = new System.Drawing.Point(12, 23);
			this.offsetLabel.Name = "offsetLabel";
			this.offsetLabel.Size = new System.Drawing.Size(203, 29);
			this.offsetLabel.TabIndex = 1;
			this.offsetLabel.Text = "Offset From Level";
			// 
			// HeightLabel
			// 
			this.HeightLabel.AutoSize = true;
			this.HeightLabel.Location = new System.Drawing.Point(12, 92);
			this.HeightLabel.Name = "HeightLabel";
			this.HeightLabel.Size = new System.Drawing.Size(210, 29);
			this.HeightLabel.TabIndex = 2;
			this.HeightLabel.Text = "Height From Level";
			// 
			// BaseOffsetLabel
			// 
			this.BaseOffsetLabel.AutoSize = true;
			this.BaseOffsetLabel.Location = new System.Drawing.Point(17, 162);
			this.BaseOffsetLabel.Name = "BaseOffsetLabel";
			this.BaseOffsetLabel.Size = new System.Drawing.Size(264, 29);
			this.BaseOffsetLabel.TabIndex = 3;
			this.BaseOffsetLabel.Text = "Base Offset From Level";
			// 
			// offsetBox
			// 
			this.offsetBox.Location = new System.Drawing.Point(303, 23);
			this.offsetBox.Name = "offsetBox";
			this.offsetBox.Size = new System.Drawing.Size(198, 35);
			this.offsetBox.TabIndex = 1;
			// 
			// HeightBox
			// 
			this.HeightBox.Location = new System.Drawing.Point(303, 86);
			this.HeightBox.Name = "HeightBox";
			this.HeightBox.Size = new System.Drawing.Size(198, 35);
			this.HeightBox.TabIndex = 2;
			// 
			// BaseBox
			// 
			this.BaseBox.Location = new System.Drawing.Point(303, 156);
			this.BaseBox.Name = "BaseBox";
			this.BaseBox.Size = new System.Drawing.Size(198, 35);
			this.BaseBox.TabIndex = 3;
			// 
			// OkButton
			// 
			this.OkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.OkButton.Location = new System.Drawing.Point(343, 232);
			this.OkButton.Name = "OkButton";
			this.OkButton.Size = new System.Drawing.Size(158, 57);
			this.OkButton.TabIndex = 4;
			this.OkButton.Text = "Next";
			this.OkButton.UseVisualStyleBackColor = true;
			this.OkButton.Click += new System.EventHandler(this.OkButton_Click_1);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(546, 301);
			this.Controls.Add(this.OkButton);
			this.Controls.Add(this.BaseBox);
			this.Controls.Add(this.HeightBox);
			this.Controls.Add(this.offsetBox);
			this.Controls.Add(this.BaseOffsetLabel);
			this.Controls.Add(this.HeightLabel);
			this.Controls.Add(this.offsetLabel);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Margin = new System.Windows.Forms.Padding(7);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label offsetLabel;
		private System.Windows.Forms.Label HeightLabel;
		private System.Windows.Forms.Label BaseOffsetLabel;
		private System.Windows.Forms.TextBox offsetBox;
		private System.Windows.Forms.TextBox HeightBox;
		private System.Windows.Forms.TextBox BaseBox;
		private System.Windows.Forms.Button OkButton;
	}
}