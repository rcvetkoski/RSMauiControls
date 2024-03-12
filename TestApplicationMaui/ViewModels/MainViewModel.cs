using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApplicationMaui.Models;

namespace TestApplicationMaui.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        public ObservableCollection<Person> People { get; set; }
        public ObservableCollection<Person> SelectedPeople { get; set; }


        public MainViewModel()
        {
            People = new ObservableCollection<Person>()
            {
                new Person(){Name = "Tom", Age = 26},
                new Person(){Name = "Natalie", Age = 20}
            };

            SelectedPeople = new ObservableCollection<Person>();

            SelectedPeople.Add(People[0]);

            Error = "Error message !";

            TrailingIconCommand = new Command<object>(TrailingIconMethod);
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


        public ICommand TrailingIconCommand { get; set; }
        private void TrailingIconMethod(object obj)
        {
            if(obj is Entry)
            {
                (obj as Entry).IsPassword = (obj as Entry).IsPassword ? false : true;
            }

            Console.WriteLine("TrailingIconMethod");
        }
    }
}
