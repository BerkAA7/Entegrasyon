using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Documents;
using WPF_LoginForm.View;

namespace ExcelToPanorama.Helpers
{
    internal class ErrorHelpers
    {

        #region ERRORS
        public void AppendErrorMessage(string message, string Kod, byte AktarimTip )
        {
         LoginView loginView = new LoginView();
            string fullMessage = message;
            string AktarimTipString = "";
            if (AktarimTip == 0)
                AktarimTipString = "Müşteri Kodu:";
            else if (AktarimTip == 1)
                AktarimTipString = "Ürün Kodu:";

            fullMessage = $"{AktarimTipString} {Kod} - {fullMessage}";

   

            // Yeni bir paragraf oluşturuyoruz
            Paragraph paragraph = new Paragraph(new Run(fullMessage));

            // RichTextBox'a paragrafı ekliyoruz
            loginView.RbtErrorMessageErrorHelpers(paragraph);
        }


        public string ParseErrorMessage(string response)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);
            var errorNode = xmlDoc.SelectSingleNode("//error");
            return errorNode?.InnerText ?? "Bilinmeyen bir hata oluştu.";
        }


        public string ParseErrorMessageFromResponse(string responseString)
        {
            try
            {
                var xDoc = XDocument.Parse(responseString);
                var errorElements = xDoc.Descendants().Where(e => e.Name.LocalName == "Hata");
                List<string> errorMessages = new List<string>();
                foreach (var errorElement in errorElements)
                {
                    errorMessages.Add(errorElement.Value);
                }
                return string.Join("\n", errorMessages);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during XML parsing
                return $"XML Yanıtı çözümleme hatası: {ex.Message}";
            }
        }

        #endregion
    }

    public enum AktarimTipEnum
    {
        MusteriKod = 0,
        UrunKod = 1
        
    }
}
