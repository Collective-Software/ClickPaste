namespace ClickPaste
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Method_ScanCode = new System.Windows.Forms.RadioButton();
            this.Method_AutoIt = new System.Windows.Forms.RadioButton();
            this.Method_Forms = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.startDelayMS = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.DelayMS = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.HotKey_Windows = new System.Windows.Forms.CheckBox();
            this.HotKey_Shift = new System.Windows.Forms.CheckBox();
            this.HotKey_Control = new System.Windows.Forms.CheckBox();
            this.HotKey_Alt = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.HotKey_Letter = new System.Windows.Forms.TextBox();
            this.Done = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.confirmOverActive = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.confirmOver = new System.Windows.Forms.TextBox();
            this.hotKeyModeTarget = new System.Windows.Forms.RadioButton();
            this.hotKeyModeType = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.Method_ScanCode);
            this.groupBox1.Controls.Add(this.Method_AutoIt);
            this.groupBox1.Controls.Add(this.Method_Forms);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(207, 70);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Typing Method:";
            // 
            // Method_AutoIt
            // 
            this.Method_AutoIt.AutoSize = true;
            this.Method_AutoIt.Location = new System.Drawing.Point(116, 19);
            this.Method_AutoIt.Name = "Method_AutoIt";
            this.Method_AutoIt.Size = new System.Drawing.Size(81, 17);
            this.Method_AutoIt.TabIndex = 2;
            this.Method_AutoIt.TabStop = true;
            this.Method_AutoIt.Tag = "1";
            this.Method_AutoIt.Text = "AutoIt Send";
            this.Method_AutoIt.UseVisualStyleBackColor = true;
            // 
            // Method_Forms
            // 
            this.Method_Forms.AutoSize = true;
            this.Method_Forms.Location = new System.Drawing.Point(6, 19);
            this.Method_Forms.Name = "Method_Forms";
            this.Method_Forms.Size = new System.Drawing.Size(104, 17);
            this.Method_Forms.TabIndex = 1;
            this.Method_Forms.TabStop = true;
            this.Method_Forms.Tag = "0";
            this.Method_Forms.Text = "Forms.SendKeys";
            this.Method_Forms.UseVisualStyleBackColor = true;
            //
            // Method_ScanCode
            //
            this.Method_ScanCode.AutoSize = true;
            this.Method_ScanCode.Location = new System.Drawing.Point(6, 42);
            this.Method_ScanCode.Name = "Method_ScanCode";
            this.Method_ScanCode.Size = new System.Drawing.Size(165, 17);
            this.Method_ScanCode.TabIndex = 4;
            this.Method_ScanCode.Tag = "3";
            this.Method_ScanCode.Text = "SendInput";
            this.Method_ScanCode.UseVisualStyleBackColor = true;
            //
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.startDelayMS);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.DelayMS);
            this.groupBox2.Location = new System.Drawing.Point(15, 67);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(204, 79);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Delays:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "milliseconds between keys";
            // 
            // startDelayMS
            // 
            this.startDelayMS.Location = new System.Drawing.Point(6, 19);
            this.startDelayMS.Name = "startDelayMS";
            this.startDelayMS.Size = new System.Drawing.Size(47, 20);
            this.startDelayMS.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "milliseconds before start";
            // 
            // DelayMS
            // 
            this.DelayMS.Location = new System.Drawing.Point(6, 45);
            this.DelayMS.Name = "DelayMS";
            this.DelayMS.Size = new System.Drawing.Size(47, 20);
            this.DelayMS.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.hotKeyModeType);
            this.groupBox3.Controls.Add(this.hotKeyModeTarget);
            this.groupBox3.Controls.Add(this.HotKey_Windows);
            this.groupBox3.Controls.Add(this.HotKey_Shift);
            this.groupBox3.Controls.Add(this.HotKey_Control);
            this.groupBox3.Controls.Add(this.HotKey_Alt);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.HotKey_Letter);
            this.groupBox3.Location = new System.Drawing.Point(15, 208);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(204, 155);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Hot Key:";
            // 
            // HotKey_Windows
            // 
            this.HotKey_Windows.AutoSize = true;
            this.HotKey_Windows.Location = new System.Drawing.Point(58, 69);
            this.HotKey_Windows.Name = "HotKey_Windows";
            this.HotKey_Windows.Size = new System.Drawing.Size(70, 17);
            this.HotKey_Windows.TabIndex = 5;
            this.HotKey_Windows.Tag = "8";
            this.HotKey_Windows.Text = "Windows";
            this.HotKey_Windows.UseVisualStyleBackColor = true;
            // 
            // HotKey_Shift
            // 
            this.HotKey_Shift.AutoSize = true;
            this.HotKey_Shift.Location = new System.Drawing.Point(6, 69);
            this.HotKey_Shift.Name = "HotKey_Shift";
            this.HotKey_Shift.Size = new System.Drawing.Size(47, 17);
            this.HotKey_Shift.TabIndex = 4;
            this.HotKey_Shift.Tag = "4";
            this.HotKey_Shift.Text = "Shift";
            this.HotKey_Shift.UseVisualStyleBackColor = true;
            // 
            // HotKey_Control
            // 
            this.HotKey_Control.AutoSize = true;
            this.HotKey_Control.Location = new System.Drawing.Point(58, 46);
            this.HotKey_Control.Name = "HotKey_Control";
            this.HotKey_Control.Size = new System.Drawing.Size(59, 17);
            this.HotKey_Control.TabIndex = 3;
            this.HotKey_Control.Tag = "2";
            this.HotKey_Control.Text = "Control";
            this.HotKey_Control.UseVisualStyleBackColor = true;
            // 
            // HotKey_Alt
            // 
            this.HotKey_Alt.AutoSize = true;
            this.HotKey_Alt.Location = new System.Drawing.Point(6, 46);
            this.HotKey_Alt.Name = "HotKey_Alt";
            this.HotKey_Alt.Size = new System.Drawing.Size(38, 17);
            this.HotKey_Alt.TabIndex = 2;
            this.HotKey_Alt.Tag = "1";
            this.HotKey_Alt.Text = "Alt";
            this.HotKey_Alt.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(72, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "key with";
            // 
            // HotKey_Letter
            // 
            this.HotKey_Letter.Location = new System.Drawing.Point(6, 19);
            this.HotKey_Letter.Name = "HotKey_Letter";
            this.HotKey_Letter.Size = new System.Drawing.Size(60, 20);
            this.HotKey_Letter.TabIndex = 1;
            this.HotKey_Letter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotKey_Letter_KeyDown);
            // 
            // Done
            // 
            this.Done.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Done.Location = new System.Drawing.Point(77, 386);
            this.Done.Name = "Done";
            this.Done.Size = new System.Drawing.Size(75, 23);
            this.Done.TabIndex = 5;
            this.Done.Text = "Done";
            this.Done.UseVisualStyleBackColor = true;
            this.Done.Click += new System.EventHandler(this.Done_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.confirmOverActive);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.confirmOver);
            this.groupBox4.Location = new System.Drawing.Point(15, 152);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(204, 50);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            // 
            // confirmOverActive
            // 
            this.confirmOverActive.AutoSize = true;
            this.confirmOverActive.Location = new System.Drawing.Point(6, 0);
            this.confirmOverActive.Name = "confirmOverActive";
            this.confirmOverActive.Size = new System.Drawing.Size(159, 17);
            this.confirmOverActive.TabIndex = 1;
            this.confirmOverActive.Text = "Confirm if pasting more than:";
            this.confirmOverActive.UseVisualStyleBackColor = true;
            this.confirmOverActive.CheckedChanged += new System.EventHandler(this.confirmOverActive_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "keystrokes";
            // 
            // confirmOver
            // 
            this.confirmOver.Location = new System.Drawing.Point(6, 17);
            this.confirmOver.Name = "confirmOver";
            this.confirmOver.Size = new System.Drawing.Size(47, 20);
            this.confirmOver.TabIndex = 2;
            // 
            // hotKeyModeTarget
            // 
            this.hotKeyModeTarget.AutoSize = true;
            this.hotKeyModeTarget.Location = new System.Drawing.Point(6, 101);
            this.hotKeyModeTarget.Name = "hotKeyModeTarget";
            this.hotKeyModeTarget.Size = new System.Drawing.Size(122, 17);
            this.hotKeyModeTarget.TabIndex = 6;
            this.hotKeyModeTarget.TabStop = true;
            this.hotKeyModeTarget.Tag = "0";
            this.hotKeyModeTarget.Text = "Go into Target mode";
            this.hotKeyModeTarget.UseVisualStyleBackColor = true;
            // 
            // hotKeyModeType
            // 
            this.hotKeyModeType.AutoSize = true;
            this.hotKeyModeType.Location = new System.Drawing.Point(6, 124);
            this.hotKeyModeType.Name = "hotKeyModeType";
            this.hotKeyModeType.Size = new System.Drawing.Size(98, 17);
            this.hotKeyModeType.TabIndex = 7;
            this.hotKeyModeType.TabStop = true;
            this.hotKeyModeType.Tag = "1";
            this.hotKeyModeType.Text = "Just start typing";
            this.hotKeyModeType.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.Done;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(231, 421);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.Done);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClickPaste Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton Method_AutoIt;
        private System.Windows.Forms.RadioButton Method_Forms;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox DelayMS;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox HotKey_Letter;
        private System.Windows.Forms.CheckBox HotKey_Alt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox HotKey_Windows;
        private System.Windows.Forms.CheckBox HotKey_Shift;
        private System.Windows.Forms.CheckBox HotKey_Control;
        private System.Windows.Forms.Button Done;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox startDelayMS;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox confirmOver;
        private System.Windows.Forms.CheckBox confirmOverActive;
        private System.Windows.Forms.RadioButton hotKeyModeType;
        private System.Windows.Forms.RadioButton hotKeyModeTarget;
        private System.Windows.Forms.RadioButton Method_ScanCode;
    }
}