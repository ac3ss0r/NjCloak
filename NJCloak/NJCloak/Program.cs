using NJCloak.Modules;
using System;
using System.Threading;
using System.Windows.Forms;

namespace NJCloak {
    static class Program {

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainException;
            Application.Run(new MainForm());
        }

        // A worker thread fault must not take the whole application down with it, otherwise the
        // screen blocker and the firewall policy would be left in whatever state they were in.
        private static void OnThreadException(object sender, ThreadExceptionEventArgs e) {
            Report(e.Exception, "Unexpected error");
        }

        private static void OnDomainException(object sender, UnhandledExceptionEventArgs e) {
            Report(e.ExceptionObject as Exception, "Unexpected error");
        }

        private static void Report(Exception exception, string caption) {
            if (exception == null) return;
            MessageBox.Show(exception.Message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
