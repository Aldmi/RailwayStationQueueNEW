namespace Server.View
{
    partial class MainForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.TicketNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cashier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button3 = new System.Windows.Forms.Button();
            this.btn_V3_Sucsess = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_V2_Sucsess = new System.Windows.Forms.Button();
            this.btn_V1_Error = new System.Windows.Forms.Button();
            this.btn_V1_Sucsess = new System.Windows.Forms.Button();
            this.btn_V3_Add = new System.Windows.Forms.Button();
            this.btn_V2_Add = new System.Windows.Forms.Button();
            this.btn_V1_Add = new System.Windows.Forms.Button();
            this.btn_long = new System.Windows.Forms.Button();
            this.btn_vilage = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.DimGray;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.CausesValidation = false;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Crimson;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeight = 150;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TicketNumber,
            this.Cashier});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.GridColor = System.Drawing.Color.White;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.DarkRed;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 200;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.White;
            this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView1.RowTemplate.Height = 100;
            this.dataGridView1.RowTemplate.ReadOnly = true;
            this.dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.ShowRowErrors = false;
            this.dataGridView1.Size = new System.Drawing.Size(1750, 937);
            this.dataGridView1.TabIndex = 0;
            // 
            // TicketNumber
            // 
            this.TicketNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            this.TicketNumber.DefaultCellStyle = dataGridViewCellStyle2;
            this.TicketNumber.HeaderText = "НОМЕР ТАЛОНА";
            this.TicketNumber.Name = "TicketNumber";
            this.TicketNumber.ReadOnly = true;
            this.TicketNumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Cashier
            // 
            this.Cashier.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Gray;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            this.Cashier.DefaultCellStyle = dataGridViewCellStyle3;
            this.Cashier.HeaderText = "КАССИР";
            this.Cashier.Name = "Cashier";
            this.Cashier.ReadOnly = true;
            this.Cashier.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(917, 724);
            this.button3.Margin = new System.Windows.Forms.Padding(6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(74, 60);
            this.button3.TabIndex = 28;
            this.button3.Text = "V3 o";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // btn_V3_Sucsess
            // 
            this.btn_V3_Sucsess.Location = new System.Drawing.Point(831, 726);
            this.btn_V3_Sucsess.Margin = new System.Windows.Forms.Padding(6);
            this.btn_V3_Sucsess.Name = "btn_V3_Sucsess";
            this.btn_V3_Sucsess.Size = new System.Drawing.Size(78, 60);
            this.btn_V3_Sucsess.TabIndex = 27;
            this.btn_V3_Sucsess.Text = "V3 x";
            this.btn_V3_Sucsess.UseVisualStyleBackColor = true;
            this.btn_V3_Sucsess.Click += new System.EventHandler(this.btn_V3_Sucsess_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(615, 728);
            this.button1.Margin = new System.Windows.Forms.Padding(6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(74, 60);
            this.button1.TabIndex = 26;
            this.button1.Text = "V2 o";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_V2_Sucsess
            // 
            this.btn_V2_Sucsess.Location = new System.Drawing.Point(525, 726);
            this.btn_V2_Sucsess.Margin = new System.Windows.Forms.Padding(6);
            this.btn_V2_Sucsess.Name = "btn_V2_Sucsess";
            this.btn_V2_Sucsess.Size = new System.Drawing.Size(78, 60);
            this.btn_V2_Sucsess.TabIndex = 25;
            this.btn_V2_Sucsess.Text = "V2 x";
            this.btn_V2_Sucsess.UseVisualStyleBackColor = true;
            this.btn_V2_Sucsess.Click += new System.EventHandler(this.btn_V2_Sucsess_Click);
            // 
            // btn_V1_Error
            // 
            this.btn_V1_Error.Location = new System.Drawing.Point(341, 726);
            this.btn_V1_Error.Margin = new System.Windows.Forms.Padding(6);
            this.btn_V1_Error.Name = "btn_V1_Error";
            this.btn_V1_Error.Size = new System.Drawing.Size(74, 60);
            this.btn_V1_Error.TabIndex = 24;
            this.btn_V1_Error.Text = "V1 o";
            this.btn_V1_Error.UseVisualStyleBackColor = true;
            this.btn_V1_Error.Click += new System.EventHandler(this.btn_V1_Error_Click);
            // 
            // btn_V1_Sucsess
            // 
            this.btn_V1_Sucsess.Location = new System.Drawing.Point(251, 726);
            this.btn_V1_Sucsess.Margin = new System.Windows.Forms.Padding(6);
            this.btn_V1_Sucsess.Name = "btn_V1_Sucsess";
            this.btn_V1_Sucsess.Size = new System.Drawing.Size(78, 60);
            this.btn_V1_Sucsess.TabIndex = 23;
            this.btn_V1_Sucsess.Text = "V1 x";
            this.btn_V1_Sucsess.UseVisualStyleBackColor = true;
            this.btn_V1_Sucsess.Click += new System.EventHandler(this.btn_V1_Sucsess_Click);
            // 
            // btn_V3_Add
            // 
            this.btn_V3_Add.Location = new System.Drawing.Point(871, 659);
            this.btn_V3_Add.Margin = new System.Windows.Forms.Padding(6);
            this.btn_V3_Add.Name = "btn_V3_Add";
            this.btn_V3_Add.Size = new System.Drawing.Size(94, 60);
            this.btn_V3_Add.TabIndex = 22;
            this.btn_V3_Add.Text = "V3 +";
            this.btn_V3_Add.UseVisualStyleBackColor = true;
            this.btn_V3_Add.Click += new System.EventHandler(this.btn_V3_Add_Click);
            // 
            // btn_V2_Add
            // 
            this.btn_V2_Add.Location = new System.Drawing.Point(561, 659);
            this.btn_V2_Add.Margin = new System.Windows.Forms.Padding(6);
            this.btn_V2_Add.Name = "btn_V2_Add";
            this.btn_V2_Add.Size = new System.Drawing.Size(90, 60);
            this.btn_V2_Add.TabIndex = 21;
            this.btn_V2_Add.Text = "V2 +";
            this.btn_V2_Add.UseVisualStyleBackColor = true;
            this.btn_V2_Add.Click += new System.EventHandler(this.btn_V2_Add_Click);
            // 
            // btn_V1_Add
            // 
            this.btn_V1_Add.Location = new System.Drawing.Point(289, 659);
            this.btn_V1_Add.Margin = new System.Windows.Forms.Padding(6);
            this.btn_V1_Add.Name = "btn_V1_Add";
            this.btn_V1_Add.Size = new System.Drawing.Size(84, 60);
            this.btn_V1_Add.TabIndex = 20;
            this.btn_V1_Add.Text = "V1 +";
            this.btn_V1_Add.UseVisualStyleBackColor = true;
            this.btn_V1_Add.Click += new System.EventHandler(this.btn_V1_Add_Click);
            // 
            // btn_long
            // 
            this.btn_long.Location = new System.Drawing.Point(15, 809);
            this.btn_long.Margin = new System.Windows.Forms.Padding(6);
            this.btn_long.Name = "btn_long";
            this.btn_long.Size = new System.Drawing.Size(176, 113);
            this.btn_long.TabIndex = 19;
            this.btn_long.Text = "дальние";
            this.btn_long.UseVisualStyleBackColor = true;
            this.btn_long.Click += new System.EventHandler(this.btn_long_Click);
            // 
            // btn_vilage
            // 
            this.btn_vilage.Location = new System.Drawing.Point(15, 659);
            this.btn_vilage.Margin = new System.Windows.Forms.Padding(6);
            this.btn_vilage.Name = "btn_vilage";
            this.btn_vilage.Size = new System.Drawing.Size(176, 113);
            this.btn_vilage.TabIndex = 18;
            this.btn_vilage.Text = "электропоезд";
            this.btn_vilage.UseVisualStyleBackColor = true;
            this.btn_vilage.Click += new System.EventHandler(this.btn_vilage_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1750, 937);
            this.ControlBox = false;
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btn_V3_Sucsess);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_V2_Sucsess);
            this.Controls.Add(this.btn_V1_Error);
            this.Controls.Add(this.btn_V1_Sucsess);
            this.Controls.Add(this.btn_V3_Add);
            this.Controls.Add(this.btn_V2_Add);
            this.Controls.Add(this.btn_V1_Add);
            this.Controls.Add(this.btn_long);
            this.Controls.Add(this.btn_vilage);
            this.Controls.Add(this.dataGridView1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn TicketNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cashier;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btn_V3_Sucsess;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_V2_Sucsess;
        private System.Windows.Forms.Button btn_V1_Error;
        private System.Windows.Forms.Button btn_V1_Sucsess;
        private System.Windows.Forms.Button btn_V3_Add;
        private System.Windows.Forms.Button btn_V2_Add;
        private System.Windows.Forms.Button btn_V1_Add;
        private System.Windows.Forms.Button btn_long;
        private System.Windows.Forms.Button btn_vilage;
    }
}

