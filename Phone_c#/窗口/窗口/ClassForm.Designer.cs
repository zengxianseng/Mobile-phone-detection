namespace 窗口
{
    partial class ClassForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.SelectBtn1 = new System.Windows.Forms.Button();
            this.SelectBtn2 = new System.Windows.Forms.Button();
            this.address1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.address2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "主类别:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(12, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "次类别:";
            // 
            // SelectBtn1
            // 
            this.SelectBtn1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SelectBtn1.Location = new System.Drawing.Point(416, 68);
            this.SelectBtn1.Name = "SelectBtn1";
            this.SelectBtn1.Size = new System.Drawing.Size(80, 25);
            this.SelectBtn1.TabIndex = 2;
            this.SelectBtn1.Text = "选择";
            this.SelectBtn1.UseVisualStyleBackColor = true;
            this.SelectBtn1.Click += new System.EventHandler(this.SelectBtn1_Click);
            // 
            // SelectBtn2
            // 
            this.SelectBtn2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SelectBtn2.Location = new System.Drawing.Point(416, 197);
            this.SelectBtn2.Name = "SelectBtn2";
            this.SelectBtn2.Size = new System.Drawing.Size(80, 25);
            this.SelectBtn2.TabIndex = 3;
            this.SelectBtn2.Text = "选择";
            this.SelectBtn2.UseVisualStyleBackColor = true;
            this.SelectBtn2.Click += new System.EventHandler(this.SelectBtn2_Click);
            // 
            // address1
            // 
            this.address1.AutoSize = true;
            this.address1.Location = new System.Drawing.Point(33, 75);
            this.address1.Name = "address1";
            this.address1.Size = new System.Drawing.Size(47, 12);
            this.address1.TabIndex = 4;
            this.address1.Text = "address";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(37, 113);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(459, 21);
            this.textBox1.TabIndex = 5;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(37, 238);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(459, 21);
            this.textBox2.TabIndex = 6;
            // 
            // address2
            // 
            this.address2.AutoSize = true;
            this.address2.Location = new System.Drawing.Point(33, 204);
            this.address2.Name = "address2";
            this.address2.Size = new System.Drawing.Size(47, 12);
            this.address2.TabIndex = 7;
            this.address2.Text = "address";
            // 
            // ClassForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 288);
            this.Controls.Add(this.address2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.address1);
            this.Controls.Add(this.SelectBtn2);
            this.Controls.Add(this.SelectBtn1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ClassForm";
            this.Text = "类别设置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SelectBtn1;
        private System.Windows.Forms.Button SelectBtn2;
        private System.Windows.Forms.Label address1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label address2;
    }
}