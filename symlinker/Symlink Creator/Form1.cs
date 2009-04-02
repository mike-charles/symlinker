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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }





        #region Events
        // Manages the action of the "info" image
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("© 2009 Alejandro Mora Díaz", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Manages the action of the "whats this?" label
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://technet.microsoft.com/en-us/library/cc753194.aspx");
        }

        // Manages the link explore button
        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowDialog();
            textBox1.Text = folderBrowser.SelectedPath;
        }
        // Manages the destination explore button
        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowDialog();
            textBox3.Text = folderBrowser.SelectedPath;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            createLink();
        }


        /// <summary>
        /// Creates the link if the conditions are met
        /// </summary>
        private void createLink()
        {
            if (textBox1.Text != "" && textBox3.Text != "" && textBox3.Text != "") // Everything need to be filled...
                if (Directory.Exists(textBox1.Text) || Directory.Exists(textBox3.Text)) // Ask if the directories exists
                {
                    string link = "\"" + textBox1.Text + "\\" + textBox2.Text + "\" ";
                    string target = "\"" + textBox3.Text + "\"";
                    string typeLink = comboBoxSelection();
                    string stringCommand =  "/c mklink " + typeLink + link + target;
                    Process.Start("cmd", stringCommand);
                    MessageBox.Show("Link successfully created", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("One of the directories does not exists, please provide valid directories", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Please fill all the required info", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        #endregion

        /// <summary>
        /// This Method lets you select the type of link you want to create
        /// </summary>
        /// <returns>String with the type of link to create</returns>
        private string comboBoxSelection()
        {
            if (comboBox1.SelectedIndex == 0)
                return "/D ";
            else if (comboBox1.SelectedIndex == 1)
                return "/H ";
            else
                return "/J ";
        }

    }
}
