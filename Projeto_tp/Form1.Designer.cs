namespace Projeto_tp
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
            textBox_msg = new TextBox();
            label1 = new Label();
            btn_enviar = new Button();
            listBox1 = new ListBox();
            label2 = new Label();
            btn_limpar = new Button();
            nameLabel = new Label();
            SuspendLayout();
            // 
            // textBox_msg
            // 
            textBox_msg.Location = new Point(73, 34);
            textBox_msg.Margin = new Padding(3, 2, 3, 2);
            textBox_msg.Name = "textBox_msg";
            textBox_msg.Size = new Size(226, 23);
            textBox_msg.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(73, 16);
            label1.Name = "label1";
            label1.Size = new Size(205, 15);
            label1.TabIndex = 1;
            label1.Text = "Envie uma mensagem para o servidor";
            // 
            // btn_enviar
            // 
            btn_enviar.Location = new Point(73, 68);
            btn_enviar.Margin = new Padding(3, 2, 3, 2);
            btn_enviar.Name = "btn_enviar";
            btn_enviar.Size = new Size(226, 22);
            btn_enviar.TabIndex = 2;
            btn_enviar.Text = "Enviar";
            btn_enviar.UseVisualStyleBackColor = true;
            btn_enviar.Click += btn_enviar_Click;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(73, 104);
            listBox1.Margin = new Padding(3, 2, 3, 2);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(226, 139);
            listBox1.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(458, 634);
            label2.Name = "label2";
            label2.Size = new Size(135, 15);
            label2.TabIndex = 5;
            label2.Text = "Escreva o seu username.";
            // 
            // btn_limpar
            // 
            btn_limpar.Location = new Point(73, 252);
            btn_limpar.Margin = new Padding(3, 2, 3, 2);
            btn_limpar.Name = "btn_limpar";
            btn_limpar.Size = new Size(226, 22);
            btn_limpar.TabIndex = 6;
            btn_limpar.Text = "Limpar Dados";
            btn_limpar.UseVisualStyleBackColor = true;
            btn_limpar.Click += btn_limpar_Click;
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Location = new Point(307, 17);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(0, 15);
            nameLabel.TabIndex = 8;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 338);
            Controls.Add(nameLabel);
            Controls.Add(btn_limpar);
            Controls.Add(label2);
            Controls.Add(listBox1);
            Controls.Add(btn_enviar);
            Controls.Add(label1);
            Controls.Add(textBox_msg);
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox_msg;
        private Label label1;
        private Button btn_enviar;
        private ListBox listBox1;
        private TextBox textBox1;
        private TextBox TextBox_username;
        private Label label2;
        private Button btn_limpar;
        private Label nameLabel;
    }
}
