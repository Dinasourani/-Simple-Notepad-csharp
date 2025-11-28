using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
    
{
    
    public partial class Form1 : Form
    {
        string currentFilePath;
        bool isTextChanged = true;
        string lastSearch = "";    // متن آخرین جستجو
        int lastIndex = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void NewFile(object sender, EventArgs e)
        {
            textBox1.Clear();
             currentFilePath = string.Empty;
            this.Text = "Notpad+ - New File";
        }
        private void newwindow(object sender, EventArgs e)
        {
            Form1 newform = new Form1();
            newform.Show();
        }

        private void openfile(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Text File(*.txt)|*.txt|All Files(*.*)|*.*";
            of.Title = "Open";
            if (of.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = System.IO.File.ReadAllText(of.FileName);
                //ذخیره مسیر فایل باز شده
                currentFilePath = of.FileName;
            }
        
        }
        private void save(object sender, EventArgs e)
        {
            // اگر فایل قبلاً باز یا ذخیره شده باشد
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                System.IO.File.WriteAllText(currentFilePath, textBox1.Text);
                return;
            }

            // اگر فایل جدید است → رفتار Save As
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            sfd.Title = "Save As";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(sfd.FileName, textBox1.Text);
                currentFilePath = sfd.FileName; // ذخیره مسیر فایل
            }
        }
        private void saveas(object sender, EventArgs e)
        {
            
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            sfd.Title = "Save As";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(sfd.FileName, textBox1.Text);
                currentFilePath = sfd.FileName; // ذخیره مسیر فایل
            }
        }
        private void pageSetup(object sender, EventArgs e)
        {
            PageSetupDialog psd = new PageSetupDialog();
            psd.Document = printDocument1;        // اتصال به PrintDocument
            psd.AllowMargins = true;
            psd.AllowOrientation = true;
            psd.AllowPaper = true;
            psd.AllowPrinter = true;

            psd.ShowDialog();
        }
        private void print(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            pd.Document = printDocument1;

            if (pd.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void exit(object sender, EventArgs e)
        {
            if (isTextChanged)
            {
                var result = MessageBox.Show(
                    "Save changes?", "Notepad",
                    MessageBoxButtons.YesNoCancel
                );

                if (result == DialogResult.Yes)
                {
                    if (!string.IsNullOrEmpty(currentFilePath))
                        System.IO.File.WriteAllText(currentFilePath, textBox1.Text);
                    else
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        if (sfd.ShowDialog() == DialogResult.OK)
                            System.IO.File.WriteAllText(sfd.FileName, textBox1.Text);
                        else return;
                    }
                }
                else if (result == DialogResult.Cancel) return;
            }

            this.Close();
        }
   
        private void undo(object sender, EventArgs e)
        {
            if (textBox1.CanUndo)
                textBox1.Undo();
        }

        private void cut(object sender, EventArgs e)
        {
            if (textBox1.SelectedText.Length > 0)
                textBox1.Cut();
        }
        private void copy(object sender, EventArgs e)
        {
            if (textBox1.SelectedText.Length > 0)
                textBox1.Copy();
        }
        private void paste(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
                textBox1.Paste();
        }
        private void delete(object sender, EventArgs e)
        {
            if (textBox1.SelectedText.Length > 0)
                textBox1.SelectedText = "";
        }
        private void searchWithBing(object sender, EventArgs e)
        {
            string query = textBox1.SelectedText.Trim();

            if (!string.IsNullOrEmpty(query))
            {
                string url = "https://www.bing.com/search?q=" + Uri.EscapeDataString(query);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Please select text to search.", "Search with Bing");
            }
        }
      
        private void find(object sender, EventArgs e)
        {
        // پنجره کوچک برای گرفتن متن جستجو
        Form inputForm = new Form() { Width = 300, Height = 120, FormBorderStyle = FormBorderStyle.FixedDialog, Text = "Find", StartPosition = FormStartPosition.CenterParent };
        TextBox tb = new TextBox() { Left = 10, Top = 10, Width = 260 };
        Button ok = new Button() { Text = "OK", Left = 60, Width = 80, Top = 40, DialogResult = DialogResult.OK };
        Button cancel = new Button() { Text = "Cancel", Left = 150, Width = 80, Top = 40, DialogResult = DialogResult.Cancel };

        inputForm.Controls.Add(tb);
        inputForm.Controls.Add(ok);
        inputForm.Controls.Add(cancel);
        inputForm.AcceptButton = ok;
        inputForm.CancelButton = cancel;

        if (inputForm.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(tb.Text))
        {
            string searchText = tb.Text;
            int start = textBox1.SelectionStart + textBox1.SelectionLength;
            int index = textBox1.Text.IndexOf(searchText, start, StringComparison.CurrentCultureIgnoreCase);
            if (index < 0)
                index = textBox1.Text.IndexOf(searchText, 0, StringComparison.CurrentCultureIgnoreCase);

        if (index >= 0)
        {
            textBox1.Select(index, searchText.Length);
            textBox1.ScrollToCaret();
            textBox1.Focus();
        }
        else
        {
            MessageBox.Show("Text not found.", "Find");
        }
      }

        }
            private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
            {
                // فرم کوچک برای گرفتن متن قدیمی و جدید
                Form frm = new Form() { Width = 300, Height = 160, FormBorderStyle = FormBorderStyle.FixedDialog, Text = "Replace", StartPosition = FormStartPosition.CenterParent };

                Label lblFind = new Label() { Text = "Find:", Left = 10, Top = 10, Width = 50 };
                TextBox tbFind = new TextBox() { Left = 70, Top = 10, Width = 200 };

                Label lblReplace = new Label() { Text = "Replace:", Left = 10, Top = 40, Width = 60 };
                TextBox tbReplace = new TextBox() { Left = 70, Top = 40, Width = 200 };

                Button btnReplace = new Button() { Text = "Replace", Left = 50, Top = 70, Width = 80, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Cancel", Left = 150, Top = 70, Width = 80, DialogResult = DialogResult.Cancel };

                frm.Controls.Add(lblFind); frm.Controls.Add(tbFind);
                frm.Controls.Add(lblReplace); frm.Controls.Add(tbReplace);
                frm.Controls.Add(btnReplace); frm.Controls.Add(btnCancel);

                frm.AcceptButton = btnReplace;
                frm.CancelButton = btnCancel;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string findText = tbFind.Text;
                    string replaceText = tbReplace.Text;

                    if (!string.IsNullOrEmpty(findText))
                    {
                        textBox1.Text = textBox1.Text.Replace(findText, replaceText);
                    }
                }
            }
           private void replace(object sender, EventArgs e)
            {
            // فرم کوچک برای گرفتن متن قدیمی و جدید
            Form frm = new Form() { Width = 300, Height = 160, FormBorderStyle = FormBorderStyle.FixedDialog, Text = "Replace", StartPosition = FormStartPosition.CenterParent };
    
            Label lblFind = new Label() { Text = "Find:", Left = 10, Top = 10, Width = 50 };
            TextBox tbFind = new TextBox() { Left = 70, Top = 10, Width = 200 };
    
            Label lblReplace = new Label() { Text = "Replace:", Left = 10, Top = 40, Width = 60 };
            TextBox tbReplace = new TextBox() { Left = 70, Top = 40, Width = 200 };
    
            Button btnReplace = new Button() { Text = "Replace", Left = 50, Top = 70, Width = 80, DialogResult = DialogResult.OK };
            Button btnCancel = new Button() { Text = "Cancel", Left = 150, Top = 70, Width = 80, DialogResult = DialogResult.Cancel };
    
            frm.Controls.Add(lblFind); frm.Controls.Add(tbFind);
            frm.Controls.Add(lblReplace); frm.Controls.Add(tbReplace);
            frm.Controls.Add(btnReplace); frm.Controls.Add(btnCancel);
    
            frm.AcceptButton = btnReplace;
            frm.CancelButton = btnCancel;

            if (frm.ShowDialog() == DialogResult.OK)
        {
            string findText = tbFind.Text;
            string replaceText = tbReplace.Text;

        if (!string.IsNullOrEmpty(findText))
        {
            textBox1.Text = textBox1.Text.Replace(findText, replaceText);
        }
    }
}
           

            private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
            {
                if (string.IsNullOrEmpty(lastSearch))
                {
                    MessageBox.Show("Please use Find first.", "Find Next");
                    return;
                }

                int start = lastIndex + lastSearch.Length;
                int index = textBox1.Text.IndexOf(lastSearch, start, StringComparison.CurrentCultureIgnoreCase);

                if (index < 0)
                {
                    index = textBox1.Text.IndexOf(lastSearch, 0, StringComparison.CurrentCultureIgnoreCase);
                }

                if (index >= 0)
                {
                    textBox1.Select(index, lastSearch.Length);
                    textBox1.ScrollToCaret();
                    textBox1.Focus();
                    lastIndex = index;
                }
                else
                {
                    MessageBox.Show("Text not found.", "Find Next");
                }
            }
            private void selectAll(object sender, EventArgs e)
            {
                textBox1.SelectAll();
                textBox1.Focus();
            }
            private void dateTime(object sender, EventArgs e)
            {
                textBox1.SelectedText = DateTime.Now.ToString("HH:mm dd/MM/yyyy");
            }
            private void wordwrap(object sender, EventArgs e)
            {
                textBox1.WordWrap = !textBox1.WordWrap;

            }
            private void font(object sender, EventArgs e)
            {
                FontDialog fd = new FontDialog();
                fd.Font = textBox1.Font;   // فونت فعلی متن

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Font = fd.Font;   // اعمال فونت انتخاب شده
                }
            }
            private void zoomIn(object sender, EventArgs e)
            {
                textBox1.Font = new Font(textBox1.Font.FontFamily, textBox1.Font.Size + 2);
            }

            private void zoomout(object sender, EventArgs e)
            {
                float newSize = textBox1.Font.Size - 2;
                if (newSize >= 6) // حداقل اندازه فونت
                    textBox1.Font = new Font(textBox1.Font.FontFamily, newSize);
            }

            private void restoreDefaultZoom(object sender, EventArgs e)
            {
                float defaultFontSize = 12; // یا هر اندازه اولیه‌ای که متن دارد
                textBox1.Font = new Font(textBox1.Font.FontFamily, defaultFontSize);
            }
        private void textBox1_SelectionChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Notepad";
            this.Icon = new Icon("icon.ico");

            if (textBox1.SelectedText.Length < 0)
            {
                undoToolStripMenuItem.Visible = false;
            }
        }
    }
}
