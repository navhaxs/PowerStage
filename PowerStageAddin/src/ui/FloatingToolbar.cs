using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using PowerStage.Models;

namespace PowerStageAddin.ui
{
    public partial class FloatingToolbar: Form
    {
        public FloatingToolbar()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Messenger.Default.Send(new StageMsgMessage
            {
                Msg = textBox1.Text,
                Visibility = checkBox1.Checked
            }
            );
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            App.ui2s = new ui.overlay();
            App.ui2s.DoFreeze();
            App.ui2s.Show();
        }

        private void FloatingToolbar_Load(object sender, EventArgs e)
        {

            var wpfwindow = new PowerStage.StageDisplayWindow();
            ElementHost.EnableModelessKeyboardInterop(wpfwindow);
            wpfwindow.Show();
        }
    }
}
