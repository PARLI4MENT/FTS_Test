using FTS_XML_UI_Netframework.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTS_XML_UI_Netframework.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Title => GET/SET
        private string _Title = "Title";
        
        /// <summary> Заголовок окна </summary>
        public string Title
        {
            get => _Title;
            set => Set(ref _Title, value);
        }
        #endregion
    }
}