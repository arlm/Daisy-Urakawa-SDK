﻿using System;
using System.Windows.Forms;
using DTbookToXuk;
using urakawa;
using urakawa.xuk;

namespace DTbookToXukUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            txtBookName.Clear();
            var open = new OpenFileDialog();
            //open.InitialDirectory = @"C:\";
            open.Filter = "XML Files (*.xml)|*.xml|All files(*.*)|*.*";
            open.FilterIndex = 1;
            open.RestoreDirectory = true;
            if (open.ShowDialog(this) == DialogResult.OK)
            {
                //m_DTBook_FilePath = open.FileName;
                // txtBookName.Text = m_DTBook_FilePath;
                //uriDTBook = new Uri(m_DTBook_FilePath);
                txtBookName.Text = open.FileName;
                var uriDTBook = new Uri(txtBookName.Text);
                DTBooktoXukConversion converter = new DTBooktoXukConversion(uriDTBook);

                ////

                Uri uriComp = new Uri(txtBookName.Text + ".COMPRESSED.xuk");

                {
                    SaveXukAction actionSave = new SaveXukAction(converter.Project, uriComp);
                    bool saveWasCancelled;
                    Progress.ExecuteProgressAction(actionSave, out saveWasCancelled);
                    if (saveWasCancelled)
                    {
                        return;
                    }
                }

                /////

                Uri uriPretty = new Uri(txtBookName.Text + ".PRETTY.xuk");

                converter.Project.SetPrettyFormat(true);

                {
                    SaveXukAction actionSave = new SaveXukAction(converter.Project, uriPretty);
                    bool saveWasCancelled;
                    Progress.ExecuteProgressAction(actionSave, out saveWasCancelled);
                    if (saveWasCancelled)
                    {
                        return;
                    }
                }

                /////
                //// Make sure we don't create concurrent access to WAV files while opening the same XUK file in several projects.
                converter.Project.GetPresentation(0).DataProviderManager.CompareByteStreamsDuringValueEqual = false;
                /////
                Project projectPretty = new Project();
                {
                    OpenXukAction actionOpen = new OpenXukAction(projectPretty, uriPretty);
                    bool openWasCancelled;
                    Progress.ExecuteProgressAction(actionOpen, out openWasCancelled);
                    if (openWasCancelled)
                    {
                        return;
                    }
                }
                projectPretty.GetPresentation(0).DataProviderManager.CompareByteStreamsDuringValueEqual = false;
                System.Diagnostics.Debug.Assert(converter.Project.ValueEquals(projectPretty));
                /////
                /////
                Project projectComp = new Project();
                {
                    OpenXukAction actionOpen = new OpenXukAction(projectComp, uriComp);
                    bool openWasCancelled;
                    Progress.ExecuteProgressAction(actionOpen, out openWasCancelled);
                    if (openWasCancelled)
                    {
                        return;
                    }
                }
                projectComp.GetPresentation(0).DataProviderManager.CompareByteStreamsDuringValueEqual = false;
                System.Diagnostics.Debug.Assert(converter.Project.ValueEquals(projectComp));
                /////
                /// //// Make sure we don't create concurrent access to WAV files while opening the same XUK file in several projects.
                System.Diagnostics.Debug.Assert(projectComp.ValueEquals(projectPretty));
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtBookName.Clear();
        }
       
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
