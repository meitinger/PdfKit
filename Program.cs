/* Copyright (C) 2016, Manuel Meitinger
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Aufbauwerk.Tools.PdfKit
{
    class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetShortPathName(string path, StringBuilder shortPath, int shortPathLength);

        internal static string GetShortPathName(string path)
        {
            // return the 8.3 version of the path
            if (path == null)
                throw new ArgumentNullException("path");
            var buffer = new StringBuilder(300);
            int len;
            while ((len = GetShortPathName(path, buffer, buffer.Capacity)) > buffer.Capacity)
                buffer.EnsureCapacity(len);
            if (len == 0)
                throw new Win32Exception();
            return buffer.ToString();
        }

        [STAThread]
        static void Main(string[] args)
        {
            Application.SetCompatibleTextRenderingDefault(true);
            Application.EnableVisualStyles();
            Application.Run(new ExtractForm(@"C:\Users\m.meitinger\Downloads\einkommensteuer2014.pdf"));
        }
    }
}
