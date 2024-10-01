using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestApplicationMaui.Helpers.Enum;
using TestApplicationMaui.Models;

namespace TestApplicationMaui.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        public ObservableCollection<Person> People { get; set; }
        public Person SelectedPerson { get; set; }  
        public ObservableCollection<Person> SelectedPeople { get; set; }


        public ObservableCollection<DaysOfWeekEnum> DaysOfWeek { get; set; }
        public ObservableCollection<DaysOfWeekEnum> SelectedDaysOfWeek { get; set; }

        private TimeSpan time;
        public TimeSpan Time
        {
            get
            {
                return time;
            }
            set
            {
                if(value != time)
                {
                    time = value;
                    OnPropertyChanged(nameof(Time));
                }
            }
        }

        public MainViewModel()
        {
            People = new ObservableCollection<Person>()
            {
                new Person(){Name = "Tom", Age = 26},
                new Person(){Name = "Natalie", Age = 20},
                new Person(){Name = "Yukari", Age = 16},
                new Person(){Name = "Mitsuru", Age = 19}
            };

            SelectedPerson = People.ElementAt(2);

            SelectedPeople = new ObservableCollection<Person>();

            SelectedPeople.Add(People[0]);
            Time = TimeSpan.FromMilliseconds(10000);

            DaysOfWeek = new ObservableCollection<DaysOfWeekEnum>()
            {
                DaysOfWeekEnum.Monday,
                DaysOfWeekEnum.Tuesday,
                DaysOfWeekEnum.Wednesday,
                DaysOfWeekEnum.Thursday,
                DaysOfWeekEnum.Friday,
                DaysOfWeekEnum.Saturday,
                DaysOfWeekEnum.Sunday
            };

            SelectedDaysOfWeek = new ObservableCollection<DaysOfWeekEnum>()
            { 
                DaysOfWeekEnum.Friday
            };

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
