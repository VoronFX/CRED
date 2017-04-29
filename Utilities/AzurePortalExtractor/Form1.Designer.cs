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
			this.SuspendLayout();
			// 
			// Input
			// 
			this.Input.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Input.Location = new System.Drawing.Point(12, 39);
			this.Input.MaxLength = 1000000;
			this.Input.Multiline = true;
			this.Input.Name = "Input";
			this.Input.Size = new System.Drawing.Size(1131, 660);
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
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1155, 711);
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
	}
}

