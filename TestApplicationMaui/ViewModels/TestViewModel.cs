using System.Collections.ObjectModel;
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

        public List<float> ChartData { get; set; }

        public TestViewModel()
        {
            ChartData = new List<float> { 10, 30, 20, 40, 50, 35, 5, 23, 45, 85};
        }
    }
}
