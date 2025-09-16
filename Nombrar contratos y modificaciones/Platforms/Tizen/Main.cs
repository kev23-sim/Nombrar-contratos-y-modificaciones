using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Nombrar_contratos_y_modificaciones
{
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
