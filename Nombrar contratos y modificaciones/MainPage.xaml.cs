using Microsoft.Maui.Storage;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using System.Diagnostics;


namespace Nombrar_contratos_y_modificaciones
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
        }

        private async void CargarContratos(object sender, EventArgs e)
        {
            try
            {
                // Configurar el selector de archivos
                var options = new PickOptions
                {
                    PickerTitle = "Selecciona uno o varios archivos PDF",
                    FileTypes = FilePickerFileType.Pdf
                };

                // Permitir múltiples archivos
                var archivos = await FilePicker.Default.PickMultipleAsync(options);
                if (archivos == null)
                    return;
                foreach (var archivo in archivos)
                {
                    try
                    {
                        string textoExtraido = "";
                        using (PdfDocument pdf = PdfDocument.Open(archivo.FullPath))
                        {
                            foreach (var pagina in pdf.GetPages())
                            {
                                textoExtraido += pagina.Text + Environment.NewLine;
                            }
                        }

                        string nombre = ExtraerNombre(textoExtraido);
                        if (!string.IsNullOrEmpty(nombre))
                        {
                            string nuevoNombre = $"contrato {nombre}.pdf";
                            string carpetaDestino = await SeleccionarCarpetaUsuario(archivo.FullPath);
                            string rutaDestino = Path.Combine(carpetaDestino, nuevoNombre);

                            // Copiar el archivo original al nuevo nombre
                            File.Copy(archivo.FullPath, rutaDestino, overwrite: true);

                            await DisplayAlert("PDF procesado",
                                $"Archivo renombrado como: {nuevoNombre}", "OK");
                            await DisplayAlert("Ruta", $"Guardado en: {rutaDestino}", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Aviso",
                                $"No se encontró nombre en: {archivo.FileName}", "OK");

                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "Cerrar");
                    }

                }


            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los PDFs: {ex.Message}", "Cerrar");
            }


        }//CargarArchivos

        private string ExtraerNombre(string textoExtraido)
        {
            if (string.IsNullOrWhiteSpace(textoExtraido))
                return null;
            var marcador = new[]
            {
              @",\s*y\s+por\s+otra\s+([A-ZÁÉÍÓÚÜÑ\s]+)\s+mayor\s+de\s+edad",
              @"([A-ZÁÉÍÓÚÜÑ\s]{10,})\s+mayor\s+de\s+edad",
              @"comparece\s+([A-ZÁÉÍÓÚÜÑ\s]+)\s+mayor\s+de\s+edad",
            };
            foreach (var patron in marcador)
            {
                var regex = new Regex(patron, RegexOptions.IgnoreCase);
                var match = regex.Match(textoExtraido);

                if (match.Success)
                {
                    // Group[1] captura el nombre después del marcador
                    string nombre = match.Groups[1].Value.Trim();

                    // Cortar en salto de línea si hay más texto
                    nombre = nombre.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0];

                    return nombre;
                }

            }
            return null;
        }//fin extraer nombre

        private async Task<string> SeleccionarCarpetaUsuario(string ruta)
        {
            try
            {
                // Obtener el directorio del archivo original
                string directorioOriginal = Path.GetDirectoryName(ruta);

                if (string.IsNullOrEmpty(directorioOriginal))
                {
                    // Fallback si no se puede obtener el directorio
                    directorioOriginal = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }

                // Crear la carpeta "CONTRATOS" dentro del mismo directorio
                string carpetaContratos = Path.Combine(directorioOriginal, "CONTRATOS");

                // Asegurarse de que la carpeta existe
                Directory.CreateDirectory(carpetaContratos);

                return carpetaContratos;
            }
            catch (Exception ex)
            {
                // En caso de error, usar una ubicación por defecto
                string carpetaPorDefecto = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "CONTRATOS");

                Directory.CreateDirectory(carpetaPorDefecto);
                return carpetaPorDefecto;
            }
        }//Seleccionar carpeta usuario

        private async void CargarModificaciones(object sender, EventArgs e)
        {
            try
            {
                // Configurar el selector de archivos
                var options = new PickOptions
                {
                    PickerTitle = "Selecciona uno o varios archivos PDF",
                    FileTypes = FilePickerFileType.Pdf
                };

                // Permitir múltiples archivos
                var archivos = await FilePicker.Default.PickMultipleAsync(options);
                if (archivos == null)
                    return;
                foreach (var archivo in archivos)
                {
                    try
                    {
                        string textoExtraido = "";
                        using (PdfDocument pdf = PdfDocument.Open(archivo.FullPath))
                        {
                            foreach (var pagina in pdf.GetPages())
                            {
                                textoExtraido += pagina.Text + Environment.NewLine;
                            }
                        }

                        var nombreExtraido = ExtraerNombreModificacion(textoExtraido);
                        string nombre = Regex.Replace(nombreExtraido.ToString(), @"\s+", " ");
                        nombre = Regex.Replace(nombreExtraido.ToString(), @"\s+", " ");
                        nombre = Regex.Replace(nombreExtraido.ToString(), @"[\(\)\,]", "").Trim();
                        if (!string.IsNullOrEmpty(nombre.ToString()))
                        {
                            string nuevoNombre = $"Adición contrato {nombre}.pdf";
                            string carpetaDestino = await SeleccionarCarpetaUsuario(archivo.FullPath);
                            string rutaDestino = Path.Combine(carpetaDestino, nuevoNombre);

                            // Copiar el archivo original al nuevo nombre
                            File.Copy(archivo.FullPath, rutaDestino, overwrite: true);

                            await DisplayAlert("PDF procesado",
                                $"Archivo renombrado como: {nuevoNombre}", "OK");
                            await DisplayAlert("Ruta", $"Guardado en: {rutaDestino}", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Aviso",
                                $"No se encontró nombre en: {archivo.FileName}", "OK");

                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "Cerrar");
                    }

                }


            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los PDFs: {ex.Message}", "Cerrar");
            }
        }//fin cargar modificaciones

        private(string nombre, string fecha) ExtraerNombreModificacion(string textoExtraido)
        {
            if (string.IsNullOrWhiteSpace(textoExtraido))
                return (null,null);
            string nombreEncontrado = null;
            string fechaEncontrado = null;
            var marcadorNombre = new[]
            {
              @",\s*y\s+por\s+otra\s+parte\s+([A-ZÁÉÍÓÚÜÑ][A-ZÁÉÍÓÚÜÑ\s]+(?=\s+mayor\s+de\s+edad))",
              @",\s*y\s+por\s+otra\s+([A-ZÁÉÍÓÚÜÑ][A-ZÁÉÍÓÚÜÑ\s]+(?=\s+mayor\s+de\s+edad))",
              @"por\s+otra\s+parte\s+([A-ZÁÉÍÓÚÜÑ][A-ZÁÉÍÓÚÜÑ\s]+(?=\s+mayor\s+de\s+edad))",
              @"compareciente\s+([A-ZÁÉÍÓÚÜÑ][A-ZÁÉÍÓÚÜÑ\s]+(?=\s+mayor\s+de\s+edad))"
            };
            var marcadorFecha = new[]
            {
              @"a\s+los\s+\w+\s*\(\d+\)\s+días\s+del\s+mes\s+de\s+([a-z]+)\s+de\s+(\d{4})",
              @"a\s+los\s+\d+\s+días\s+del\s+mes\s+de\s+([a-z]+)\s+de\s+(\d{4})",
              @"del\s+mes\s+de\s+([a-z]+)\s+de\s+(\d{4})",
              @"mes\s+de\s+([a-z]+)\s+de\s+(\d{4})",
            };
            foreach (var patron in marcadorNombre)
            {
                try
                {
                    var regex = new Regex(patron, RegexOptions.IgnoreCase);
                    var match = regex.Match(textoExtraido);

                    if (match.Success && match.Groups.Count > 1)
                    {
                        nombreEncontrado = match.Groups[1].Value.Trim();
                        // Limpiar el nombre de posibles caracteres no deseados
                        nombreEncontrado = Regex.Replace(nombreEncontrado, @"\s+", " ");
                        nombreEncontrado = Regex.Replace(nombreEncontrado, @"\s+", " ");
                        nombreEncontrado = Regex.Replace(nombreEncontrado, @"[\(\)\,]", "").Trim();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error en patrón {patron}: {ex.Message}");
                }
            }
            foreach (var patron in marcadorFecha)
            {
                var regex = new Regex(patron, RegexOptions.IgnoreCase);
                var match = regex.Match(textoExtraido);

                if (match.Success)
                {
                    // Group[1] captura el nombre después del marcador
                    fechaEncontrado = match.Groups[1].Value.Trim();
                    fechaEncontrado = Regex.Replace(fechaEncontrado, @"\s+", " ");
                    fechaEncontrado = Regex.Replace(fechaEncontrado, @"\s+", " ");
                    fechaEncontrado = Regex.Replace(fechaEncontrado, @"[\(\)\,]", "").Trim();
                    break;
                    
                }

            }
            return (nombreEncontrado, fechaEncontrado);
        }//fin extraer nombre y fecha modificacion

    }//fin de la clase
}
