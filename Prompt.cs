using System.Windows.Forms;

namespace GraphsClassProjectTakeTwo
{
    /// <summary>
    /// Static class making a Dialog with user input
    /// </summary>
    public static class Prompt
    {
        /// <summary>
        /// Shows the Dialog with user input
        /// </summary>
        /// <param name="text">The text of the Dialog</param>
        /// <param name="caption">The caption of the Dialog</param>
        /// <returns></returns>
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text, Width = 200 };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 200 };
            Button confirmation = new Button() { Text = "Ok", Left = 50, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
