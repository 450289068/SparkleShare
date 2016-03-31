//   SparkleShare, a collaboration and sharing tool.
//   Copyright (C) 2010  Hylke Bons <hylkebons@gmail.com>
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//   GNU General Public License for more details.
//
//   You should have received a copy of the GNU General Public License
//   along with this program. If not, see <http://www.gnu.org/licenses/>.


using System;
using System.Diagnostics;
using System.IO;

using Gtk;
using Mono.Unix.Native;

using Sparkles;

namespace SparkleShare {

    public class Controller : BaseController {

        public Controller ()
        {
        }

        public override bool CreateSparkleShareFolder ()
        {
            if (Directory.Exists (Configuration.DefaultConfig.FoldersPath))
                return false;
            
            Directory.CreateDirectory (Configuration.DefaultConfig.FoldersPath);
            Syscall.chmod (Configuration.DefaultConfig.FoldersPath, (FilePermissions) 448); // 448 -> 700

            return false;
        }
        

        public override void SetFolderIcon ()
        {
            var command = new Command ("gvfs-set-attribute", Configuration.DefaultConfig.FoldersPath + " " +
                "metadata::custom-icon-name org.sparkleshare.SparkleShare");

            command.StartAndWaitForExit ();
        }


        public override void CreateStartupItem ()
        {
            string autostart_file_path = Path.Combine (Path.GetDirectoryName (InstallationInfo.Directory),
                "applications", "SparkleShare.Autostart.desktop");
            
            string autostart_file_dest = Path.Combine (Config.HomePath, ".config", "autostart", "SparkleShare.Autostart.desktop");
			string autostart_path = Path.GetDirectoryName (autostart_file_dest);

			if (!Directory.Exists (autostart_path))
                Directory.CreateDirectory (autostart_path);

            try {
                File.Copy (autostart_file_path, autostart_file_dest);
                Logger.LogInfo ("Controller", "Added SparkleShare to startup items");

            } catch (Exception e) {
                Logger.LogInfo ("Controller", "Failed to add SparkleShare to startup items", e);
            }
        }


        public override void InstallProtocolHandler ()
        {
        }
        

		public override void CopyToClipboard (string text)
        {
			Clipboard.Get (Gdk.Atom.Intern ("CLIPBOARD", false)).Text = text;
        }


        public override void OpenFolder (string path)
        {
            OpenFile (path);
        }


        public override void OpenFile (string path)
        {
            new Command ("xdg-open", "\"" + path + "\"").Start ();
        }


        public override string EventLogHTML {
            get {
                string html_path = Path.Combine (InstallationInfo.Directory, "html", "event-log.html");
                string jquery_file_path = Path.Combine (InstallationInfo.Directory, "html", "jquery.js");

                string html   = File.ReadAllText (html_path);
                string jquery = File.ReadAllText (jquery_file_path);

                return html.Replace ("<!-- $jquery -->", jquery);
            }
        }

        
        public override string DayEntryHTML {
            get {
                string path = Path.Combine (InstallationInfo.Directory, "html", "day-entry.html");
                return File.ReadAllText (path);
            }
        }

        
        public override string EventEntryHTML {
            get {
                string path = Path.Combine (InstallationInfo.Directory, "html", "event-entry.html");
                return File.ReadAllText (path);
            }
        }


        public override string PresetsPath {
            get {
                return Path.Combine (InstallationInfo.Directory, "presets");
            }
        }
    }
}