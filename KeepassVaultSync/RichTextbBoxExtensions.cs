using System.Drawing;
using System.Windows.Forms;

namespace KeepassVaultSync
{
    // https://stackoverflow.com/a/1926822
    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
        
    }
}