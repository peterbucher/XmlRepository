namespace WindowsFormsMappingTestApp
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
            if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtDefault = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listBoxMappings = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSaveUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPropertyAlias = new System.Windows.Forms.TextBox();
            this.ddlMappingType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ddlPropertyName = new System.Windows.Forms.ComboBox();
            this.txtMapped = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtDefault
            // 
            this.txtDefault.Location = new System.Drawing.Point(11, 186);
            this.txtDefault.Multiline = true;
            this.txtDefault.Name = "txtDefault";
            this.txtDefault.Size = new System.Drawing.Size(598, 561);
            this.txtDefault.TabIndex = 0;
            this.txtDefault.Text = resources.GetString("txtDefault.Text");
            this.txtDefault.TextChanged += new System.EventHandler(this.txtDefault_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 157);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(688, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "ConvertToRight";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBoxMappings
            // 
            this.listBoxMappings.FormattingEnabled = true;
            this.listBoxMappings.Location = new System.Drawing.Point(14, 35);
            this.listBoxMappings.Name = "listBoxMappings";
            this.listBoxMappings.Size = new System.Drawing.Size(355, 108);
            this.listBoxMappings.TabIndex = 3;
            this.listBoxMappings.SelectedIndexChanged += new System.EventHandler(this.listBoxMappings_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Mappings";
            // 
            // btnSaveUpdate
            // 
            this.btnSaveUpdate.Location = new System.Drawing.Point(390, 120);
            this.btnSaveUpdate.Name = "btnSaveUpdate";
            this.btnSaveUpdate.Size = new System.Drawing.Size(114, 23);
            this.btnSaveUpdate.TabIndex = 5;
            this.btnSaveUpdate.Text = "Update / Save";
            this.btnSaveUpdate.UseVisualStyleBackColor = true;
            this.btnSaveUpdate.Click += new System.EventHandler(this.btnSaveUpdate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(510, 120);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(387, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "PropertyName";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(387, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "PropertyAlias";
            // 
            // txtPropertyAlias
            // 
            this.txtPropertyAlias.Location = new System.Drawing.Point(473, 59);
            this.txtPropertyAlias.Name = "txtPropertyAlias";
            this.txtPropertyAlias.Size = new System.Drawing.Size(236, 20);
            this.txtPropertyAlias.TabIndex = 10;
            this.txtPropertyAlias.TextChanged += new System.EventHandler(this.txtPropertyAlias_TextChanged);
            // 
            // ddlMappingType
            // 
            this.ddlMappingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlMappingType.FormattingEnabled = true;
            this.ddlMappingType.Location = new System.Drawing.Point(473, 86);
            this.ddlMappingType.Name = "ddlMappingType";
            this.ddlMappingType.Size = new System.Drawing.Size(236, 21);
            this.ddlMappingType.TabIndex = 11;
            this.ddlMappingType.SelectedIndexChanged += new System.EventHandler(this.ddlMappingType_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(390, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "MappingType";
            // 
            // ddlPropertyName
            // 
            this.ddlPropertyName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlPropertyName.FormattingEnabled = true;
            this.ddlPropertyName.Location = new System.Drawing.Point(473, 32);
            this.ddlPropertyName.Name = "ddlPropertyName";
            this.ddlPropertyName.Size = new System.Drawing.Size(236, 21);
            this.ddlPropertyName.TabIndex = 16;
            this.ddlPropertyName.SelectedIndexChanged += new System.EventHandler(this.ddlPropertyName_SelectedIndexChanged);
            // 
            // txtMapped
            // 
            this.txtMapped.Location = new System.Drawing.Point(615, 186);
            this.txtMapped.Multiline = true;
            this.txtMapped.Name = "txtMapped";
            this.txtMapped.Size = new System.Drawing.Size(660, 561);
            this.txtMapped.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1282, 757);
            this.Controls.Add(this.ddlPropertyName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ddlMappingType);
            this.Controls.Add(this.txtPropertyAlias);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSaveUpdate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxMappings);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtMapped);
            this.Controls.Add(this.txtDefault);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDefault;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBoxMappings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSaveUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPropertyAlias;
        private System.Windows.Forms.ComboBox ddlMappingType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ddlPropertyName;
        private System.Windows.Forms.TextBox txtMapped;
    }
}

