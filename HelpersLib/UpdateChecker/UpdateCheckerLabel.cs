﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System.Threading;
using System.Windows.Forms;

namespace HelpersLib
{
    public delegate UpdateChecker CheckUpdate();

    public partial class UpdateCheckerLabel : UserControl
    {
        private UpdateChecker updateChecker;
        private bool isBusy;

        public UpdateCheckerLabel()
        {
            InitializeComponent();
        }

        public void CheckUpdate(CheckUpdate checkUpdate)
        {
            if (!isBusy)
            {
                isBusy = true;

                lblStatus.Visible = false;
                llblUpdateAvailable.Visible = false;

                pbLoading.Visible = true;
                lblCheckingUpdates.Visible = true;

                new Thread(() => CheckingUpdate(checkUpdate)).Start();
            }
        }

        private void CheckingUpdate(CheckUpdate checkUpdate)
        {
            updateChecker = checkUpdate();
            UpdateControls();
            isBusy = false;
        }

        private void UpdateControls()
        {
            this.InvokeSafe(() =>
            {
                pbLoading.Visible = false;
                lblCheckingUpdates.Visible = false;

                switch (updateChecker.UpdateInfo.Status)
                {
                    case UpdateStatus.UpdateCheckFailed:
                        lblStatus.Text = "Update check failed";
                        lblStatus.Visible = true;
                        break;
                    case UpdateStatus.UpdateAvailable:
                        llblUpdateAvailable.Text = "A newer version of ShareX is available";
                        llblUpdateAvailable.Visible = true;
                        break;
                    case UpdateStatus.UpToDate:
                        lblStatus.Text = "ShareX is up to date";
                        lblStatus.Visible = true;
                        break;
                }
            });
        }

        private void llblUpdateAvailable_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (updateChecker != null && updateChecker.UpdateInfo != null && updateChecker.UpdateInfo.Status == UpdateStatus.UpdateAvailable)
            {
                UpdaterForm updaterForm = new UpdaterForm(updateChecker);
                updaterForm.ShowDialog();

                if (updaterForm.Status == DownloaderFormStatus.InstallStarted)
                {
                    Application.Exit();
                }
            }
        }
    }
}