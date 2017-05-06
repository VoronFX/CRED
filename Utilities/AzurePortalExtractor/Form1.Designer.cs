namespace AzurePortalExtractor
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
			this.Input = new System.Windows.Forms.TextBox();
			this.ExtractSvgToAngularTemplate = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.ExtractFromAzureSourcesBtn = new System.Windows.Forms.Button();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// Input
			// 
			this.Input.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Input.Location = new System.Drawing.Point(12, 134);
			this.Input.MaxLength = 1000000;
			this.Input.Multiline = true;
			this.Input.Name = "Input";
			this.Input.Size = new System.Drawing.Size(1131, 565);
			this.Input.TabIndex = 0;
			this.Input.WordWrap = false;
			// 
			// ExtractSvgToAngularTemplate
			// 
			this.ExtractSvgToAngularTemplate.Location = new System.Drawing.Point(12, 10);
			this.ExtractSvgToAngularTemplate.Name = "ExtractSvgToAngularTemplate";
			this.ExtractSvgToAngularTemplate.Size = new System.Drawing.Size(181, 23);
			this.ExtractSvgToAngularTemplate.TabIndex = 1;
			this.ExtractSvgToAngularTemplate.Text = "ExtractSvgToAngularTemplate";
			this.ExtractSvgToAngularTemplate.UseVisualStyleBackColor = true;
			this.ExtractSvgToAngularTemplate.Click += new System.EventHandler(this.ExtractSvgToAngularTemplate_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(199, 10);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(181, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "ExtractAllFxsClasseseNames";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(155, 39);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(554, 20);
			this.textBox1.TabIndex = 3;
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(155, 65);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(554, 20);
			this.textBox2.TabIndex = 4;
			// 
			// ExtractFromAzureSourcesBtn
			// 
			this.ExtractFromAzureSourcesBtn.Location = new System.Drawing.Point(386, 10);
			this.ExtractFromAzureSourcesBtn.Name = "ExtractFromAzureSourcesBtn";
			this.ExtractFromAzureSourcesBtn.Size = new System.Drawing.Size(181, 23);
			this.ExtractFromAzureSourcesBtn.TabIndex = 5;
			this.ExtractFromAzureSourcesBtn.Text = "ExtractFromAzureSources";
			this.ExtractFromAzureSourcesBtn.UseVisualStyleBackColor = true;
			this.ExtractFromAzureSourcesBtn.Click += new System.EventHandler(this.ExtractFromAzureSourcesBtn_Click);
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(155, 91);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(554, 20);
			this.textBox3.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(103, 42);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Sources";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(103, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(49, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "wwwroot";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(78, 94);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(74, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "relative output";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1155, 711);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.ExtractFromAzureSourcesBtn);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.ExtractSvgToAngularTemplate);
			this.Controls.Add(this.Input);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox Input;
		private System.Windows.Forms.Button ExtractSvgToAngularTemplate;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button ExtractFromAzureSourcesBtn;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
	}
}

