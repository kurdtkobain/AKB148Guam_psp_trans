#region copyright
// <copyright file="Program.cs" company="Kurdtkobain">
// Copyright (c) 2015-2017 All Rights Reserved
// </copyright>
// <author>Kurdtkobain</author>
// <date>2015/9/19 7:03:28 AM </date>
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoryTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
