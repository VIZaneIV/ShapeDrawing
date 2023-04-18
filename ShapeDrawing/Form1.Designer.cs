
namespace ShapeDrawing
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Canvas = new System.Windows.Forms.PictureBox();
            this.thicknessButton = new System.Windows.Forms.Button();
            this.thicknessBar = new System.Windows.Forms.TrackBar();
            this.thicknessBox = new System.Windows.Forms.TextBox();
            this.brushButton = new System.Windows.Forms.Button();
            this.circleButton = new System.Windows.Forms.Button();
            this.polygonButton = new System.Windows.Forms.Button();
            this.grabButton = new System.Windows.Forms.Button();
            this.capsuleButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Canvas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.thicknessBar)).BeginInit();
            this.SuspendLayout();
            // 
            // Canvas
            // 
            this.Canvas.Location = new System.Drawing.Point(14, 16);
            this.Canvas.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Canvas.Name = "Canvas";
            this.Canvas.Size = new System.Drawing.Size(594, 568);
            this.Canvas.TabIndex = 0;
            this.Canvas.TabStop = false;
            this.Canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseDown);
            this.Canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseMove);
            this.Canvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Canvas_MouseUp);
            // 
            // thicknessButton
            // 
            this.thicknessButton.Location = new System.Drawing.Point(615, 16);
            this.thicknessButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.thicknessButton.Name = "thicknessButton";
            this.thicknessButton.Size = new System.Drawing.Size(93, 31);
            this.thicknessButton.TabIndex = 1;
            this.thicknessButton.Text = "Line";
            this.thicknessButton.UseVisualStyleBackColor = true;
            this.thicknessButton.Click += new System.EventHandler(this.thicknessButton_Click);
            // 
            // thicknessBar
            // 
            this.thicknessBar.Location = new System.Drawing.Point(714, 16);
            this.thicknessBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.thicknessBar.Maximum = 5;
            this.thicknessBar.Minimum = 1;
            this.thicknessBar.Name = "thicknessBar";
            this.thicknessBar.Size = new System.Drawing.Size(147, 56);
            this.thicknessBar.TabIndex = 2;
            this.thicknessBar.Value = 1;
            // 
            // thicknessBox
            // 
            this.thicknessBox.Location = new System.Drawing.Point(869, 16);
            this.thicknessBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.thicknessBox.Name = "thicknessBox";
            this.thicknessBox.Size = new System.Drawing.Size(31, 27);
            this.thicknessBox.TabIndex = 3;
            this.thicknessBox.Text = "1";
            this.thicknessBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // brushButton
            // 
            this.brushButton.Location = new System.Drawing.Point(615, 93);
            this.brushButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.brushButton.Name = "brushButton";
            this.brushButton.Size = new System.Drawing.Size(93, 31);
            this.brushButton.TabIndex = 4;
            this.brushButton.Text = "Brush";
            this.brushButton.UseVisualStyleBackColor = true;
            this.brushButton.Click += new System.EventHandler(this.brushButton_Click);
            // 
            // circleButton
            // 
            this.circleButton.Location = new System.Drawing.Point(615, 132);
            this.circleButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.circleButton.Name = "circleButton";
            this.circleButton.Size = new System.Drawing.Size(93, 31);
            this.circleButton.TabIndex = 5;
            this.circleButton.Text = "Circle";
            this.circleButton.UseVisualStyleBackColor = true;
            this.circleButton.Click += new System.EventHandler(this.circleButton_Click);
            // 
            // polygonButton
            // 
            this.polygonButton.Location = new System.Drawing.Point(615, 55);
            this.polygonButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.polygonButton.Name = "polygonButton";
            this.polygonButton.Size = new System.Drawing.Size(93, 31);
            this.polygonButton.TabIndex = 6;
            this.polygonButton.Text = "Polygon";
            this.polygonButton.UseVisualStyleBackColor = true;
            this.polygonButton.Click += new System.EventHandler(this.polygonButton_Click);
            // 
            // grabButton
            // 
            this.grabButton.Location = new System.Drawing.Point(615, 553);
            this.grabButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grabButton.Name = "grabButton";
            this.grabButton.Size = new System.Drawing.Size(93, 31);
            this.grabButton.TabIndex = 7;
            this.grabButton.Text = "Grab";
            this.grabButton.UseVisualStyleBackColor = true;
            this.grabButton.Click += new System.EventHandler(this.grabButton_Click);
            // 
            // capsuleButton
            // 
            this.capsuleButton.Location = new System.Drawing.Point(615, 517);
            this.capsuleButton.Name = "capsuleButton";
            this.capsuleButton.Size = new System.Drawing.Size(94, 29);
            this.capsuleButton.TabIndex = 8;
            this.capsuleButton.Text = "Capsule";
            this.capsuleButton.UseVisualStyleBackColor = true;
            this.capsuleButton.Click += new System.EventHandler(this.capsuleButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 600);
            this.Controls.Add(this.capsuleButton);
            this.Controls.Add(this.grabButton);
            this.Controls.Add(this.polygonButton);
            this.Controls.Add(this.circleButton);
            this.Controls.Add(this.brushButton);
            this.Controls.Add(this.thicknessBox);
            this.Controls.Add(this.thicknessBar);
            this.Controls.Add(this.thicknessButton);
            this.Controls.Add(this.Canvas);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.Canvas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.thicknessBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Canvas;
        private System.Windows.Forms.Button thicknessButton;
        private System.Windows.Forms.TrackBar thicknessBar;
        private System.Windows.Forms.TextBox thicknessBox;
        private System.Windows.Forms.Button brushButton;
        private System.Windows.Forms.Button circleButton;
        private System.Windows.Forms.Button polygonButton;
        private System.Windows.Forms.Button grabButton;
        private System.Windows.Forms.Button capsuleButton;
    }
}

