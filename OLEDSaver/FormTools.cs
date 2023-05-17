using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OLEDSaver
{
    public static class FormTools
    {
        public static void ShowSelectableMessageBox(string message, string title, string additionalMessage)
        {
            int formWidth = 600;
            int formHeight = 225;

            Label label = new Label
            {
                Text = additionalMessage,

                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                ForeColor = Color.FromArgb(255, 255, 255),
                Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
                Left = 5, 
                Top = 5,
                Width = formWidth - 10,
                Height = 25,
            };

            TextBox textBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Text = message,
                ScrollBars = ScrollBars.Horizontal,
                //Dock = DockStyle.Fill,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                WordWrap = false,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.FromArgb(255, 255, 255),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                Left = 5,
                Top = 45,
                Width = formWidth - 45,
                Height = formHeight - 160,
            };
            textBox.SelectionStart = textBox.Text.Length;

            Button okButton = new Button
            {
                Text = "OK",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.OK,
                ForeColor = Color.FromArgb(255, 255, 255),
                BackColor = Color.FromArgb(60, 60, 60),
                Left = textBox.Right - 50,
                Top = textBox.Bottom + 15,
                Width = 50,
                Height = 25,
            };

            Form messageBoxForm = new Form
            {
                Text = title,
                StartPosition = FormStartPosition.CenterScreen,
                Width = formWidth,
                Height = formHeight,
                MinimumSize = new Size(formWidth, formHeight),
                BackColor = Color.FromArgb(30, 30, 30),
                AcceptButton = okButton // This will allow the Enter key to trigger the button
            };

            messageBoxForm.Controls.Add(textBox);
            messageBoxForm.Controls.Add(label);
            messageBoxForm.Controls.Add(okButton);

            messageBoxForm.ShowDialog();
            messageBoxForm.TopMost = true;
            messageBoxForm.TopLevel = true;
        }
    }
}
