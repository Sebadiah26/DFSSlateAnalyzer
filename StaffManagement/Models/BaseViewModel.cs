using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StaffManagement.Models
{
    public class BaseViewModel : INotifyPropertyChanged

    {


        public BaseViewModel()
        {
        }


        public string PageTitle { get; set; }
        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        internal void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        public event PropertyChangedEventHandler PropertyChanged;





    }
}
