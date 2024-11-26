using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApplicationMaui.Models;

namespace TestApplicationMaui.ViewModels
{
    public class TestViewModel : BaseViewModel
    {

        private ObservableCollection<Person> people;
        public ObservableCollection<Person> People 
        {
            get
            {
                return people;
            }
            set
            {
                if(value != people)
                {
                    people = value;
                    OnPropertyChanged(nameof(People));
                }
            }
        }

        private Person selectedPerson;
        public Person SelectedPerson 
        {
            get
            {
                return selectedPerson;
            }
            set
            {
                if (value != selectedPerson)
                {
                    selectedPerson = value;
                    OnPropertyChanged(nameof(SelectedPerson));
                }
            }
        }
    }
}
