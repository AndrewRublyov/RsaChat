using System.Drawing;
using System.Windows.Forms;

namespace RsaChat.App
{
    public static class RichTextBoxExtensions
    {
        public static void AppendError(this RichTextBox box, string message)
        {
            box.AppendText($"{message}\n", Color.DarkRed, true);
        }

        public static void AppendInfo(this RichTextBox box, string message)
        {
            box.AppendText($"{message}\n", Color.DarkGray);
        }

        public static void AppendMessage(this RichTextBox box, string from, string message)
        {
            box.AppendText($"{from}: ", Color.DarkBlue, true);
            box.AppendText($"{message}\n", Color.Black, false);
        }

        public static void AppendText(this RichTextBox box, string text, Color color, bool bold = false)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionFont = bold ? new Font(box.SelectionFont, FontStyle.Bold) : box.SelectionFont;
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            box.SelectionFont = box.Font;
        }
    }
}
