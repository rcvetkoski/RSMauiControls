using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApplicationMaui.Models;

namespace TestApplicationMaui.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        public ObservableCollection<Person> People { get; set; }   
        
        public MainViewModel()
        {
            People = new ObservableCollection<Person>()
            {
                new Person(){Name = "Tom", Age = 26},
                new Person(){Name = "Natalie", Age = 20}
            };

            Error = "Error message !";
        }

        private string error;
        public string Error 
        {
            get { return error; }
            set
            {
                if(error != value) 
                { 
                    error = value;
                    OnPropertyChanged(nameof(Error));   
                }
            }
        }       
    }
}
