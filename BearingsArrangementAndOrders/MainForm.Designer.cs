namespace BearingsArrangementAndOrders
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.btFileNameSelect = new System.Windows.Forms.Button();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 53);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(256, 65);
            this.button1.TabIndex = 0;
            this.button1.Text = "Скомплектовать";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btFileNameSelect
            // 
            this.btFileNameSelect.BackgroundImage = global::BearingsArrangementAndOrders.Properties.Resources.open_file16;
            this.btFileNameSelect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btFileNameSelect.Location = new System.Drawing.Point(241, 12);
            this.btFileNameSelect.Name = "btFileNameSelect";
            this.btFileNameSelect.Size = new System.Drawing.Size(31, 35);
            this.btFileNameSelect.TabIndex = 1;
            this.btFileNameSelect.UseVisualStyleBackColor = true;
            this.btFileNameSelect.Click += new System.EventHandler(this.btFileNameSelect_Click);
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(16, 17);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(219, 20);
            this.tbFileName.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 127);
            this.Controls.Add(this.tbFileName);
            this.Controls.Add(this.btFileNameSelect);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "Комплектовка и заказ колец";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btFileNameSelect;
        private System.Windows.Forms.TextBox tbFileName;
    }
}

