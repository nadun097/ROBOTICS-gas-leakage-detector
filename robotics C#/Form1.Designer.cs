using System;
using System.Windows.Forms;

namespace GasDetection
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblGasStatus;
        private System.Windows.Forms.Button btnToggleWindow;
        private System.Windows.Forms.Label lblWindows;
        private System.Windows.Forms.Button btnToggleValve;
        private System.Windows.Forms.Label lblFire; 



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
            this.lblMode = new System.Windows.Forms.Label();
            this.lblRisk = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnToggleValve = new System.Windows.Forms.Button();
            this.lblGasStatus = new System.Windows.Forms.Label();
            this.lblGasValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblMode
            // 
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(17, 26);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(34, 13);
            this.lblMode.TabIndex = 0;
            this.lblMode.Text = "Mode";
            // 
            // lblRisk
            // 
            this.lblRisk.AutoSize = true;
            this.lblRisk.Location = new System.Drawing.Point(20, 50);
            this.lblRisk.Name = "lblRisk";
            this.lblRisk.Size = new System.Drawing.Size(28, 13);
            this.lblRisk.TabIndex = 1;
            this.lblRisk.Text = "Risk";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(20, 80);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(200, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // btnToggleValve
            // 
            this.btnToggleValve.Location = new System.Drawing.Point(20, 0);
            this.btnToggleValve.Name = "btnToggleValve";
            this.btnToggleValve.Size = new System.Drawing.Size(75, 23);
            this.btnToggleValve.TabIndex = 3;
            // 
            // lblGasStatus
            // 
            this.lblGasStatus.Location = new System.Drawing.Point(0, 0);
            this.lblGasStatus.Name = "lblGasStatus";
            this.lblGasStatus.Size = new System.Drawing.Size(100, 23);
            this.lblGasStatus.TabIndex = 4;
            // 
            // lblGasValue
            // 
            this.lblGasValue.Location = new System.Drawing.Point(0, 0);
            this.lblGasValue.Name = "lblGasValue";
            this.lblGasValue.Size = new System.Drawing.Size(100, 23);
            this.lblGasValue.TabIndex = 5;
            this.lblGasValue.Text = "Gas Value: 0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblMode);
            this.Controls.Add(this.lblRisk);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnToggleValve);
            this.Controls.Add(this.lblGasStatus);
            this.Controls.Add(this.lblGasValue);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private Label lblGasValue;
    }
}

