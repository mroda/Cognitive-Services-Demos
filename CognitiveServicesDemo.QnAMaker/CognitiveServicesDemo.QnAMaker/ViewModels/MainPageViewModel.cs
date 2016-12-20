using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CognitiveServicesDemo.QnAMaker.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public Command MakeQuestionCommand { get; set; }
        public QnAMakerService service;

        private string answer = string.Empty;
        public string Answer
        {
            get { return answer; }
            set
            {
                answer = value;
                OnPropertyChanged();
            }
        }
        
        private string score = string.Empty;
        public string Score
        {
            get { return score; }
            set
            {
                score = value;
                OnPropertyChanged();
            }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel()
        {
            MakeQuestionCommand = new Command<string>((parameter) => OnMakeQuestion(parameter));
            service = new QnAMakerService();
        }

        private async void OnMakeQuestion(string question)
        {
            IsBusy = true;
            this.Answer = string.Empty;
            this.Score = string.Empty;


            var answer = await service.GetAnswer(question);

            if (answer == null)
            {
                this.Answer = "Lo siento no pude encontrar una respuessta";
                this.Score = string.Empty;
            }

            this.Answer = answer.Text;
            this.Score = $"Score: {answer.Score}" ;

            IsBusy = false;
        }

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged == null)
            {
                return;
            }

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
