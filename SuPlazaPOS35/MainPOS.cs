using System;
using System.Windows.Forms;
using DsiCodeTech.Mapper.Profiles;
using SuPlazaPOS35.view;

namespace SuPlazaPOS35
{
    internal static class MainPOS
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// Modify by: DSI Code Tech
        /// Description: Se Agrega NLog para el manejo de las excepciones
        /// </summary>
        [STAThread]
        static void Main()
        {
            AutoMapperProfiles.Run();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(defaultValue: false);
            try
            {
                Application.Run(new Login());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }
}

