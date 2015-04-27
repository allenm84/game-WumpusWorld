namespace WumpusWorld.WinForms
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
      this.components = new System.ComponentModel.Container();
      this.timerUpdate = new System.Windows.Forms.Timer(this.components);
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.btnUpdate = new System.Windows.Forms.Button();
      this.pnlScreen = new System.Windows.Forms.PanelEx();
      this.tableLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // timerUpdate
      // 
      this.timerUpdate.Enabled = true;
      this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Controls.Add(this.pnlScreen, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.btnUpdate, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(460, 338);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // btnUpdate
      // 
      this.btnUpdate.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.btnUpdate.Location = new System.Drawing.Point(192, 311);
      this.btnUpdate.Name = "btnUpdate";
      this.btnUpdate.Size = new System.Drawing.Size(75, 23);
      this.btnUpdate.TabIndex = 1;
      this.btnUpdate.Text = "Run";
      this.btnUpdate.UseVisualStyleBackColor = true;
      this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
      // 
      // pnlScreen
      // 
      this.pnlScreen.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.pnlScreen.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pnlScreen.Location = new System.Drawing.Point(3, 3);
      this.pnlScreen.Name = "pnlScreen";
      this.pnlScreen.Size = new System.Drawing.Size(454, 302);
      this.pnlScreen.TabIndex = 0;
      this.pnlScreen.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlScreen_Paint);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(460, 338);
      this.Controls.Add(this.tableLayoutPanel1);
      this.DoubleBuffered = true;
      this.KeyPreview = true;
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Form1";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Timer timerUpdate;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.PanelEx pnlScreen;
    private System.Windows.Forms.Button btnUpdate;
  }
}

