using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Symlink_Creator
{
    ///<summary>
    /// This class manages the window
    ///</summary>
    public partial class MainWindow : Form
    {

        private bool _folder;
        private readonly ToolTip _tip = new ToolTip();


        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _tip.IsBalloon = true;
            comboBox1.SelectedIndex = 0;
            TypeSelector.SelectedIndex = 0;
        }

        #region Events

        // Manages the action of the "info" image
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("© 2010 Alejandro Mora Díaz \n Version: 1.1.0.5 \n e-mail: amora@plexip.com \n Thanks to Microsoft for the use of their shortcut arrow :)", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }




        // Manages the link explore button
        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowDialog();
            textBox1.Text = folderBrowser.SelectedPath;
        }

        // Manages the explore button
        private void button2_Click(object sender, EventArgs e)
        {
            // if _folder is true the folder browser will be shown
            if (_folder)
            {
                folderBrowser.ShowDialog();
                textBox3.Text = folderBrowser.SelectedPath;
            }
            else
            {
                filesBrowser.ShowDialog();
                textBox3.Text = filesBrowser.FileName;
            }
        }

        // manages the type selector combobox
        private void TypeSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            Switcher();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CreateLink();
        }


        private void TypeSelector_MouseHover(object sender, EventArgs e)
        {
            _tip.ToolTipIcon = ToolTipIcon.Info;
            _tip.UseAnimation = true;
            _tip.UseFading = true;
            _tip.AutoPopDelay = 10000;
            _tip.ToolTipTitle = "Symbolic Link type selector";
            _tip.SetToolTip(TypeSelector, "With this option you can choose between creating file symbolic links; \nthis is using a file to point to another file, or folder symbolic links; this \nis using folders that point to other folders");
        }

        private void comboBox1_MouseHover(object sender, EventArgs e)
        {
            _tip.ToolTipIcon = ToolTipIcon.Info;
            _tip.UseAnimation = true;
            _tip.UseFading = true;
            _tip.AutoPopDelay = 5000;
            _tip.ToolTipTitle = "Symbolic Link types";
            _tip.SetToolTip(comboBox1, "This option allows you to select the style of your symbolic link, either\nyou choose to use symbolic links, hard links or directory junctions");
        }
        

        #endregion


        /// <summary>
        /// Creates the link if the conditions are met
        /// </summary>
        private void CreateLink()
        {
            if (textBox1.Text != "" && textBox3.Text != "" && textBox3.Text != "")// Everything need to be filled...
                if (_folder && Directory.Exists(textBox1.Text) && Directory.Exists(textBox3.Text)) // Ask if the folders exist
                {
                    var link = "\"" + textBox1.Text + "\\" + textBox2.Text + "\" "; // concatenates the link name with the folder name and then it adds a pair of ", to allow using directories with spaces..

                    var directories = Directory.GetDirectories(textBox1.Text); // gets the folders in the selected directory
                    if (directories.Any(e => e.Split('\\').Last().Equals(textBox2.Text))) // looks for folders with the same name of the link name
                    {
                        // if found the program ask the user if he wants to delete the folder that is already there
                        var answer = MessageBox.Show("The link name you are using already exists in the selected directory, would you like to DELETE the folder and then create a new link?", "Folder already there...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (answer == DialogResult.Yes)
                        {
                            // if the answer is yes, the folder is deleted in order to create a new one
                            var dir2Delete = directories.First(e => e.Split('\\').Last().Equals(textBox2.Text));
                            Directory.Delete(dir2Delete);
                            SendCommand(link);
                            return;
                        }
                        MessageBox.Show("Link creation aborted", "Aborted Operation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    SendCommand(link);
                }
                else if (Directory.Exists(textBox1.Text) && File.Exists(textBox3.Text))
                {
                    // same thing as above... it just deletes files instead of folders
                    var link = "\"" + textBox1.Text + "\\" + textBox2.Text + "\" ";

                    var files = Directory.GetFiles(textBox1.Text);
                    if (files.Any(e => e.Split('\\').Last().Equals(textBox2.Text)))
                    {
                        var answer = MessageBox.Show("The link name you are using already exists in the selected directory, would you like to DELETE the file and then create a new link?", "File already there...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (answer == DialogResult.Yes)
                        {
                            var file2Delete = files.First(e => e.Split('\\').Last().Equals(textBox2.Text));
                            File.Delete(file2Delete);
                            SendCommand(link);
                            return;
                        }
                        MessageBox.Show("Link creation aborted", "Aborted Operation", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    SendCommand(link);
                }
                else
                    MessageBox.Show("One of the directories/files does not exists, please provide valid directories/files", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Please fill all the required info", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// This method allows to switch modes between file or folder symlinks
        /// </summary>
        private void Switcher()
        {
            if (TypeSelector.SelectedIndex == 0)
            {
                groupBox1.Text = "Link Folder";
                groupBox2.Text = "Destination Folder";
                label3.Text = "Please select the path to the real folder you want to link:";
                _folder = true;
            }
            else
            {
                groupBox1.Text = "Link File";
                groupBox2.Text = "Destination File";
                label3.Text = "Please select the path to the real file you want to link:";
                _folder = false;
            }
        }

        /// <summary>
        /// This method build a string using the paramethers provided by the user, after that, it start a new
        /// cmd.exe process with the string just built.
        /// </summary>
        /// <param name="link"></param>
        public void SendCommand(string link)
        {
            var target = "\"" + textBox3.Text + "\""; // concatenates a pair of "", to provide
            var typeLink = ComboBoxSelection();
            var directory = _folder ? "/D " : "";
            var stringCommand = "/c mklink " + directory + typeLink + link + target;
            Process.Start("cmd", stringCommand);
            MessageBox.Show("Link successfully created", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        /// <summary>
        /// This Method lets you select the type of link you want to create
        /// </summary>
        /// <returns>String with the type of link to create</returns>
        private string ComboBoxSelection()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 1:
                    return "/H ";
                case 2:
                    return "/J ";
                default:
                    return "";
            }
        }


    }
}
